using GromaxMobileApis.Models.Services;
using GromaxMobileApis.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Xml.Linq;
using Document = iTextSharp.text.Document;
using Font = iTextSharp.text.Font;
using Rectangle = iTextSharp.text.Rectangle;

namespace GromaxMobileApis.Utilities
{
    /// <summary>
    /// ServiceInvoiceResponse ko bind karke GST invoice PDF generate karta hai (iTextSharp).
    /// Proper spacing between sections, aligned invoice meta, normal font sizes.
    /// Chassis service history 2nd page me tabular format me - SINGLE ROW PER CHASSIS
    /// </summary>
    public class ServiceInvoicePdfGenerator
    {
        private readonly getFileName _upload;

        public ServiceInvoicePdfGenerator(getFileName upload)
        {
            _upload = upload;
        }

        // Colors
        private static readonly BaseColor HEADER_BLUE = new BaseColor(49, 142, 194);
        private static readonly BaseColor HEADER_BLUE_DARK = new BaseColor(37, 110, 153);
        private static readonly BaseColor LIGHT_BLUE = new BaseColor(230, 240, 250);
        private static readonly BaseColor LIGHT_BLUE_ALT = new BaseColor(242, 248, 253);
        private static readonly BaseColor ACCENT_RED = new BaseColor(255, 77, 77);
        private static readonly BaseColor LIGHT_RED = new BaseColor(255, 241, 241);
        private static readonly BaseColor GRAY_TEXT = new BaseColor(90, 103, 116);
        private static readonly BaseColor BORDER = new BaseColor(199, 211, 222);
        private static readonly BaseColor INK = new BaseColor(31, 42, 55);
        private static readonly BaseColor WHITE = BaseColor.White;

        // Fonts
        private static readonly Font FontCompanyName = GetFont(FontFactory.HELVETICA_BOLD, 10, WHITE);
        private static readonly Font FontCenter = GetFont(FontFactory.HELVETICA, 9, INK);
        private static readonly Font FontGstBanner = GetFont(FontFactory.HELVETICA_BOLD, 9, ACCENT_RED);
        private static readonly Font FontSectionTitle = GetFont(FontFactory.HELVETICA_BOLD, 9, HEADER_BLUE_DARK);
        private static readonly Font FontLabel = GetFont(FontFactory.HELVETICA_BOLD, 9, INK);
        private static readonly Font FontLine = GetFont(FontFactory.HELVETICA, 8, INK);
        private static readonly Font FontLineBold = GetFont(FontFactory.HELVETICA_BOLD, 8, INK);
        private static readonly Font FontMetaLabel = GetFont(FontFactory.HELVETICA_BOLD, 8, INK);
        private static readonly Font FontMetaValue = GetFont(FontFactory.HELVETICA, 8, INK);
        private static readonly Font FontMetaHighlight = GetFont(FontFactory.HELVETICA_BOLD, 8, HEADER_BLUE_DARK);
        private static readonly Font FontOriginal = GetFont(FontFactory.HELVETICA_BOLD, 9, HEADER_BLUE_DARK);
        private static readonly Font FontTableHeader = GetFont(FontFactory.HELVETICA_BOLD, 7.5f, WHITE);
        private static readonly Font FontTableCell = GetFont(FontFactory.HELVETICA, 7.5f, INK);
        private static readonly Font FontTableCellBold = GetFont(FontFactory.HELVETICA_BOLD, 7.5f, HEADER_BLUE_DARK);
        private static readonly Font FontWords = GetFont(FontFactory.HELVETICA_BOLD, 8, GRAY_TEXT);
        private static readonly Font FontTotalsLabel = GetFont(FontFactory.HELVETICA, 9, INK);
        private static readonly Font FontTotalsValue = GetFont(FontFactory.HELVETICA_BOLD, 9, INK);
        private static readonly Font FontGrandLabel = GetFont(FontFactory.HELVETICA_BOLD, 9, HEADER_BLUE_DARK);
        private static readonly Font FontGrandValue = GetFont(FontFactory.HELVETICA_BOLD, 9, HEADER_BLUE_DARK);
        private static readonly Font FontNote = GetFont(FontFactory.HELVETICA, 9, new BaseColor(178, 48, 48));
        private static readonly Font FontDeclaration = GetFont(FontFactory.HELVETICA_OBLIQUE, 8, new BaseColor(136, 136, 136));

        /// <summary>
        /// Font loading - Try multiple options (Arial, Helvetica) with fallback
        /// </summary>
        private static Font GetFont(string fontName, float size, BaseColor color)
        {
            try
            {
                // Try Arial first (Windows standard)
                string arialPath = @"C:\Windows\Fonts\arial.ttf";
                if (File.Exists(arialPath))
                {
                    FontFactory.Register(arialPath, "Arial");
                    return FontFactory.GetFont("Arial", size, Font.NORMAL, color);
                }
            }
            catch { }

            try
            {
                // Try Aptos (Windows 11)
                string aptosFontPath = @"C:\Windows\Fonts\aptos.ttf";
                if (File.Exists(aptosFontPath))
                {
                    FontFactory.Register(aptosFontPath, "Aptos");
                    return FontFactory.GetFont("Aptos", size, Font.NORMAL, color);
                }
            }
            catch { }

            // Fallback: Helvetica
            return FontFactory.GetFont(fontName, size, color);
        }

        /// <summary>
        /// ServiceInvoiceResponse ko accept karke PDF generate aur save karta hai
        /// </summary>
        public async Task<string> GenerateAndSave(ServiceInvoiceResponse response, string fileName = null)
        {
            if (response?.Master == null)
                throw new Exception("Invoice data is required.");

            if (string.IsNullOrWhiteSpace(fileName))
                throw new Exception("File name is required.");

            string finalFileName = $"{fileName}-{response.Master.InvoiceNo?.Replace("/", "_")}";

            byte[] pdfBytes = await Generate(response);

            var stream = new MemoryStream(pdfBytes);
            var formFile = new FormFile(stream, 0, pdfBytes.Length, "file", finalFileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            string fullPath = await _upload._getFileNamev1(formFile, finalFileName);
            return fullPath;
        }

        /// <summary>
        /// ServiceInvoiceResponse ko in-memory PDF generate karke bytes return karta hai
        /// </summary>
        public async Task<byte[]> Generate(ServiceInvoiceResponse response)
        {
            if (response?.Master == null)
                throw new Exception("ServiceInvoiceResponse data is required.");

            var invoice = response.Master;
            var items = response.Items ?? new List<ServiceInvoiceItemModel>();
            var hoursData = response.Hours ?? new List<ChassisServiceHoursModel>();

            using (var ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 20, 20, 20, 20);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                writer.SetFullCompression();
                writer.CompressionLevel = 9;
                writer.PdfVersion = PdfWriter.VERSION_1_5;

                doc.Open();

                // PAGE 1: Invoice
                AddHeader(doc, invoice);
                AddBillToAndShipTo(doc, invoice);
                AddSpacing(doc, 8);
                AddInvoiceMeta(doc, invoice);
                AddSpacing(doc, 8);
                AddOriginalBanner(doc);
                AddSpacing(doc, 6);
                AddItemsTable(doc, invoice, items);
                AddSpacing(doc, 6);
                AddTotalsRows(doc, invoice);
                AddSpacing(doc, 6);
                AddNote(doc, invoice);
                AddSpacing(doc, 4);
                AddDeclaration(doc);

                // PAGE 2: Service History (agar hours data available ho)
                if (hoursData?.Count > 0)
                {
                    AddChassisReportPage(doc, hoursData);
                }

                doc.Close();
                writer.Close();

                return ms.ToArray();
            }
        }

        // ============ SPACING METHOD ============
        private void AddSpacing(Document doc, float height)
        {
            var table = new PdfPTable(1) { WidthPercentage = 100 };
            var cell = new PdfPCell(new Phrase(" "))
            {
                Border = Rectangle.NO_BORDER,
                FixedHeight = height,
                Padding = 0,
                PaddingTop = 0,
                PaddingBottom = 0
            };
            table.AddCell(cell);
            doc.Add(table);
        }

        // ============ SECTIONS ============

        private void AddHeader(Document doc, ServiceInvoiceModel inv)
        {
            var headerTable = new PdfPTable(1) { WidthPercentage = 100 };
            var headerCell = new PdfPCell(new Phrase(inv.CompanyName, FontCompanyName))
            {
                BackgroundColor = HEADER_BLUE,
                Border = Rectangle.BOX,
                BorderColor = HEADER_BLUE_DARK,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingTop = 8,
                PaddingBottom = 8
            };
            headerTable.AddCell(headerCell);
            doc.Add(headerTable);

            AddPlainCenterBox(doc, $"Authorised TSD : {inv.AuthorisedTsd}");
            AddPlainCenterBox(doc, inv.CompanyAddress);

            var gstBanner = new PdfPTable(1) { WidthPercentage = 100 };
            var gstCell = new PdfPCell(new Phrase("GST Invoice", FontGstBanner))
            {
                BackgroundColor = LIGHT_RED,
                Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER,
                BorderColor = BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingTop = 5,
                PaddingBottom = 5
            };
            gstBanner.AddCell(gstCell);
            doc.Add(gstBanner);
        }

        private void AddPlainCenterBox(Document doc, string text)
        {
            var table = new PdfPTable(1) { WidthPercentage = 100 };
            var cell = new PdfPCell(new Phrase(text, FontCenter))
            {
                Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER,
                BorderColor = BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingTop = 4,
                PaddingBottom = 4
            };
            table.AddCell(cell);
            doc.Add(table);
        }

        private void AddBillToAndShipTo(Document doc, ServiceInvoiceModel inv)
        {
            var mainTable = new PdfPTable(2) { WidthPercentage = 100, SpacingBefore = 0, SpacingAfter = 0 };
            mainTable.SetWidths(new float[] { 1f, 1f });

            // BILL TO PARTY (Left)
            var billToCell = new PdfPCell
            {
                BackgroundColor = LIGHT_BLUE_ALT,
                Border = Rectangle.BOX,
                BorderColor = BORDER,
                PaddingTop = 6,
                PaddingBottom = 6,
                PaddingLeft = 6,
                PaddingRight = 6
            };

            billToCell.AddElement(new Paragraph("Bill to Party", FontSectionTitle) { SpacingAfter = 4 });
            billToCell.AddElement(LabelValueParagraph("Name : ", inv.ConsigneeName));
            billToCell.AddElement(LabelValueParagraph("Address : ", inv.ConsigneeAddress));
            billToCell.AddElement(new Paragraph($"PIN CODE: {inv.ConsigneePinCode}", FontLine) { SpacingAfter = 2 });
            billToCell.AddElement(LabelValueParagraph("State : ", inv.ConsigneeState));
            billToCell.AddElement(LabelValueParagraph("State Code : ", inv.ConsigneeStateCode));
            billToCell.AddElement(LabelValueParagraph("GSTN No. : ", inv.ConsigneeGstn));
            billToCell.AddElement(new Paragraph($"Phone No : {inv.ConsigneePhone}", FontLine) { SpacingAfter = 2 });
            billToCell.AddElement(new Paragraph(
                $"Tax Reverse Charge : {inv.TaxPayableOnReverseCharge ?? "No"}",
                FontLine)
            { SpacingBefore = 2 });

            mainTable.AddCell(billToCell);

            // SHIP TO PARTY (Right)
            var shipToCell = new PdfPCell
            {
                BackgroundColor = LIGHT_BLUE_ALT,
                Border = Rectangle.BOX,
                BorderColor = BORDER,
                PaddingTop = 6,
                PaddingBottom = 6,
                PaddingLeft = 6,
                PaddingRight = 6
            };

            shipToCell.AddElement(new Paragraph("Ship to Party", FontSectionTitle) { SpacingAfter = 4 });
            shipToCell.AddElement(LabelValueParagraph("Name : ", inv.ConsigneeName));
            shipToCell.AddElement(LabelValueParagraph("Address : ", inv.ConsigneeAddress));
            shipToCell.AddElement(new Paragraph($"PIN CODE: {inv.ConsigneePinCode}", FontLine) { SpacingAfter = 2 });
            shipToCell.AddElement(LabelValueParagraph("State : ", inv.ConsigneeState));
            shipToCell.AddElement(LabelValueParagraph("State Code : ", inv.ConsigneeStateCode));
            shipToCell.AddElement(LabelValueParagraph("GSTN No. : ", inv.ConsigneeGstn));
            shipToCell.AddElement(new Paragraph($"Phone No : {inv.ConsigneePhone}", FontLine) { SpacingAfter = 2 });
            shipToCell.AddElement(new Paragraph(
                $"Tax Reverse Charge : {inv.TaxPayableOnReverseCharge ?? "No"}",
                FontLine)
            { SpacingBefore = 2 });

            mainTable.AddCell(shipToCell);
            doc.Add(mainTable);
        }

        private void AddInvoiceMeta(Document doc, ServiceInvoiceModel inv)
        {
            var table = new PdfPTable(4) { WidthPercentage = 100, SpacingBefore = 0, SpacingAfter = 0 };
            table.SetWidths(new float[] { 1f, 1f, 1f, 1f });

            // Row 1 - Labels (Light Blue)
            AddMetaLabelCell(table, "Invoice No :");
            AddMetaLabelCell(table, "Invoice Date :");
            AddMetaLabelCell(table, "Dealer Code :");
            AddMetaLabelCell(table, "AO Name :");

            // Row 2 - Values (White)
            AddMetaValueCell(table, inv.InvoiceNo ?? "");
            AddMetaValueCell(table, inv.InvoiceDate.ToString("dd/MM/yyyy"));
            AddMetaValueCell(table, inv.DealerCode ?? "");
            AddMetaValueCell(table, inv.AoName ?? "");

            // Row 3 - Labels (Light Blue)
            AddMetaLabelCell(table, "Dealer Location :");
            AddMetaLabelCell(table, "GST :");
            AddMetaLabelCell(table, "State Code :");
            AddMetaLabelCell(table, "Claim :");

            // Row 4 - Values (White)
            AddMetaValueCell(table, inv.DealerLocation ?? "");
            AddMetaValueCell(table, inv.GstHeader ?? "");
            AddMetaValueCell(table, Convert.ToString(inv.StateCode) ?? "");
            AddMetaValueCell(table, inv.Claims ?? "");

            doc.Add(table);
        }

        private void AddMetaLabelCell(PdfPTable table, string label)
        {
            var cell = new PdfPCell(new Phrase(label, FontMetaLabel))
            {
                BackgroundColor = LIGHT_BLUE,
                Border = Rectangle.BOX,
                BorderColor = BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingTop = 4,
                PaddingBottom = 4,
                PaddingLeft = 5,
                PaddingRight = 5,
                MinimumHeight = 16,
                UseAscender = true
            };
            table.AddCell(cell);
        }

        private void AddMetaValueCell(PdfPTable table, string value)
        {
            var cell = new PdfPCell(new Phrase(value, FontMetaHighlight))
            {
                BackgroundColor = WHITE,
                Border = Rectangle.BOX,
                BorderColor = BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingTop = 4,
                PaddingBottom = 4,
                PaddingLeft = 5,
                PaddingRight = 5,
                MinimumHeight = 16,
                UseAscender = true
            };
            table.AddCell(cell);
        }

        private Paragraph LabelValueParagraph(string label, string value)
        {
            var para = new Paragraph { SpacingAfter = 1 };
            para.Add(new Chunk(label, FontLineBold));
            para.Add(new Chunk(value ?? "", FontLine));
            return para;
        }

        private void AddOriginalBanner(Document doc)
        {
            var table = new PdfPTable(1) { WidthPercentage = 100 };
            var cell = new PdfPCell(new Phrase("Original for Recipient", FontOriginal))
            {
                BackgroundColor = LIGHT_BLUE,
                Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER,
                BorderColor = BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingTop = 4,
                PaddingBottom = 4
            };
            table.AddCell(cell);
            doc.Add(table);
        }

        private void AddItemsTable(Document doc, ServiceInvoiceModel inv, List<ServiceInvoiceItemModel> Items)
        {
            float[] widths = { 4, 16, 7, 5, 6, 8, 6, 6, 6, 6, 6, 6, 7 };
            var table = new PdfPTable(widths) { WidthPercentage = 100, SpacingBefore = 0, SpacingAfter = 0 };

            string[] headers =
            {
                "Sr No", "Item Name", "HSN Code", "Qty", "Rate", "Taxable Value",
                "CGST Rate", "CGST Amt.", "SGST Rate", "SGST Amt.",
                "IGST Rate", "IGST Amt.", "Amount"
            };

            foreach (var h in headers)
            {
                var cell = new PdfPCell(new Phrase(h, FontTableHeader))
                {
                    BackgroundColor = HEADER_BLUE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Padding = 3,
                    BorderColor = BORDER
                };
                table.AddCell(cell);
            }

            int idx = 0;
            int srNo = 1;
            foreach (var it in Items)
            {
                BaseColor rowBg = (idx % 2 == 1) ? LIGHT_BLUE_ALT : WHITE;

                AddCell(table, srNo.ToString(), rowBg, false);
                AddCell(table, it.ItemName, rowBg, true);
                AddCell(table, it.HSNCode, rowBg, false);
                AddCell(table, it.Qty.ToString("0.##"), rowBg, false);
                AddCell(table, it.Rate.ToString("N2"), rowBg, false);
                AddCell(table, it.TaxableValue.ToString("N2"), rowBg, false);
                AddCell(table, it.CGSTPercentage.ToString("0.##") + "%", rowBg, false);
                AddCell(table, it.CGSTAmt.ToString("N2"), rowBg, false);
                AddCell(table, it.SGSTPercentage.ToString("0.##") + "%", rowBg, false);
                AddCell(table, it.SGSTAmt.ToString("N2"), rowBg, false);
                AddCell(table, it.IGSTPercentage.ToString("0.##") + "%", rowBg, false);
                AddCell(table, it.IGSTAmt.ToString("N2"), rowBg, false);
                AddCell(table, it.Amount.ToString("N2"), rowBg, false);

                idx++;
                srNo++;
            }

            // Totals row
            for (int i = 0; i < 5; i++)
                table.AddCell(EmptyTotalCell());

            table.AddCell(BoldTotalCell(inv.TotalTaxableValue.ToString("N2")));
            table.AddCell(EmptyTotalCell());
            table.AddCell(BoldTotalCell(inv.TotalCGSTAmt.ToString("N2")));
            table.AddCell(EmptyTotalCell());
            table.AddCell(BoldTotalCell(inv.TotalSGSTAmt.ToString("N2")));
            table.AddCell(EmptyTotalCell());
            table.AddCell(BoldTotalCell(inv.TotalIGSTAmt.ToString("N2")));
            table.AddCell(BoldTotalCell(inv.GrandTotal.ToString("N2")));

            doc.Add(table);
        }

        private void AddCell(PdfPTable table, string text, BaseColor bg, bool left)
        {
            var cell = new PdfPCell(new Phrase(text, FontTableCell))
            {
                BackgroundColor = bg,
                HorizontalAlignment = left ? Element.ALIGN_LEFT : Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 3,
                BorderColor = BORDER
            };
            table.AddCell(cell);
        }

        private PdfPCell EmptyTotalCell()
        {
            return new PdfPCell(new Phrase(""))
            {
                BackgroundColor = LIGHT_BLUE,
                BorderColor = BORDER,
                Padding = 3
            };
        }

        private PdfPCell BoldTotalCell(string text)
        {
            return new PdfPCell(new Phrase(text, FontTableCellBold))
            {
                BackgroundColor = LIGHT_BLUE,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 3,
                BorderColor = BORDER
            };
        }

        private void AddTotalsRows(Document doc, ServiceInvoiceModel inv)
        {
            AddWordsTotalsRow(doc, NumberToWordsHelper.Convert(inv.TotalTaxableValue), "Total Taxable Value", inv.TotalTaxableValue.ToString("N2"), true);
            AddWordsTotalsRow(doc, NumberToWordsHelper.Convert(inv.TotalTaxAmt), "Total Tax", inv.TotalTaxAmt.ToString("N2"), true);
            AddWordsTotalsRow(doc, NumberToWordsHelper.Convert(inv.GrandTotal), "Grand Total", inv.GrandTotal.ToString("N2"), true);
        }

        private void AddWordsTotalsRow(Document doc, string words, string label, string value, bool grand)
        {
            var table = new PdfPTable(3) { WidthPercentage = 100, SpacingBefore = 0, SpacingAfter = 0 };
            table.SetWidths(new float[] { 2f, 1.3f, 1f });

            BaseColor bg = grand ? LIGHT_BLUE : LIGHT_BLUE_ALT;

            var wordsCell = new PdfPCell(new Phrase($"In words- {words}", FontWords))
            {
                Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER,
                BorderColor = BORDER,
                Padding = 5
            };
            table.AddCell(wordsCell);

            var labelCell = new PdfPCell(new Phrase(label, grand ? FontGrandLabel : FontTotalsLabel))
            {
                BackgroundColor = bg,
                Border = Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER,
                BorderColor = BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 5
            };
            table.AddCell(labelCell);

            var valueCell = new PdfPCell(new Phrase(value, grand ? FontGrandValue : FontTotalsValue))
            {
                BackgroundColor = bg,
                Border = Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER,
                BorderColor = BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 5
            };
            table.AddCell(valueCell);

            doc.Add(table);
        }

        private void AddNote(Document doc, ServiceInvoiceModel inv)
        {
            var table = new PdfPTable(1) { WidthPercentage = 100, SpacingBefore = 0, SpacingAfter = 0 };
            var cell = new PdfPCell(new Phrase($"Note : {inv.Note}", FontNote))
            {
                BackgroundColor = LIGHT_RED,
                Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER,
                BorderColor = BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            };
            table.AddCell(cell);
            doc.Add(table);
        }

        private void AddDeclaration(Document doc)
        {
            var table = new PdfPTable(1) { WidthPercentage = 100, SpacingBefore = 0, SpacingAfter = 0 };

            var declCell = new PdfPCell(new Phrase(
                "Declaration: We declare that this bill shows the actual price of the parts and labor described are true and correct.",
                FontDeclaration))
            {
                Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER,
                BorderColor = BORDER,
                Padding = 5
            };
            table.AddCell(declCell);

            var sigCell = new PdfPCell(new Phrase("Authorised Signatory : Dealer signature", FontLine))
            {
                Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER,
                BorderColor = BORDER,
                Padding = 5
            };
            table.AddCell(sigCell);

            doc.Add(table);
        }

        /// <summary>
        /// PAGE 2: Service History table - SINGLE ROW PER CHASSIS
        /// Each chassis in ONE row with Date/Hours for all 6 services
        /// </summary>
        private void AddChassisReportPage(Document doc, List<ChassisServiceHoursModel> hoursData)
        {
            doc.NewPage();

            AddHeader(doc, new ServiceInvoiceModel
            {
                CompanyName = "Service Report",
                AuthorisedTsd = "",
                CompanyAddress = ""
            });

            AddSpacing(doc, 8);

            // Title
            var titleTable = new PdfPTable(1) { WidthPercentage = 100 };
            var titleCell = new PdfPCell(new Phrase("Service History", FontSectionTitle))
            {
                BackgroundColor = LIGHT_BLUE,
                Border = Rectangle.BOX,
                BorderColor = BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 6
            };
            titleTable.AddCell(titleCell);
            doc.Add(titleTable);

            AddSpacing(doc, 4);

            if (hoursData?.Count > 0)
            {
                AddChassisServiceTable(doc, hoursData);
            }
            else
            {
                var emptyTable = new PdfPTable(1) { WidthPercentage = 100 };
                var emptyCell = new PdfPCell(new Phrase("No service data available", FontLine))
                {
                    Padding = 10,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                emptyTable.AddCell(emptyCell);
                doc.Add(emptyTable);
            }
        }

        /// <summary>
        /// Service History table - SINGLE ROW PER CHASSIS
        /// Columns: Chassis | 1st Service Date | 1st Service Hours | 2nd Service Date | 2nd Service Hours | ... | 6th Service Date | 6th Service Hours
        /// </summary>
        private void AddChassisServiceTable(Document doc, List<ChassisServiceHoursModel> hoursData)
        {
            // 1 (Chassis) + 6 services * 2 (Date + Hours) = 13 columns
            float[] widths = { 2.5f, 1.8f, 1.8f, 1.8f, 1.8f, 1.8f, 1.8f, 1.8f, 1.8f, 1.8f, 1.8f, 1.8f, 1.8f };
            var table = new PdfPTable(widths) { WidthPercentage = 100, SpacingBefore = 0, SpacingAfter = 0 };

            // ===== HEADER ROW 1: Service Numbers (spans 2 columns each) =====
            var chassisHeaderCell = new PdfPCell(new Phrase("Chassis Number", FontTableHeader))
            {
                BackgroundColor = HEADER_BLUE,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 3,
                BorderColor = BORDER,
                Colspan = 1
            };
            table.AddCell(chassisHeaderCell);

            string[] serviceLabels = { "1st Service", "2nd Service", "3rd Service", "4th Service", "5th Service", "6th Service" };
            foreach (var label in serviceLabels)
            {
                var cell = new PdfPCell(new Phrase(label, FontTableHeader))
                {
                    BackgroundColor = HEADER_BLUE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Padding = 3,
                    BorderColor = BORDER,
                    Colspan = 2
                };
                table.AddCell(cell);
            }

            // ===== HEADER ROW 2: Date / Hours =====
            var emptyCornerCell = new PdfPCell(new Phrase("", FontTableHeader))
            {
                BackgroundColor = HEADER_BLUE,
                BorderColor = BORDER,
                Padding = 3
            };
            table.AddCell(emptyCornerCell);

            for (int i = 0; i < 6; i++)
            {
                var dateCell = new PdfPCell(new Phrase("Date", FontTableHeader))
                {
                    BackgroundColor = HEADER_BLUE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Padding = 3,
                    BorderColor = BORDER
                };
                table.AddCell(dateCell);

                var hoursCell = new PdfPCell(new Phrase("Hours", FontTableHeader))
                {
                    BackgroundColor = HEADER_BLUE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Padding = 3,
                    BorderColor = BORDER
                };
                table.AddCell(hoursCell);
            }

            // ===== DATA ROWS - SINGLE ROW PER CHASSIS =====
            int idx = 0;
            foreach (var item in hoursData)
            {
                BaseColor rowBg = (idx % 2 == 1) ? LIGHT_BLUE_ALT : WHITE;

                // Chassis number
                AddCell(table, item.ChassiNumber ?? "", rowBg, false);

                // 1st Service: Date + Hours
                AddCell(table, item.FirstServiceDate?.ToString() ?? "", rowBg, false);
                AddCell(table, item.FirstServiceHours?.ToString() ?? "", rowBg, false);

                // 2nd Service: Date + Hours
                AddCell(table, item.SecondServiceDate?.ToString() ?? "", rowBg, false);
                AddCell(table, item.SecondServiceHours?.ToString() ?? "", rowBg, false);

                // 3rd Service: Date + Hours
                AddCell(table, item.ThirdServiceDate?.ToString() ?? "", rowBg, false);
                AddCell(table, item.ThirdServiceHours?.ToString() ?? "", rowBg, false);

                // 4th Service: Date + Hours
                AddCell(table, item.FourthServiceDate?.ToString() ?? "", rowBg, false);
                AddCell(table, item.FourthServiceHours?.ToString() ?? "", rowBg, false);

                // 5th Service: Date + Hours
                AddCell(table, item.FifthServiceDate?.ToString() ?? "", rowBg, false);
                AddCell(table, item.FifthServiceHours?.ToString() ?? "", rowBg, false);

                // 6th Service: Date + Hours
                AddCell(table, item.SixthServiceDate?.ToString() ?? "", rowBg, false);
                AddCell(table, item.SixthServiceHours?.ToString() ?? "", rowBg, false);

                idx++;
            }

            doc.Add(table);
        }
    }

    /// <summary>
    /// Simple number-to-words converter (Indian numbering: Lakh/Crore)
    /// </summary>
    internal static class NumberToWordsHelper
    {
        private static readonly string[] Ones =
        {
            "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
            "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
            "Seventeen", "Eighteen", "Nineteen"
        };

        private static readonly string[] Tens =
        {
            "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
        };

        public static string Convert(decimal amount)
        {
            long rupees = (long)Math.Floor(amount);
            int paisa = (int)Math.Round((amount - rupees) * 100);

            string result = ConvertWhole(rupees) + " Rupees";

            if (paisa > 0)
                result += " " + ConvertWhole(paisa) + " Paisa";

            return result + " Only";
        }

        private static string ConvertWhole(long number)
        {
            if (number == 0) return "Zero";

            string result = "";

            long crore = number / 10000000;
            number %= 10000000;
            long lakh = number / 100000;
            number %= 100000;
            long thousand = number / 1000;
            number %= 1000;
            long hundred = number / 100;
            number %= 100;

            if (crore > 0) result += ConvertTwoDigit((int)crore) + " Crore ";
            if (lakh > 0) result += ConvertTwoDigit((int)lakh) + " Lakh ";
            if (thousand > 0) result += ConvertTwoDigit((int)thousand) + " Thousand ";
            if (hundred > 0) result += Ones[hundred] + " Hundred ";
            if (number > 0) result += ConvertTwoDigit((int)number);

            return result.Trim();
        }

        private static string ConvertTwoDigit(int number)
        {
            if (number < 20) return Ones[number];
            return (Tens[number / 10] + " " + Ones[number % 10]).Trim();
        }
    }
}