using FirebaseAdmin.Messaging;
using GromaxMobileApis.Interfaces;
using GromaxMobileApis.Models;
using GromaxMobileApis.Models.Services;
using GromaxMobileApis.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace GromaxMobileApis.Utilities
{
    public static class UtilityFunctions
    {


        public static DataTable ConvertListToDataTableV1(List<InstallationImageData> modellist)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("InstallationMasterId");
            dt.Columns.Add("ImgUrl");
            dt.Columns.Add("Latitude");
            dt.Columns.Add("Longitude");
            dt.Columns.Add("Address");

            foreach (var i in modellist)
            {
                dt.Rows.Add(i.InstallationMasterId, i.ImgUrl, i.Latitude, i.Longitude, i.Address);
            }
            return dt;
        }
        public static DataTable ConvertStockExcelToDataTable(IFormFile file)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("DealerCode");
            dt.Columns.Add("BillingDate");
            dt.Columns.Add("TractorSrNumber");
            dt.Columns.Add("Model");
            dt.Columns.Add("DriveType");
            dt.Columns.Add("Colour");
            dt.Columns.Add("ProductDetails");
            dt.Columns.Add("TransferBy");
            dt.Columns.Add("ChassisNo");
            dt.Columns.Add("MfgDate");
            dt.Columns.Add("EngineNo");
            dt.Columns.Add("HpCategory");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);   // ✅ Copy IFormFile into MemoryStream
                stream.Position = 0;   // reset position

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int totalColumns = worksheet.Dimension.End.Column;
                    int totalRows = worksheet.Dimension.End.Row;

                    // Start reading from row 2 (skip headers)
                    for (int rowNum = 2; rowNum <= totalRows; rowNum++)
                    {
                        DataRow row = dt.NewRow();
                        for (int col = 1; col <= totalColumns && col <= dt.Columns.Count; col++)
                        {
                            string cellValue = worksheet.Cells[rowNum, col].Text?.Trim();
                            row[col - 1] = cellValue;
                        }
                        dt.Rows.Add(row);
                    }
                }
            }

            return dt;
        }


        public static DataTable ConvertStockExcelToDataTablev1(IFormFile file)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ChassiNo");
            dt.Columns.Add("BillingDate");
            dt.Columns.Add("ModelCode");
            dt.Columns.Add("InvoiceNo");
            dt.Columns.Add("InvValue");
            dt.Columns.Add("StockLocated");


            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);   // ✅ Copy IFormFile into MemoryStream
                stream.Position = 0;   // reset position

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int totalColumns = worksheet.Dimension.End.Column;
                    int totalRows = worksheet.Dimension.End.Row;

                    // Start reading from row 2 (skip headers)
                    for (int rowNum = 2; rowNum <= totalRows; rowNum++)
                    {
                        DataRow row = dt.NewRow();
                        for (int col = 1; col <= totalColumns && col <= dt.Columns.Count; col++)
                        {
                            string cellValue = worksheet.Cells[rowNum, col].Text?.Trim();
                            row[col - 1] = cellValue;
                        }
                        dt.Rows.Add(row);
                    }
                }
            }

            return dt;
        }


        public async static Task<string> SaveImage(Microsoft.AspNetCore.Http.IFormFile? image)
        {
            if (image == null || image.Length == 0)
                throw new Exception("No image uploaded.");
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Imageuploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            //string fileName = Guid.NewGuid().ToString("N").Substring(0, 5).ToUpper() + image.FileName;
            string filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return fileName;

        }

        public static DataTable ConvertCampaignExcelToDataTable(IFormFile file)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("DealerCode");
            dt.Columns.Add("CustomerName");
            dt.Columns.Add("CustomerMobileNo");
            dt.Columns.Add("StateName");
            dt.Columns.Add("StateCode");
            dt.Columns.Add("CustomerDistrictName");
            dt.Columns.Add("ModelCode");
            dt.Columns.Add("HpCategory");
            dt.Columns.Add("DriveType");
            dt.Columns.Add("ModelName");
            dt.Columns.Add("EnquirySource");
            dt.Columns.Add("EnquirySubSource");
            dt.Columns.Add("EnquiryGeneratedBy");
            dt.Columns.Add("EnquiryStatus");
            dt.Columns.Add("NextFollowUpDate");
            dt.Columns.Add("Remarks");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);   // ✅ Copy IFormFile into MemoryStream
                stream.Position = 0;   // reset position

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int totalColumns = worksheet.Dimension.End.Column;
                    int totalRows = worksheet.Dimension.End.Row;

                    // Start reading from row 2 (skip headers)
                    for (int rowNum = 2; rowNum <= totalRows; rowNum++)
                    {
                        DataRow row = dt.NewRow();
                        for (int col = 1; col <= totalColumns && col <= dt.Columns.Count; col++)
                        {
                            string cellValue = worksheet.Cells[rowNum, col].Text?.Trim();
                            row[col - 1] = cellValue;
                        }
                        dt.Rows.Add(row);
                    }
                }
            }

            return dt;
        }

        //public static async Task<string> SendNotificationAsync(string deviceToken, string title, string body)
        //{
        //    try
        //    {
        //        var message = new Message()
        //        {
        //            Token = deviceToken,
        //            Notification = new Notification
        //            {
        //                Title = title,
        //                Body = body
        //            },
        //            // Optional: Add custom data
        //            Data = new System.Collections.Generic.Dictionary<string, string>()
        //        {
        //            { "click_action", "FLUTTER_NOTIFICATION_CLICK" },
        //            { "message", "This is from .NET 5" }
        //        }
        //        };

        //        string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        //        return $"Notification sent successfully: {response}";
        //    }
        //    catch (Exception ex)
        //    {
        //        return $"Error sending notification: {ex.Message}";
        //    }
        //}


        public static async Task<string> SendNotificationAsync(string deviceToken, string open, string overdue, string seven)
        {
            try
            {
                TimeSpan currentTime = DateTime.Now.TimeOfDay;
                var title = "";
                var body = "";
                if (currentTime > TimeSpan.Parse("08:30:00") && currentTime < TimeSpan.Parse("09:30:00"))
                {
                    title = "Gromax";
                    body = $"🚨 {overdue.ToString()} enquiries overdue!\r\nFollow-up now to avoid losing potential clients.";
                }
                else if (currentTime > TimeSpan.Parse("10:30:00") && currentTime < TimeSpan.Parse("11:30:00"))
                {
                    title = "Gromax";
                    body = $"💬 You’ve got {open.ToString()} enquiries pending for follow-up!\r\nReach out and keep the momentum going.";
                }
                else if (currentTime > TimeSpan.Parse("11:30:00") && currentTime < TimeSpan.Parse("12:30:00"))
                {
                    title = "Gromax";
                    body = $"🔥 You’ve got {seven.ToString()} enquiries pending for delivery in 7 days!\r\nClose them fast and boost your conversions.";
                }
                else
                {
                    return "";
                }
                var message = new Message()
                {
                    Token = deviceToken,

                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    },

                    Data = new Dictionary<string, string>()
            {
                { "click_action", "FLUTTER_NOTIFICATION_CLICK" },
                { "message", "This is from .NET 5" }
            },

                    // ✅ iOS specific configuration
                    Apns = new ApnsConfig
                    {
                        Aps = new Aps
                        {
                            Alert = new ApsAlert
                            {
                                Title = title,
                                Body = body
                            },
                            Badge = 1,
                            Sound = "default"
                        },
                        Headers = new Dictionary<string, string>()
                {
                    // Set "priority" for instant delivery
                    { "apns-priority", "10" }
                }
                    },

                    // ✅ Android specific configuration (optional)
                    Android = new AndroidConfig
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification
                        {
                            Sound = "default",
                            ClickAction = "FLUTTER_NOTIFICATION_CLICK"
                        }
                    }
                };

                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return $"Notification sent successfully: {response}";
            }
            catch (Exception ex)
            {
                return $"Error sending notification: {ex.Message}";
            }
        }

        public static DataTable BDRCModelToConvertToDataTable(List<BDRCList> list)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("UploadId", typeof(int));
            dt.Columns.Add("Month", typeof(int));
            dt.Columns.Add("Year", typeof(int));
            dt.Columns.Add("StateName", typeof(string));
            dt.Columns.Add("DealerCode", typeof(string));
            dt.Columns.Add("DealerName", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("BatchId", typeof(int));

            dt.Columns.Add("BillPlan_W1", typeof(int));
            dt.Columns.Add("BillPlan_W2", typeof(int));
            dt.Columns.Add("BillPlan_W3", typeof(int));
            dt.Columns.Add("BillPlan_W4", typeof(int));
            dt.Columns.Add("BillPlan_W5", typeof(int));

            dt.Columns.Add("DelPlan_W1", typeof(int));
            dt.Columns.Add("DelPlan_W2", typeof(int));
            dt.Columns.Add("DelPlan_W3", typeof(int));
            dt.Columns.Add("DelPlan_W4", typeof(int));
            dt.Columns.Add("DelPlan_W5", typeof(int));

            dt.Columns.Add("RetPlan_W1", typeof(int));
            dt.Columns.Add("RetPlan_W2", typeof(int));
            dt.Columns.Add("RetPlan_W3", typeof(int));
            dt.Columns.Add("RetPlan_W4", typeof(int));
            dt.Columns.Add("RetPlan_W5", typeof(int));

            dt.Columns.Add("CollPlan_W1", typeof(int));
            dt.Columns.Add("CollPlan_W2", typeof(int));
            dt.Columns.Add("CollPlan_W3", typeof(int));
            dt.Columns.Add("CollPlan_W4", typeof(int));
            dt.Columns.Add("CollPlan_W5", typeof(int));

            dt.Columns.Add("BGPlan_W1", typeof(int));
            dt.Columns.Add("BGPlan_W2", typeof(int));
            dt.Columns.Add("BGPlan_W3", typeof(int));
            dt.Columns.Add("BGPlan_W4", typeof(int));
            dt.Columns.Add("BGPlan_W5", typeof(int));
            dt.Columns.Add("IsEdit", typeof(string));

            foreach (var item in list)
            {
                dt.Rows.Add(
                    item.UploadId,
                    item.Month,
                    item.Year,
                    item.StateName,
                    item.DealerCode,
                    item.DealerName,
                    item.Status,
                    item.BatchId,

                    item.BillPlan_W1,
                    item.BillPlan_W2,
                    item.BillPlan_W3,
                    item.BillPlan_W4,
                    item.BillPlan_W5,

                    item.DelPlan_W1,
                    item.DelPlan_W2,
                    item.DelPlan_W3,
                    item.DelPlan_W4,
                    item.DelPlan_W5,

                    item.RetPlan_W1,
                    item.RetPlan_W2,
                    item.RetPlan_W3,
                    item.RetPlan_W4,
                    item.RetPlan_W5,

                    item.CollPlan_W1,
                    item.CollPlan_W2,
                    item.CollPlan_W3,
                    item.CollPlan_W4,
                    item.CollPlan_W5,

                    item.BGPlan_W1,
                    item.BGPlan_W2,
                    item.BGPlan_W3,
                    item.BGPlan_W4,
                    item.BGPlan_W5,
                    item.IsEdit
                );
            }

            return dt;
        }


        public static DataTable ForecastModelToConvertToDataTable(List<ForecastList> list)
        {
            DataTable dt = new DataTable();


            dt.Columns.Add("UploadId", typeof(int));
            dt.Columns.Add("Month", typeof(int));
            dt.Columns.Add("Year", typeof(int));
            dt.Columns.Add("StateName", typeof(string));
            dt.Columns.Add("BatchId", typeof(int));

            dt.Columns.Add("ModelCode", typeof(string));
            dt.Columns.Add("ModelName", typeof(string));

            dt.Columns.Add("W1_BillingPlan", typeof(int));
            dt.Columns.Add("W2_BillingPlan", typeof(int));
            dt.Columns.Add("W3_BillingPlan", typeof(int));
            dt.Columns.Add("W4_BillingPlan", typeof(int));
            dt.Columns.Add("W5_BillingPlan", typeof(int));

            dt.Columns.Add("Total_BillingPlan", typeof(int));

            dt.Columns.Add("IsEdit", typeof(string));

            foreach (var item in list)
            {
                dt.Rows.Add(
                    item.UploadId,
                    item.Month,
                    item.Year,
                    item.StateName,
                    item.BatchId,
                    item.ModelCode,
                    item.ModelName,
                    item.W1_BillingPlan,
                    item.W2_BillingPlan,
                    item.W3_BillingPlan,
                    item.W4_BillingPlan,
                    item.W5_BillingPlan,
                    item.Total_BillingPlan,
                    item.IsEdit
                );
            }

            return dt;

        }


        public static DataTable ConvertStockExcelToDataTablev2(IFormFile file)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("DealerCode", typeof(string));
            dt.Columns.Add("BillDate", typeof(DateTime));
            dt.Columns.Add("InvNo", typeof(string));
            dt.Columns.Add("ModelCode", typeof(string));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("BillParty", typeof(string));
            dt.Columns.Add("Qty", typeof(int));
            dt.Columns.Add("ChassisNo", typeof(string));
            dt.Columns.Add("From", typeof(string));
            dt.Columns.Add("Destination", typeof(string));
            dt.Columns.Add("InvDate", typeof(DateTime));
            dt.Columns.Add("InvValue", typeof(double));
            dt.Columns.Add("StateName", typeof(string));


            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int totalColumns = worksheet.Dimension.End.Column;
                    int totalRows = worksheet.Dimension.End.Row;

                    // Start reading from row 2 (skip headers)
                    for (int rowNum = 2; rowNum <= totalRows; rowNum++)
                    {
                        DataRow row = dt.NewRow();
                        for (int col = 1; col <= totalColumns && col <= dt.Columns.Count; col++)
                        {
                            string cellValue = worksheet.Cells[rowNum, col].Text?.Trim();
                            Type columnType = dt.Columns[col - 1].DataType;

                            try
                            {
                                if (cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString()))
                                {
                                    row[col - 1] = DBNull.Value;
                                }
                                else if (columnType == typeof(string))
                                {
                                    row[col - 1] = cellValue.ToString().Trim();
                                }
                                else if (columnType == typeof(int))
                                {
                                    row[col - 1] = Convert.ToInt32(cellValue);
                                }
                                else if (columnType == typeof(double))
                                {
                                    row[col - 1] = Convert.ToDouble(cellValue);
                                }
                                else if (columnType == typeof(DateTime))
                                {
                                    row[col - 1] = DateTime.FromOADate(Convert.ToDouble(cellValue));
                                }
                                else
                                {
                                    row[col - 1] = cellValue;
                                }
                            }
                            catch
                            {
                                // Agar conversion fail ho to null store karo
                                //row[col - 1] = DBNull.Value;
                                throw;
                            }

                            //if (columnType == typeof(DateTime))
                            //    row[col - 1] = DateTime.FromOADate(Convert.ToDouble(cellValue));
                            //else
                            //    row[col - 1] = cellValue;
                        }
                        dt.Rows.Add(row);
                    }
                }
            }

            return dt;
        }

        public static DataTable PricePositionModelToConvertToDataTable(List<UpdatePricePosition> model)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Id", typeof(Guid));
            dt.Columns.Add("StateName", typeof(string));
            dt.Columns.Add("HPRange", typeof(string));
            dt.Columns.Add("SubHPRange", typeof(string));
            dt.Columns.Add("Drive", typeof(string));
            dt.Columns.Add("Make", typeof(string));
            dt.Columns.Add("Model", typeof(string));

            dt.Columns.Add("BOM", typeof(string));
            dt.Columns.Add("NDP", typeof(long));
            dt.Columns.Add("ACC", typeof(long));
            dt.Columns.Add("Freight", typeof(long));
            dt.Columns.Add("DM", typeof(long));
            dt.Columns.Add("MOP", typeof(long));

            dt.Columns.Add("ProofUrl", typeof(string));
            dt.Columns.Add("AvgVolPerMonth", typeof(int));
            dt.Columns.Add("RcCopy", typeof(string));

            foreach (var item in model)
            {
                dt.Rows.Add(
                    item.Id,
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    item.ndp,
                    item.accessories,
                    item.freight,
                    item.dlrMargin,
                    item.mop,
                    "",
                    item.avgVolPerMonth,
                    ""
                );
            }

            return dt;

        }

        public static DataTable PricePositionModelToConvertToDataTablev1(List<UpdatePricePositionv1> model)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Id", typeof(Guid));
            dt.Columns.Add("StateName", typeof(string));
            dt.Columns.Add("HPRange", typeof(string));
            dt.Columns.Add("SubHPRange", typeof(string));
            dt.Columns.Add("Drive", typeof(string));
            dt.Columns.Add("Make", typeof(string));
            dt.Columns.Add("Model", typeof(string));

            dt.Columns.Add("BOM", typeof(string));
            dt.Columns.Add("NDP", typeof(long));
            dt.Columns.Add("ACC", typeof(long));
            dt.Columns.Add("Freight", typeof(long));
            dt.Columns.Add("DM", typeof(long));
            dt.Columns.Add("MOP", typeof(long));

            dt.Columns.Add("ProofUrl", typeof(string));
            dt.Columns.Add("AvgVolPerMonth", typeof(int));
            dt.Columns.Add("RcCopy", typeof(string));

            dt.Columns.Add("implementPrice", typeof(long));
            dt.Columns.Add("rtoInsurance", typeof(long));

            foreach (var item in model)
            {
                dt.Rows.Add(
                    item.Id,
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    item.ndp,
                    item.accessories,
                    item.freight,
                    item.dlrMargin,
                    item.mop,
                    "",
                    item.avgVolPerMonth,
                    "",
                    item.implementPrice,
                    item.rtoInsurance
                );
            }

            return dt;

        }

        public static DataTable PricePositionModelToConvertToDataTablev2(List<UpdatePricePositionv1> model)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Id", typeof(Guid));
            dt.Columns.Add("StateName", typeof(string));
            dt.Columns.Add("HPRange", typeof(string));
            dt.Columns.Add("SubHPRange", typeof(string));
            dt.Columns.Add("Drive", typeof(string));
            dt.Columns.Add("Make", typeof(string));
            dt.Columns.Add("Model", typeof(string));

            dt.Columns.Add("BOM", typeof(string));
            dt.Columns.Add("NDP", typeof(long));
            dt.Columns.Add("ACC", typeof(long));
            dt.Columns.Add("Freight", typeof(long));
            dt.Columns.Add("DM", typeof(long));
            dt.Columns.Add("MOP", typeof(long));

            dt.Columns.Add("ProofUrl", typeof(string));
            dt.Columns.Add("AvgVolPerMonth", typeof(int));
            dt.Columns.Add("RcCopy", typeof(string));

            dt.Columns.Add("implementPrice", typeof(long));
            dt.Columns.Add("rtoInsurance", typeof(long));
            dt.Columns.Add("offerPrice", typeof(long));

            foreach (var item in model)
            {
                dt.Rows.Add(
                    item.Id,
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    item.ndp,
                    item.accessories,
                    item.freight,
                    item.dlrMargin,
                    item.mop,
                    "",
                    item.avgVolPerMonth,
                    "",
                    item.implementPrice,
                    item.rtoInsurance,
                    item.offerPrice
                );
            }

            return dt;

        }

        public static DataTable ConvertListToDataTableV2(List<InstallationImageData> modellist)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("InstallationMasterId");
            dt.Columns.Add("ImgUrl");
            dt.Columns.Add("Latitude");
            dt.Columns.Add("Longitude");
            dt.Columns.Add("Address");
            dt.Columns.Add("TagName");

            foreach (var i in modellist)
            {
                dt.Rows.Add(i.InstallationMasterId, i.ImgUrl, i.Latitude, i.Longitude, i.Address, i.TagName);
            }
            return dt;
        }

        public static DataTable GetPdiInspectionItemsDataTable(List<PdiField> fields)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("section", typeof(string));
            dt.Columns.Add("status", typeof(string));
            dt.Columns.Add("remark", typeof(string));
            dt.Columns.Add("photo1", typeof(string));
            dt.Columns.Add("photo2", typeof(string));
            dt.Columns.Add("serialNumber", typeof(string));
            dt.Columns.Add("brand", typeof(string));

            if (fields != null)
            {
                foreach (var item in fields)
                {
                    dt.Rows.Add(
                        string.IsNullOrWhiteSpace(item.name) ? DBNull.Value : item.name,
                        string.IsNullOrWhiteSpace(item.section) ? DBNull.Value : item.section,
                        string.IsNullOrWhiteSpace(item.status) ? DBNull.Value : item.status,
                        string.IsNullOrWhiteSpace(item.remark) ? DBNull.Value : item.remark,
                        string.IsNullOrWhiteSpace(item.photo1) ? DBNull.Value : item.photo1,
                        string.IsNullOrWhiteSpace(item.photo2) ? DBNull.Value : item.photo2,
                        string.IsNullOrWhiteSpace(item.serial) ? DBNull.Value : item.serial,
                        string.IsNullOrWhiteSpace(item.brand) ? DBNull.Value : item.brand
                    );
                }
            }

            return dt;
        }


        public static DataTable GetNTIRInspectionItemsDataTable(List<NTIRField> fields)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(Guid));
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("section", typeof(string));
            dt.Columns.Add("status", typeof(string));
            dt.Columns.Add("remark", typeof(string));
            dt.Columns.Add("photo1", typeof(string));
            dt.Columns.Add("photo2", typeof(string));
            dt.Columns.Add("serialNumber", typeof(string));
            dt.Columns.Add("brand", typeof(string));

            if (fields != null)
            {
                foreach (var item in fields)
                {
                    dt.Rows.Add(
                        item.id.HasValue ? (object)item.id.Value : DBNull.Value,
                        string.IsNullOrWhiteSpace(item.name) ? DBNull.Value : item.name,
                        string.IsNullOrWhiteSpace(item.section) ? DBNull.Value : item.section,
                        string.IsNullOrWhiteSpace(item.status) ? DBNull.Value : item.status,
                        string.IsNullOrWhiteSpace(item.remark) ? DBNull.Value : item.remark,
                        string.IsNullOrWhiteSpace(item.photo1) ? DBNull.Value : item.photo1,
                        string.IsNullOrWhiteSpace(item.photo2) ? DBNull.Value : item.photo2,
                        string.IsNullOrWhiteSpace(item.serial) ? DBNull.Value : item.serial,
                        string.IsNullOrWhiteSpace(item.brand) ? DBNull.Value : item.brand
                    );
                }
            }

            return dt;
        }


        public static JobCardDataTables UpdateJobCardDataTables(JobCardMaster model)
        {
            JobCardDataTables tables = new JobCardDataTables();

            #region Complaint

            try
            {
                tables.Complaints = new DataTable();
                tables.Complaints.Columns.Add("Id", typeof(Guid));

                tables.Complaints.Columns.Add("Complaint", typeof(string));
                tables.Complaints.Columns.Add("ActionTaken", typeof(string));
                tables.Complaints.Columns.Add("Remark", typeof(string));

                if (model.Complaints != null)
                {
                    foreach (var item in model.Complaints)
                    {
                        tables.Complaints.Rows.Add(
                           Guid.TryParse(item.Id, out var complaintId) ? (object)complaintId : DBNull.Value,
                            item.Complaint,
                            item.ActionTaken,
                            item.Remark);
                    }
                }
            }
            catch
            {
                throw;
            }
            #endregion

            #region Missing Part

            try
            {
                tables.MissingParts = new DataTable();
                tables.MissingParts.Columns.Add("Id", typeof(Guid));

                tables.MissingParts.Columns.Add("PartDescription", typeof(string));

                if (model.MissingParts != null)
                {
                    foreach (var item in model.MissingParts)
                    {

                        tables.MissingParts.Rows.Add(Guid.TryParse(item.Id, out var missingPartsId) ? (object)missingPartsId : DBNull.Value, item.PartDescription);
                    }
                }
            }
            catch
            {
                throw;
            }

            #endregion

            #region Spare Part

            try
            {
                tables.SpareParts = new DataTable();
                tables.SpareParts.Columns.Add("Id", typeof(Guid));

                tables.SpareParts.Columns.Add("PartNumber", typeof(string));
                tables.SpareParts.Columns.Add("PartName", typeof(string));
                tables.SpareParts.Columns.Add("Qty", typeof(decimal));
                tables.SpareParts.Columns.Add("Labour", typeof(decimal));
                tables.SpareParts.Columns.Add("GstAmount", typeof(decimal));
                tables.SpareParts.Columns.Add("TotalPrice", typeof(decimal));
                tables.SpareParts.Columns.Add("Remark", typeof(string));

                if (model.SpareParts != null)
                {
                    foreach (var item in model.SpareParts)
                    {
                        tables.SpareParts.Rows.Add(
                            Guid.TryParse(item.Id, out var sparePartsId) ? (object)sparePartsId : DBNull.Value,
                            item.PartNumber,
                            item.PartName,
                            item.Qty,
                            item.Labour,
                            item.GstAmount,
                            item.TotalPrice,
                            item.Remark);
                    }
                }
            }
            catch
            {
                throw;
            }

            #endregion

            #region Local Part
            try
            {
                tables.LocalParts = new DataTable();
                tables.LocalParts.Columns.Add("Id", typeof(Guid));

                tables.LocalParts.Columns.Add("PartName", typeof(string));
                tables.LocalParts.Columns.Add("PartNumber", typeof(string));
                tables.LocalParts.Columns.Add("Qty", typeof(decimal));
                tables.LocalParts.Columns.Add("Price", typeof(decimal));
                tables.LocalParts.Columns.Add("GSTRate", typeof(decimal));
                tables.LocalParts.Columns.Add("Labour", typeof(decimal));
                tables.LocalParts.Columns.Add("Remark", typeof(string));

                if (model.LocalParts != null)
                {
                    foreach (var item in model.LocalParts)
                    {
                        tables.LocalParts.Rows.Add(
                            Guid.TryParse(item.Id, out var localPartsId) ? (object)localPartsId : DBNull.Value,
                            item.PartName,
                            item.PartNumber,
                            item.Qty,
                            item.Price,
                            item.GSTRate,
                            item.Labour,
                            item.Remark);
                    }
                }
            }
            catch
            {
                throw;
            }
            #endregion

            return tables;
        }



        public static JobCardDataTables CreateJobCardDataTables(JobCardMaster model)
        {
            JobCardDataTables tables = new JobCardDataTables();

            #region Complaint

            try
            {
                tables.Complaints = new DataTable();

                tables.Complaints.Columns.Add("Complaint", typeof(string));
                tables.Complaints.Columns.Add("ActionTaken", typeof(string));
                tables.Complaints.Columns.Add("Remark", typeof(string));

                if (model.Complaints != null)
                {
                    foreach (var item in model.Complaints)
                    {
                        tables.Complaints.Rows.Add(
                            item.Complaint,
                            item.ActionTaken,
                            item.Remark);
                    }
                }
            }
            catch
            {
                throw;
            }
            #endregion

            #region Missing Part

            try
            {
                tables.MissingParts = new DataTable();

                tables.MissingParts.Columns.Add("PartDescription", typeof(string));

                if (model.MissingParts != null)
                {
                    foreach (var item in model.MissingParts)
                    {

                        tables.MissingParts.Rows.Add(item.PartDescription);
                    }
                }
            }
            catch
            {
                throw;
            }

            #endregion

            #region Spare Part

            try
            {
                tables.SpareParts = new DataTable();

                tables.SpareParts.Columns.Add("PartNumber", typeof(string));
                tables.SpareParts.Columns.Add("PartName", typeof(string));
                tables.SpareParts.Columns.Add("Qty", typeof(decimal));
                tables.SpareParts.Columns.Add("Labour", typeof(decimal));
                tables.SpareParts.Columns.Add("GstAmount", typeof(decimal));
                tables.SpareParts.Columns.Add("TotalPrice", typeof(decimal));
                tables.SpareParts.Columns.Add("Remark", typeof(string));

                if (model.SpareParts != null)
                {
                    foreach (var item in model.SpareParts)
                    {
                        tables.SpareParts.Rows.Add(
                            item.PartNumber,
                            item.PartName,
                            item.Qty,
                            item.Labour,
                            item.GstAmount,
                            item.TotalPrice,
                            item.Remark);
                    }
                }
            }
            catch
            {
                throw;
            }

            #endregion

            #region Local Part
            try
            {
                tables.LocalParts = new DataTable();

                tables.LocalParts.Columns.Add("PartName", typeof(string));
                tables.LocalParts.Columns.Add("PartNumber", typeof(string));
                tables.LocalParts.Columns.Add("Qty", typeof(decimal));
                tables.LocalParts.Columns.Add("Price", typeof(decimal));
                tables.LocalParts.Columns.Add("GSTRate", typeof(decimal));
                tables.LocalParts.Columns.Add("Labour", typeof(decimal));
                tables.LocalParts.Columns.Add("Remark", typeof(string));

                if (model.LocalParts != null)
                {
                    foreach (var item in model.LocalParts)
                    {
                        tables.LocalParts.Rows.Add(
                            item.PartName,
                            item.PartNumber,
                            item.Qty,
                            item.Price,
                            item.GSTRate,
                            item.Labour,
                            item.Remark);
                    }
                }
            }
            catch
            {
                throw;
            }
            #endregion

            return tables;
        }


        public static DataTable ConvertIdsToDataTable(List<Guid> ids)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Id", typeof(Guid));

                foreach (Guid id in ids)
                {
                    dt.Rows.Add(id);
                }

                return dt;
            }
            catch
            {
                throw;
            }
        }
    }
}
