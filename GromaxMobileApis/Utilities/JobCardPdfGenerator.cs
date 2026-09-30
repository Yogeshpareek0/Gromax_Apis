using GromaxMobileApis.Models.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Document = iTextSharp.text.Document;
using Font = iTextSharp.text.Font;
using Rectangle = iTextSharp.text.Rectangle;

namespace GromaxMobileApis.Utilities
{
    /// <summary>
    /// Job Card print PDF - existing ResponseJobCardMaster model se
    /// </summary>
    public class JobCardPdfGenerator
    {
        // ============ COLORS ============
        private static readonly BaseColor HEADER_BLUE = new BaseColor(49, 142, 194);
        private static readonly BaseColor HEADER_BLUE_DARK = new BaseColor(37, 110, 153);
        private static readonly BaseColor LIGHT_BLUE = new BaseColor(230, 240, 250);
        private static readonly BaseColor LIGHT_BLUE_ALT = new BaseColor(242, 248, 253);
        private static readonly BaseColor ACCENT_RED = new BaseColor(200, 50, 50);
        private static readonly BaseColor LIGHT_RED = new BaseColor(255, 241, 241);
        private static readonly BaseColor GREEN = new BaseColor(34, 139, 84);
        private static readonly BaseColor GRAY_TEXT = new BaseColor(100, 112, 125);
        private static readonly BaseColor BORDER = new BaseColor(199, 211, 222);
        private static readonly BaseColor INK = new BaseColor(31, 42, 55);
        private static readonly BaseColor WHITE = BaseColor.White;

        // ============ FONTS ============
        private static readonly BaseFont BF_REGULAR = LoadBaseFont(false);
        private static readonly BaseFont BF_BOLD = LoadBaseFont(true);

        private static readonly Font F_DEALER = new Font(BF_BOLD, 13, Font.NORMAL, WHITE);
        private static readonly Font F_HEADER_SMALL = new Font(BF_REGULAR, 7.5f, Font.NORMAL, WHITE);
        private static readonly Font F_TITLE = new Font(BF_BOLD, 16, Font.NORMAL, WHITE);
        private static readonly Font F_TITLE_META = new Font(BF_BOLD, 8.5f, Font.NORMAL, WHITE);
        private static readonly Font F_SECTION = new Font(BF_BOLD, 8.5f, Font.NORMAL, HEADER_BLUE_DARK);
        private static readonly Font F_LABEL = new Font(BF_REGULAR, 7, Font.NORMAL, GRAY_TEXT);
        private static readonly Font F_VALUE_BOLD = new Font(BF_BOLD, 8.5f, Font.NORMAL, INK);
        private static readonly Font F_TH = new Font(BF_BOLD, 7.5f, Font.NORMAL, WHITE);
        private static readonly Font F_TD = new Font(BF_REGULAR, 7.5f, Font.NORMAL, INK);
        private static readonly Font F_TD_BOLD = new Font(BF_BOLD, 8, Font.NORMAL, HEADER_BLUE_DARK);
        private static readonly Font F_TOTAL_LABEL = new Font(BF_REGULAR, 8.5f, Font.NORMAL, INK);
        private static readonly Font F_TOTAL_VALUE = new Font(BF_BOLD, 8.5f, Font.NORMAL, INK);
        private static readonly Font F_GRAND = new Font(BF_BOLD, 10, Font.NORMAL, WHITE);
        private static readonly Font F_BALANCE = new Font(BF_BOLD, 9, Font.NORMAL, ACCENT_RED);
        private static readonly Font F_PAID = new Font(BF_BOLD, 9, Font.NORMAL, GREEN);
        private static readonly Font F_WORDS = new Font(BF_BOLD, 8, Font.NORMAL, INK);
        private static readonly Font F_NOTE = new Font(BF_REGULAR, 7.5f, Font.NORMAL, GRAY_TEXT);
        private static readonly Font F_SIGN = new Font(BF_BOLD, 8, Font.NORMAL, INK);
        private static readonly Font F_FOOTER = new Font(BF_REGULAR, 7, Font.NORMAL, GRAY_TEXT);

        private static readonly CultureInfo IN = new CultureInfo("en-IN");

        private static BaseFont LoadBaseFont(bool bold)
        {
            string path = bold ? @"C:\Windows\Fonts\arialbd.ttf" : @"C:\Windows\Fonts\arial.ttf";
            try
            {
                if (File.Exists(path))
                    return BaseFont.CreateFont(path, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            }
            catch { }

            return BaseFont.CreateFont(bold ? BaseFont.HELVETICA_BOLD : BaseFont.HELVETICA,
                                       BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        }

        // ============ PUBLIC ============

        public byte[] Generate(ResponseJobCardMaster data)
        {
            var m = data?.resJobCardMaster?.FirstOrDefault();
            if (m == null) throw new Exception("Job card data is required.");

            var complaints = data.resJobCardComplaint ?? new List<JobCardComplaint>();
            var missingParts = data.resJobCardMissingPart ?? new List<JobCardMissingPart>();
            var spareParts = data.resJobCardSparePart ?? new List<JobCardSparePart>();
            var localParts = data.resJobCardLocalPart ?? new List<JobCardLocalPart>();

            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 20, 20, 20, 34);
                var writer = PdfWriter.GetInstance(doc, ms);
                writer.SetFullCompression();
                writer.CompressionLevel = 9;
                writer.PageEvent = new JobCardPageEvent(m.JobCardNo, F_FOOTER, BORDER);

                doc.Open();

                AddHeader(doc, m);
                AddSpacing(doc, 6);
                AddMetaStrip(doc, m);
                AddSpacing(doc, 8);
                AddCustomerAndTractor(doc, m);
                AddSpacing(doc, 8);
                AddTyrePressure(doc, m);

                

                AddSpacing(doc, 8);
                AddComplaints(doc, complaints);
                AddSpacing(doc, 8);
                AddMissingParts(doc, missingParts);
                AddSpacing(doc, 8);
                AddSpareParts(doc, spareParts, m);

                if (localParts.Count > 0)
                {
                    AddSpacing(doc, 8);
                    AddLocalParts(doc, localParts, m);
                }

                if (m.SubletTotal > 0 || !IsEmpty(m.SubletDescription))
                {
                    AddSpacing(doc, 8);
                    AddSublet(doc, m);
                }

                AddSpacing(doc, 8);
                AddTimeAndCost(doc, m);
                AddSpacing(doc, 8);
                AddSummary(doc, m);
                AddSpacing(doc, 10);
                AddSignatures(doc, m);

                doc.Close();
                return ms.ToArray();
            }
        }

        // ============ SECTIONS ============

        private void AddHeader(Document doc, JobCardMaster m)
        {
            var t = new PdfPTable(2) { WidthPercentage = 100 };
            t.SetWidths(new float[] { 1.7f, 1f });

            var left = new PdfPCell { BackgroundColor = HEADER_BLUE, Border = Rectangle.NO_BORDER, Padding = 10 };
            left.AddElement(new Paragraph(Dash(m.DealerName), F_DEALER) { SpacingAfter = 3 });
            if (!IsEmpty(m.DealerCode))
                left.AddElement(new Paragraph($"Dealer Code: {m.DealerCode}", F_HEADER_SMALL));
            t.AddCell(left);

            var right = new PdfPCell { BackgroundColor = HEADER_BLUE_DARK, Border = Rectangle.NO_BORDER, Padding = 10 };
            right.AddElement(new Paragraph("JOB CARD", F_TITLE) { Alignment = Element.ALIGN_RIGHT, SpacingAfter = 4 });
            right.AddElement(new Paragraph($"No : {Dash(m.JobCardNo)}", F_TITLE_META) { Alignment = Element.ALIGN_RIGHT });
            right.AddElement(new Paragraph($"Date : {FormatDate(m.JobCardDate)}", F_TITLE_META) { Alignment = Element.ALIGN_RIGHT });
            t.AddCell(right);

            doc.Add(t);
        }

        private void AddMetaStrip(Document doc, JobCardMaster m)
        {
            var t = new PdfPTable(3) { WidthPercentage = 100, KeepTogether = true };
            t.AddCell(InfoCell("Job Card Type", m.JobCardType, bg: LIGHT_BLUE_ALT));
            t.AddCell(InfoCell("Fuel Level", m.Fuel, bg: LIGHT_BLUE_ALT));
            t.AddCell(InfoCell("Work Done By", m.WorkDoneBy, bg: LIGHT_BLUE_ALT));
            doc.Add(t);
        }

        private void AddCustomerAndTractor(Document doc, JobCardMaster m)
        {
            var cust = new PdfPTable(2) { WidthPercentage = 100 };
            cust.AddCell(TitleCell("Customer Details", 2));
            cust.AddCell(InfoCell("Customer Name", m.CustomerName, 2));
            cust.AddCell(InfoCell("Address", m.CustomerAddress, 2));
            cust.AddCell(InfoCell("Mobile No.", m.MobileNo));
            cust.AddCell(InfoCell("Alternate Mobile No.", m.AlternateMobileNo));

            var tractor = new PdfPTable(2) { WidthPercentage = 100 };
            tractor.AddCell(TitleCell("Tractor Details", 2));
            tractor.AddCell(InfoCell("Chassis No.", m.ChassiNumber, 2));
            tractor.AddCell(InfoCell("Tractor Sl. No.", m.TractorSlNo, 2));
            tractor.AddCell(InfoCell("Regn. No.", m.RegnNo));
            tractor.AddCell(InfoCell("Date of Sale", FormatDate(m.DateOfSale)));

            var outer = new PdfPTable(2) { WidthPercentage = 100, KeepTogether = true };
            outer.SetWidths(new float[] { 1f, 1f });
            outer.AddCell(new PdfPCell(cust) { Border = Rectangle.NO_BORDER, PaddingRight = 4 });
            outer.AddCell(new PdfPCell(tractor) { Border = Rectangle.NO_BORDER, PaddingLeft = 4 });
            doc.Add(outer);
        }

        private void AddTyrePressure(Document doc, JobCardMaster m)
        {
            var t = new PdfPTable(4) { WidthPercentage = 100, KeepTogether = true };
            t.AddCell(TitleCell("Tyre Pressure", 4));
            t.AddCell(InfoCell("Front (Left)", Num(m.FrontTyrePressureLeft)));
            t.AddCell(InfoCell("Front (Right)", Num(m.FrontTyrePressureRight)));
            t.AddCell(InfoCell("Rear (Left)", Num(m.RearTyrePressureLeft)));
            t.AddCell(InfoCell("Rear (Right)", Num(m.RearTyrePressureRight)));
            doc.Add(t);
        }

        
        private void AddComplaints(Document doc, List<JobCardComplaint> list)
        {
            var t = new PdfPTable(new float[] { 5, 37, 37, 21 }) { WidthPercentage = 100, HeaderRows = 2 };
            t.AddCell(TitleCell("Customer's Complaints", 4));
            t.AddCell(TH("#"));
            t.AddCell(TH("Customer's Complaint"));
            t.AddCell(TH("Action Taken / Analysis"));
            t.AddCell(TH("Remark"));

            if (list.Count == 0)
            {
                t.AddCell(TD("No complaints recorded", WHITE, colspan: 4));
            }
            else
            {
                int i = 0;
                foreach (var c in list)
                {
                    var bg = Row(i);
                    t.AddCell(TD((i + 1).ToString(), bg));
                    t.AddCell(TD(Dash(c.Complaint), bg, Element.ALIGN_LEFT));
                    t.AddCell(TD(Dash(c.ActionTaken), bg, Element.ALIGN_LEFT));
                    t.AddCell(TD(Dash(c.Remark), bg, Element.ALIGN_LEFT));
                    i++;
                }
            }
            doc.Add(t);
        }

        private void AddMissingParts(Document doc, List<JobCardMissingPart> list)
        {
            var t = new PdfPTable(new float[] { 5, 95 }) { WidthPercentage = 100, HeaderRows = 2 };
            t.AddCell(TitleCell("Missing Parts / Accessories", 2));
            t.AddCell(TH("#"));
            t.AddCell(TH("Part / Accessory Name"));

            if (list.Count == 0)
            {
                t.AddCell(TD("Nil", WHITE, colspan: 2));
            }
            else
            {
                int i = 0;
                foreach (var p in list)
                {
                    var bg = Row(i);
                    t.AddCell(TD((i + 1).ToString(), bg));
                    t.AddCell(TD(Dash(p.PartDescription), bg, Element.ALIGN_LEFT));
                    i++;
                }
            }
            doc.Add(t);
        }

        private void AddSpareParts(Document doc, List<JobCardSparePart> list, JobCardMaster m)
        {
            var t = new PdfPTable(new float[] { 4, 12, 24, 6, 11, 7, 9, 11, 16 }) { WidthPercentage = 100, HeaderRows = 2 };
            t.AddCell(TitleCell("Spare Parts - Statement cum Estimate", 9));
            foreach (var h in new[] { "#", "Part No.", "Part Name", "Qty", "MRP (Incl. GST)", "GST %", "GST Amt.", "Total", "Remark" })
                t.AddCell(TH(h));

            decimal sum = 0;
            int i = 0;
            foreach (var p in list)
            {
                decimal qty = p.Qty ?? 0;
                decimal mrp = p.BasePrice ?? 0;
                decimal rate = p.GstRate ?? 0;
                decimal total = (p.TotalPrice ?? 0) > 0 ? p.TotalPrice.Value : qty * mrp;
                decimal gstAmt = rate > 0 ? total - (total / (1 + rate / 100m)) : 0;
                sum += total;

                var bg = Row(i);
                t.AddCell(TD((i + 1).ToString(), bg));
                t.AddCell(TD(Dash(p.PartNumber), bg));
                t.AddCell(TD(Dash(p.PartName), bg, Element.ALIGN_LEFT));
                t.AddCell(TD(Num(qty), bg));
                t.AddCell(TD(Money(mrp), bg, Element.ALIGN_RIGHT));
                t.AddCell(TD(Num(rate) + "%", bg));
                t.AddCell(TD(Money(gstAmt), bg, Element.ALIGN_RIGHT));
                t.AddCell(TD(Money(total), bg, Element.ALIGN_RIGHT));
                t.AddCell(TD(Dash(p.Remark), bg, Element.ALIGN_LEFT));
                i++;
            }

            if (i == 0) t.AddCell(TD("No spare parts", WHITE, colspan: 9));

            AddSubTotalRow(t, 7, m.SpareTotal > 0 ? m.SpareTotal : sum, 1);
            doc.Add(t);
        }

        private void AddLocalParts(Document doc, List<JobCardLocalPart> list, JobCardMaster m)
        {
            var t = new PdfPTable(new float[] { 4, 24, 12, 6, 10, 7, 9, 12, 16 }) { WidthPercentage = 100, HeaderRows = 2 };
            t.AddCell(TitleCell("Local Parts - Statement cum Estimate", 9));
            foreach (var h in new[] { "#", "Part Name", "Part No.", "Qty", "Price", "GST %", "GST Amt.", "Total (Incl. GST)", "Remark" })
                t.AddCell(TH(h));

            decimal sum = 0;
            int i = 0;
            foreach (var p in list)
            {
                decimal qty = p.Qty ?? 0;
                decimal price = p.Price ?? 0;
                decimal rate = p.GSTRate ?? 0;
                decimal baseAmt = qty * price;
                decimal gstAmt = baseAmt * rate / 100m;
                decimal total = baseAmt + gstAmt;
                sum += total;

                var bg = Row(i);
                t.AddCell(TD((i + 1).ToString(), bg));
                t.AddCell(TD(Dash(p.PartName), bg, Element.ALIGN_LEFT));
                t.AddCell(TD(Dash(p.PartNumber), bg));
                t.AddCell(TD(Num(qty), bg));
                t.AddCell(TD(Money(price), bg, Element.ALIGN_RIGHT));
                t.AddCell(TD(Num(rate) + "%", bg));
                t.AddCell(TD(Money(gstAmt), bg, Element.ALIGN_RIGHT));
                t.AddCell(TD(Money(total), bg, Element.ALIGN_RIGHT));
                t.AddCell(TD(Dash(p.Remark), bg, Element.ALIGN_LEFT));
                i++;
            }

            AddSubTotalRow(t, 7, m.LocalTotal > 0 ? m.LocalTotal : sum, 1);
            doc.Add(t);
        }

        private void AddSublet(Document doc, JobCardMaster m)
        {
            var t = new PdfPTable(new float[] { 75, 25 }) { WidthPercentage = 100, KeepTogether = true };
            t.AddCell(TitleCell("Sublet Work", 2));
            t.AddCell(TH("Description"));
            t.AddCell(TH("Cost (Rs.)"));
            t.AddCell(TD(Dash(m.SubletDescription), WHITE, Element.ALIGN_LEFT));
            t.AddCell(TD(Money(m.SubletTotal), WHITE, Element.ALIGN_RIGHT, F_TD_BOLD));
            doc.Add(t);
        }

        private void AddTimeAndCost(Document doc, JobCardMaster m)
        {
            var t = new PdfPTable(3) { WidthPercentage = 100, KeepTogether = true };
            t.AddCell(TitleCell("Cost & Time Estimated", 3));
            t.AddCell(InfoCell("Hours (HMR)", Num(m.Hours)));
            t.AddCell(InfoCell("Time Estimate (Hrs)", Num(m.TimeEstimate)));
            t.AddCell(InfoCell("Time Actual (Hrs)", Num(m.TimeActual)));
            t.AddCell(InfoCell("Cost Estimate (Rs.)", Money(m.CostEstimate)));
            t.AddCell(InfoCell("Cost Actual (Rs.)", Money(m.CostActual)));
            t.AddCell(InfoCell("Labour Cost (Rs.)", Money(m.LabourTotal)));
            doc.Add(t);
        }

        private void AddSummary(Document doc, JobCardMaster m)
        {
            decimal labour = m.LabourTotal ?? 0;
            decimal paid = m.TotalAmountPaid ?? 0;
            decimal balance = m.GrandTotal - paid;

            var left = new PdfPCell
            {
                Border = Rectangle.BOX,
                BorderColor = BORDER,
                BackgroundColor = LIGHT_BLUE_ALT,
                Padding = 8
            };
            left.AddElement(new Paragraph("Amount in Words", F_LABEL) { SpacingAfter = 2 });
            left.AddElement(new Paragraph(NumberToWordsHelper.Convert(m.GrandTotal), F_WORDS) { SpacingAfter = 8 });
            left.AddElement(new Paragraph(
                "Note: Spare part prices are MRP inclusive of GST. Local part prices are exclusive of GST; GST is added as shown. " +
                "Estimates may change after detailed inspection.", F_NOTE));

            var totals = new PdfPTable(new float[] { 1.4f, 1f }) { WidthPercentage = 100 };
            AddTotalLine(totals, "Spare Parts", m.SpareTotal);
            AddTotalLine(totals, "Local Parts", m.LocalTotal);
            AddTotalLine(totals, "Labour Charges", labour);
            AddTotalLine(totals, "Sublet Charges", m.SubletTotal);

            totals.AddCell(new PdfPCell(new Phrase("GRAND TOTAL", F_GRAND))
            {
                BackgroundColor = HEADER_BLUE_DARK,
                BorderColor = HEADER_BLUE_DARK,
                Padding = 6
            });
            totals.AddCell(new PdfPCell(new Phrase("Rs. " + Money(m.GrandTotal), F_GRAND))
            {
                BackgroundColor = HEADER_BLUE_DARK,
                BorderColor = HEADER_BLUE_DARK,
                Padding = 6,
                HorizontalAlignment = Element.ALIGN_RIGHT
            });

            AddTotalLine(totals, "Amount Paid", paid, F_PAID);
            AddTotalLine(totals, "Balance Due", balance, balance > 0 ? F_BALANCE : F_TOTAL_VALUE,
                         balance > 0 ? LIGHT_RED : WHITE);

            var outer = new PdfPTable(new float[] { 1.3f, 1f }) { WidthPercentage = 100, KeepTogether = true };
            outer.AddCell(left);
            outer.AddCell(new PdfPCell(totals) { Border = Rectangle.NO_BORDER, PaddingLeft = 6 });
            doc.Add(outer);
        }

        private void AddSignatures(Document doc, JobCardMaster m)
        {
            var t = new PdfPTable(3) { WidthPercentage = 100, KeepTogether = true };

            t.AddCell(new PdfPCell(new Phrase(
                "Customer Declaration: I hereby confirm that the above mentioned work has been carried out to my satisfaction " +
                "and I have received my tractor in good condition along with all belongings.", F_NOTE))
            {
                Colspan = 3,
                Border = Rectangle.BOX,
                BorderColor = BORDER,
                BackgroundColor = LIGHT_BLUE_ALT,
                Padding = 6
            });

            t.AddCell(SignCell("Work Done By", m.WorkDoneBy));
            t.AddCell(SignCell("Workshop Manager", m.CreatedBy));
            t.AddCell(SignCell("Customer Signature", m.CustomerName));

            doc.Add(t);
        }

        // ============ CELL HELPERS ============

        private PdfPCell TitleCell(string title, int colspan)
        {
            return new PdfPCell(new Phrase(title.ToUpperInvariant(), F_SECTION))
            {
                Colspan = colspan,
                BackgroundColor = LIGHT_BLUE,
                UseVariableBorders = true,
                BorderColor = BORDER,
                BorderColorLeft = HEADER_BLUE,
                BorderWidthLeft = 3f,
                PaddingTop = 5,
                PaddingBottom = 6,
                PaddingLeft = 8
            };
        }

        private PdfPCell InfoCell(string label, string value, int colspan = 1, BaseColor bg = null)
        {
            var c = new PdfPCell
            {
                Colspan = colspan,
                BackgroundColor = bg ?? WHITE,
                BorderColor = BORDER,
                PaddingTop = 3,
                PaddingBottom = 6,
                PaddingLeft = 6,
                PaddingRight = 6
            };
            c.AddElement(new Paragraph(label, F_LABEL));
            c.AddElement(new Paragraph(Dash(value), F_VALUE_BOLD));
            return c;
        }

        private PdfPCell TH(string text)
        {
            return new PdfPCell(new Phrase(text, F_TH))
            {
                BackgroundColor = HEADER_BLUE,
                BorderColor = BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingTop = 4,
                PaddingBottom = 5
            };
        }

        private PdfPCell TD(string text, BaseColor bg, int align = Element.ALIGN_CENTER, Font font = null, int colspan = 1)
        {
            return new PdfPCell(new Phrase(text ?? "", font ?? F_TD))
            {
                BackgroundColor = bg,
                BorderColor = BORDER,
                HorizontalAlignment = align,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Colspan = colspan,
                PaddingTop = 4,
                PaddingBottom = 5,
                PaddingLeft = 4,
                PaddingRight = 4
            };
        }

        private void AddSubTotalRow(PdfPTable t, int labelColspan, decimal value, int trailingEmpty)
        {
            t.AddCell(TD("Sub Total", LIGHT_BLUE, Element.ALIGN_RIGHT, F_TD_BOLD, labelColspan));
            t.AddCell(TD(Money(value), LIGHT_BLUE, Element.ALIGN_RIGHT, F_TD_BOLD));
            for (int i = 0; i < trailingEmpty; i++)
                t.AddCell(TD("", LIGHT_BLUE));
        }

        private void AddTotalLine(PdfPTable t, string label, decimal value, Font valueFont = null, BaseColor bg = null)
        {
            t.AddCell(new PdfPCell(new Phrase(label, F_TOTAL_LABEL))
            {
                BackgroundColor = bg ?? WHITE,
                BorderColor = BORDER,
                Padding = 5
            });
            t.AddCell(new PdfPCell(new Phrase(Money(value), valueFont ?? F_TOTAL_VALUE))
            {
                BackgroundColor = bg ?? WHITE,
                BorderColor = BORDER,
                Padding = 5,
                HorizontalAlignment = Element.ALIGN_RIGHT
            });
        }

        private PdfPCell SignCell(string title, string name)
        {
            var p = new Phrase();
            p.Add(new Chunk("\n\n\n", F_TD));
            p.Add(new Chunk("______________________________\n", F_TD));
            p.Add(new Chunk(title + "\n", F_SIGN));
            p.Add(new Chunk(IsEmpty(name) ? " " : $"({name})", F_NOTE));

            return new PdfPCell(p)
            {
                Border = Rectangle.BOX,
                BorderColor = BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                MinimumHeight = 75,
                PaddingBottom = 8
            };
        }

        private void AddSpacing(Document doc, float height)
        {
            var t = new PdfPTable(1) { WidthPercentage = 100 };
            t.AddCell(new PdfPCell(new Phrase(" ")) { Border = Rectangle.NO_BORDER, FixedHeight = height, Padding = 0 });
            doc.Add(t);
        }

        // ============ FORMAT HELPERS ============

        private static BaseColor Row(int i) => i % 2 == 1 ? LIGHT_BLUE_ALT : WHITE;
        private static bool IsEmpty(string s) => string.IsNullOrWhiteSpace(s);
        private static string Dash(string s) => IsEmpty(s) ? "-" : s.Trim();
        private static string Money(decimal? v) => (v ?? 0).ToString("N2", IN);
        private static string Num(decimal? v) => (v ?? 0).ToString("0.##", CultureInfo.InvariantCulture);

        private static string FormatDate(DateTime? d)
        {
            if (!d.HasValue || d.Value == DateTime.MinValue) return "-";
            return d.Value.ToString("dd-MM-yyyy");
        }
    }

    internal class JobCardPageEvent : PdfPageEventHelper
    {
        private readonly string _jobCardNo;
        private readonly Font _font;
        private readonly BaseColor _line;

        public JobCardPageEvent(string jobCardNo, Font font, BaseColor line)
        {
            _jobCardNo = jobCardNo ?? "";
            _font = font;
            _line = line;
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            var cb = writer.DirectContent;
            float left = document.LeftMargin;
            float right = document.PageSize.Width - document.RightMargin;
            float y = 16;

            cb.SetColorStroke(_line);
            cb.SetLineWidth(0.5f);
            cb.MoveTo(left, y + 10);
            cb.LineTo(right, y + 10);
            cb.Stroke();

            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT,
                new Phrase($"Job Card No: {_jobCardNo}", _font), left, y, 0);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER,
                new Phrase("This is a computer generated job card.", _font), document.PageSize.Width / 2, y, 0);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_RIGHT,
                new Phrase($"Page {writer.PageNumber}", _font), right, y, 0);
        }
    }
}