using Azure;
using Google.Api.Gax;
using Google.Apis.Download;
using GromaxMobileApis.Interfaces;
using GromaxMobileApis.Models;
using GromaxMobileApis.Models.DealerMaster;
using GromaxMobileApis.Models.EmployeeMaster;
using GromaxMobileApis.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime.Workdays;
using OfficeOpenXml.Style.XmlAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static GromaxMobileApis.Utilities.WhatsappMessageSend;
using static System.Net.WebRequestMethods;

namespace GromaxMobileApis.Controllers
{
    [Authorize]
    //[Authorize(Roles =
    //RoleMaster.Dealer + "," +
    //RoleMaster.SH + "," +
    //RoleMaster.NSH + "," +
    //RoleMaster.AM + "," +
    //RoleMaster.TM)]
    //[Route("Api/[Controller]")]
    public class HomeController : ControllerBase
    {
        private IDatabaseService _db;
        private IDatabaseServicesweb dbweb;
        private ResponseClass _r;
        private IAzureStorageService azureStorageService;
        private getFileName _getFilename;
        public HomeController(IDatabaseService db, IDatabaseServicesweb dbweb, ResponseClass r, IAzureStorageService azureStorageService, getFileName getFilename)
        {
            _db = db;
            this.dbweb = dbweb;
            _r = r;
            this.azureStorageService = azureStorageService;
            _getFilename = getFilename;
        }

        #region MobileApplication
        [AllowAnonymous]
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.Other.GetVersion)]
        public async Task<IActionResult> Version(string platform)
        {
            try
            {
                List<VersionMasterDto> VersionList = await _db.VersionMaster(platform);
                return Ok(VersionList);
            }
            catch (Exception Ex)
            {
                return BadRequest(new { Message = "Something Wrong", Error = Ex.Message });
            }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.Other.GetSalesTarget)]
        public async Task<IActionResult> GetSalesTarget(string DateFilter)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string dealerCode = User.FindFirst("dealercode")?.Value;
                var SalesTarget = await _db.GetSalesTarget(dealerCode, DateFilter, LoginPosition, username);
                return Ok(SalesTarget);
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.GetLeadsFollowup)]
        public async Task<IActionResult> GetLeadsFollowUp()
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string dealerCode = User.FindFirst("dealercode")?.Value;
                var LeadFollowUp = await _db.GetLeadFollowUp(dealerCode, LoginPosition, username);
                return Ok(LeadFollowUp);
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.InstallationMaster.Get)]
        public async Task<IActionResult> GetInstallationList()
        {
            try
            {
                var LeadFollowUp = await _db.GetInstallationMaster();
                return Ok(LeadFollowUp);
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.Other.GetBanner)]
        public async Task<IActionResult> BannerUrl()
        {
            try
            {
                var Bannerurl = await _db.BannerUrl();
                return Ok(Bannerurl);
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.Insert)]
        public async Task<IActionResult> GenerateEnquiry([FromBody] SalesEnquiryMaster model)
        {
            try
            {
                string dealerCode = User.FindFirst("dealercode")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string mobileNumber = User.FindFirst("mobilenumber")?.Value;
                var result = await _db.GenerateSalesEnquiry(model, dealerCode, username, mobileNumber);
                if (result > 0)
                    return Ok("Inserted Successfully");
                return BadRequest("Something Error");
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// for testing on local
        /// <returns></returns>

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.Insertv1)]
        public async Task<IActionResult> GenerateEnquiryv1([FromBody] SalesEnquiryMaster model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.DealerCode) && string.IsNullOrEmpty(model.ProspectMobile))
                    return Ok("Failed");
                string dealerCode = User.FindFirst("dealercode")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string mobileNumber = User.FindFirst("mobilenumber")?.Value;
                //var result = await _db.GenerateSalesEnquiryv1(model, dealerCode, username, mobileNumber);
                var result = await _db.GenerateSalesEnquiryv1(model, dealerCode, username, mobileNumber, "app");

                if (result > 0)
                    return Ok("Inserted Successfully");
                return BadRequest("Something Error");
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.Get)]
        public async Task<IActionResult> GetSalesEnquiry([FromBody] SalesEnquiryMasterdto model)
        {
            try
            {
                string dealerCode = User.FindFirst("dealercode")?.Value;//not in use
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string mobileNumber = User.FindFirst("mobilenumber")?.Value;
                var result = await _db.GetSalesEnquiry(model, dealerCode, username, LoginPosition);
                return Ok(result);
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.GetPendingFollowUp)]
        public async Task<IActionResult> PendingSalesFollowupList([FromBody] SalesEnquiryMaster model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string dealerCode = User.FindFirst("dealercode")?.Value;
                //string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string mobileNumber = User.FindFirst("mobilenumber")?.Value;
                var result = await _db.GetPendingSalesFollowupList(model, mobileNumber, dealerCode, LoginPosition, username);
                return Ok(result);
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.GetPendingFollowUpv1)]
        public async Task<IActionResult> PendingSalesFollowupListv1([FromBody] SalesEnquiryMaster model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string dealerCode = User.FindFirst("dealercode")?.Value;
                //string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string mobileNumber = User.FindFirst("mobilenumber")?.Value;
                var result = await dbweb.GetPendingSalesFollowupList(model, mobileNumber, dealerCode, LoginPosition, username);
                return Ok(result);
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.GetSalesEnquiryOnPendingFollowUp)]
        public async Task<IActionResult> GetSalesEnquiryOnPendingFollowUp([FromBody] SalesEnquiryMasterdto model)
        {
            try
            {
                var result = await _db.GetSalesEnquiryOnPendingFollowUpdb(model);
                return Ok(new { Message = "Success", data = result });
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.HistoryEnquiry.Insert)]
        public async Task<IActionResult> AddHistoryEnquiry([FromBody] HistoryEnquiry model)
        {
            try
            {
                var PlatformType = User.FindFirst("PlatformType")?.Value;
                var result = await _db.InsertHistoryEnquiry(model, PlatformType);
                if (result > 0)
                    return Ok("Inserted Successfully");
                return Ok("Something Wrong");
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.ModelMaster.Get)]
        public async Task<IActionResult> Getmodelmaster()
        {
            try
            {
                var result = await _db.Getmodelmasterlist();
                return Ok(result);
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.HistoryEnquiry.Get)]
        public async Task<IActionResult> GetHistoryEnquiryBySalesEnquiryId([FromBody] HistoryEnquiry model)
        {
            try
            {
                var result = await _db.GetHistoryEnquiry(model);
                return Ok(result);
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.FinanceMaster.Insert)]
        public async Task<IActionResult> InsertFinanceMaster([FromBody] FinanceMaster model)
        {
            try
            {
                var result = await _db.InsertFinanceMaster(model);
                if (result > 0)
                    return Ok("Inserted Successfully");
                else
                    return BadRequest("something wrong");
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        /// <summary>
        /// for testion version api
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.FinanceMaster.Insertv1)]
        public async Task<IActionResult> InsertFinanceMasterv1([FromBody] FinanceMaster model)
        {
            try
            {
                var result = await _db.InsertFinanceMasterv1(model);
                if (result > 0)
                    return Ok("Inserted Successfully");
                else
                    return BadRequest("something wrong");
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.FinanceMaster.Insertv2)]
        public async Task<IActionResult> InsertFinanceMasterv2([FromBody] FinanceMaster model)
        {
            try
            {
                //var PlatformType = User.
                string PlatformType = User.FindFirst("PlatformType")?.Value;
                var result = await _db.InsertFinanceMasterv2(model, PlatformType);
                if (result > 0)
                    return Ok("Inserted Successfully");
                else
                    return BadRequest("something wrong");
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.FinanceMaster.Get)]
        public async Task<IActionResult> GetFinanceMaster([FromBody] FinanceMastersalesenqiddto model)
        {
            try
            {
                var result = await _db.GetFinanceMaster(model);
                return Ok(result);
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.updCustomerEnquiry)]
        public async Task<IActionResult> updsalesCustomerEnquiry([FromBody] SalesEnquiryMaster model)
        {
            try
            {
                var result = await _db.updtSalesCustomerEnquiry(model);
                if (result > 0)
                    return Ok("Inserted successfully");
                else
                    return BadRequest("Something Wrong");
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        /// <summary> for testing
        ///         /// </summary>
        /// <returns></returns>

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.updCustomerEnquiryv1)]
        public async Task<IActionResult> updsalesCustomerEnquiryv1([FromBody] SalesEnquiryMaster model)
        {
            try
            {
                var result = await _db.updtSalesCustomerEnquiryv1(model);
                if (result > 0)
                    return Ok("Inserted successfully");
                else
                    return BadRequest("Something Wrong");
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.updCustomerEnquiryv2)]
        public async Task<IActionResult> updsalesCustomerEnquiryv2([FromBody] SalesEnquiryMaster model)
        {
            try
            {
                string PlatformType = User.FindFirst("PlatformType")?.Value;
                var result = await _db.updtSalesCustomerEnquiryv2(model, PlatformType);
                if (result > 0)
                    return Ok("Inserted successfully");
                else
                    return BadRequest("Something Wrong");
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.updCustomerProfile)]
        public async Task<IActionResult> updsalesCustomerProfile([FromBody] SalesEnquiryMaster model)
        {
            try
            {
                string PlatformType = User.FindFirst("PlatformType")?.Value;
                var result = await _db.updtSalesCustomerProfile(model, PlatformType);
                if (result > 0)
                    return Ok("Inserted successfully");
                else
                    return BadRequest("Something Wrong");
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        //[HttpPost]
        //[Route(GromaxMobileApis.Utilities.ApiRoutes.InstallationMaster.InsertImg)]
        //public async Task<IActionResult> InsertInstallationImg([FromBody] List<InstallationImg> modellist)
        //{
        //    try
        //    {
        //        DataTable dt = GromaxMobileApis.Utilities.UtilityFunctions.ConvertListToDataTable(modellist);
        //        var result = await _db.InsertInstallationImg(dt);
        //        if (result > 0)
        //            return Ok("Inserted successfully");
        //        else
        //            return BadRequest("Something Wrong");

        //    }
        //    catch (Exception Ex)
        //    {

        //        return BadRequest(new { Message = Ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Route(GromaxMobileApis.Utilities.ApiRoutes.InstallationMaster.InsertImg)]
        //public async Task<IActionResult> InsertInstallationImg([FromForm] IFormCollection form)
        //{
        //    try
        //    {
        //        string installationId = form["InstallationMasterId"];
        //        string Latitude = form["Latitude"];
        //        string Longitude = form["Longitude"];
        //        string Address = form["Address"];
        //        string TagName = form["TagName"];
        //        if (string.IsNullOrEmpty(installationId))
        //            return BadRequest("Error.");

        //        var files = form.Files;

        //        if (files == null || files.Count == 0)
        //            return BadRequest("Error.");

        //        string baseUrl = "https://loadcrm.com/growmaxmobileapi/UploadedFiles/";
        //        //string rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        //        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
        //        //string folderPath = Path.Combine(rootPath, "UploadedFiles");
        //        if (!Directory.Exists(folderPath))
        //            Directory.CreateDirectory(folderPath);

        //        var modellist = new List<InstallationImageData>();

        //        foreach (var file in files)
        //        {
        //            if (file.Length > 0)
        //            {
        //                string fileName = Path.GetFileNameWithoutExtension(file.FileName).Trim().Replace(" ", "_") +
        //                                  "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") +
        //                                  Path.GetExtension(file.FileName);

        //                string savePath = Path.Combine(folderPath, fileName);
        //                using (var stream = new FileStream(savePath, FileMode.Create))
        //                {
        //                    await file.CopyToAsync(stream);
        //                }

        //                string imageUrl = baseUrl + fileName;
        //                modellist.Add(new InstallationImageData
        //                {
        //                    InstallationMasterId = installationId,
        //                    ImgUrl = imageUrl,
        //                    Latitude = Latitude,
        //                    Longitude = Longitude,
        //                    Address = Address,
        //                    TagName = TagName
        //                });
        //            }
        //        }

        //        DataTable dt = GromaxMobileApis.Utilities.UtilityFunctions.ConvertListToDataTableV1(modellist);
        //        var result = await _db.InsertInstallationImg(dt);

        //        return result > 0 ? Ok("Inserted successfully") : BadRequest("Insert failed.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Message = ex.Message });
        //    }
        //}

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.InstallationMaster.InsertImg)]
        public async Task<IActionResult> InsertInstallationImg([FromForm] IFormCollection form)
        {
            try
            {
                string installationId = form["InstallationMasterId"];
                string Latitude = form["Latitude"];
                string Longitude = form["Longitude"];
                string Address = form["Address"];
                string TagName = form["TagName"];
                if (string.IsNullOrEmpty(installationId))
                    return BadRequest("Error.");

                var files = form.Files;

                if (files == null || files.Count == 0)
                    return BadRequest("Error.");



                var modellist = new List<InstallationImageData>();
                string fileUrl = "";
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        string baseUrl = "https://loadinfotechdb.blob.core.windows.net/gromaxwebprod1/";
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName).Trim().Replace(" ", "_") +
                                          "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                                          Path.GetExtension(file.FileName);
                        using (var stream = file.OpenReadStream())
                        {
                            fileUrl = await azureStorageService.UploadAsync(stream, fileName, file.ContentType);


                        }
                        baseUrl = baseUrl + fileName;
                        modellist.Add(new InstallationImageData
                        {
                            InstallationMasterId = installationId,
                            ImgUrl = baseUrl,
                            Latitude = Latitude,
                            Longitude = Longitude,
                            Address = Address,
                            TagName = TagName
                        });
                    }
                }

                DataTable dt = GromaxMobileApis.Utilities.UtilityFunctions.ConvertListToDataTableV1(modellist);
                var result = await _db.InsertInstallationImg(dt);

                return result > 0 ? Ok("Inserted successfully") : BadRequest("Insert failed.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.ReturnRequestMaster.Insert)]
        public async Task<IActionResult> InsertReturnRequestmaster([FromBody] ReturnRequestMaster model)
        {
            try
            {
                var result = await _db.InsertReturnRequestMaster(model);
                if (result >= 0)
                    return Ok("Inserted Successfully");
                else
                    return Ok("Something Went Wrong");


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.ReturnRequestMaster.Get)]
        public async Task<IActionResult> GetReturnRequestmaster([FromBody] ReturnRequestMaster model)
        {
            try
            {
                var result = await _db.GetReturnRequestMaster(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.getSevenDaySalesEnquiryDelivery)]
        public async Task<IActionResult> getSevenDaySalesEnquiryDelivery()
        {
            try
            {
                string dealerCode = User.FindFirst("dealercode")?.Value;//not in use
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string mobileNumber = User.FindFirst("mobilenumber")?.Value;

                //string dealerCode = User.FindFirst("dealercode")?.Value;
                ////string username = User.FindFirst(ClaimTypes.Name)?.Value;
                //string mobileNumber = User.FindFirst("mobilenumber")?.Value;
                var result = await _db.getSevenDaySalesEnquiryDelivery(mobileNumber, dealerCode, username, LoginPosition);

                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.getSevenDaySalesEnquiryDeliveryv1)]
        public async Task<IActionResult> getSevenDaySalesEnquiryDeliveryV1([FromBody] SuperHotEnquiry model)
        {
            try
            {
                string dealerCode = User.FindFirst("dealercode")?.Value;//not in use
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string mobileNumber = User.FindFirst("mobilenumber")?.Value;

                var result = await _db.getSevenDaySalesEnquiryDeliveryV1(model, mobileNumber, dealerCode, username, LoginPosition);

                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StateDistrictTehsilMaster.Filter)]
        public async Task<IActionResult> GetCustomerAddressFilter(string Type, string Name, string code)
        {
            try
            {
                var result = await _db.GetCustomerAddressFilter(Type, Name, code);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.MasterClosureReason.GetClosureMaster)]
        public async Task<IActionResult> GetClosureMaster()
        {
            try
            {
                var result = await _db.GetClosureMasterdb();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.GetInventoryData)]
        public async Task<IActionResult> GetInventoryData([FromBody] StockMaster model)
        {
            try
            {
                var result = await _db.GetInventoryData(model);
                return Ok(new { message = "Success", data = result });
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.MarkStockSold)]
        public async Task<IActionResult> MarkStockSold([FromBody] MarkStockSoldModel model)
        {
            try
            {
                var result = await _db.MarkStockSolddb(model);
                if (result > 0)
                    return Ok(new { Message = "Success", Data = "" });
                else
                    return Ok(new { Message = "Failed", Data = "" });
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }

        }
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StateDistrictTehsilMaster.GetState)]
        public async Task<IActionResult> GetState(string DealerCode)
        {
            try
            {
                var result = await _db.GetStatedb(DealerCode);
                return Ok(new { Message = "Success", data = result });

            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.ReturnRequestMaster.Getv1)]
        public async Task<IActionResult> GetReturnRequestmasterv1([FromBody] GetReturnRequestMasterv1 model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.GetReturnRequestMasterv1(model, LoginPosition, username);
                return Ok(new { Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.ReturnRequestMaster.Generate)]
        public async Task<IActionResult> GenerateReturnRequest([FromForm] GenerateReturnRequestv1 model)
        {
            try
            {
                model.FileNamesurl = new List<string>();
                if (model.FileNames != null && model.FileNames.Count > 0)
                {
                    foreach (var file in model.FileNames)
                    {
                        var FileName = await GromaxMobileApis.Utilities.UtilityFunctions.SaveImage(file);
                        model.FileNamesurl.Add(FileName);
                    }
                }
                string PlatformType = User.FindFirst("PlatformType")?.Value;
                var result = await _db.GenerateReturnRequest(model, PlatformType);
                return Ok(new { Message = "Success", Data = result >= 1 ? "Inserted Successfully" : "Not Inserted" });
                //return Ok();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.ReturnRequestMaster.GetSalesDetailsForReturnRequest)]
        public async Task<IActionResult> GetSalesDetailsForReturnRequest([FromBody] GetReturnRequestMasterv1 model)
        {
            try
            {
                var result = await _db.GetSalesDetailsForReturnRequest(model);
                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.ReturnRequestMaster.ReturnRequestApproval)]
        public async Task<IActionResult> ReturnRequestApproval([FromBody] ReturnRequestApproval model)
        {
            try
            {
                string PlatformType = User.FindFirst("PlatformType")?.Value;
                var result = await _db.ReturnRequestApprovaldb(model, PlatformType);
                return Ok(new { Message = "Success", Data = result >= 1 ? "Status Updated" : "Status Not Updated" });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }


        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.ReturnRequestMaster.GetStatusHistory)]
        public async Task<IActionResult> GetStatusHistory([FromBody] ReturnRequestApproval model)
        {
            try
            {
                var result = await _db.GetStatusHistorydb(model);
                return Ok(new { Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.ReturnRequestMaster.ReturnRequestApprovalv1)]
        public async Task<IActionResult> ReturnRequestApprovalv1([FromBody] ReturnRequestApproval model)
        {
            try
            {
                //string PlatformType = User.FindFirst("PlatformType")?.Value;
                var result = await _db.ReturnRequestApprovaldbv1(model);
                return Ok(new { Message = "Success", Data = result >= 1 ? "Status Updated" : "Status Not Updated" });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }


        }
        #endregion

        #region webapisforMobile
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.GetSalesEnquiryMasterweb)]
        public async Task<IActionResult> GetSalesEnquiryMaster([FromBody] SalesEnquiryMasterFiltered model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(ms => ms.Value.Errors.Any()) // take only invalid ones
                        .Select(ms => ms.Key)               // just the field name
                        .ToList();

                    return BadRequest(new
                    {
                        Code = 400,
                        Message = _r.MissingParam,
                        MissingFields = errors
                    });
                }

                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.GetSalesEnquiryMaster(model, LoginPosition, username);
                //var result1 = await dbweb.GetLeads(model, LoginPosition, username);
                return Ok(new { Message = "Success", Data = result });
            }
            catch (Exception ex) { throw new Exception(ex.Message); }



        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.GetInventoryDataweb)]
        public async Task<IActionResult> GetInventoryDataweb([FromBody] StockFilterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(ms => ms.Value.Errors.Any()) // take only invalid ones
                        .Select(ms => ms.Key)               // just the field name
                        .ToList();

                    return BadRequest(new
                    {
                        Code = 400,
                        Message = _r.MissingParam,
                        MissingFields = errors
                    });
                }
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await dbweb.GetInventoryData(model, LoginPosition, username);
                return Ok(new { message = "Success", data = result });
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.BussinessPerformance.Get)]
        public async Task<IActionResult> GetBussinessPerformance([FromBody] BussinessPerformance model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(ms => ms.Value.Errors.Any()) // take only invalid ones
                        .Select(ms => ms.Key)               // just the field name
                        .ToList();

                    return BadRequest(new
                    {
                        Code = 400,
                        Message = _r.MissingParam,
                        MissingFields = errors
                    });
                }
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await dbweb.GetBussinessPerformance(model, LoginPosition, username);
                return Ok(new { message = "Success", data = result });
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }


        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.BussinessPerformance.Getv1)]
        public async Task<IActionResult> GetBussinessPerformancev1([FromBody] BussinessPerformance model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(ms => ms.Value.Errors.Any()) // take only invalid ones
                        .Select(ms => ms.Key)               // just the field name
                        .ToList();

                    return BadRequest(new
                    {
                        Code = 400,
                        Message = _r.MissingParam,
                        MissingFields = errors
                    });
                }
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await dbweb.GetBussinessPerformanceWeb(model, LoginPosition, username);
                return Ok(new { message = "Success", data = result });
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }


        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.PossitionMaster.PossitionFilter)]
        public async Task<IActionResult> PossitionFilter([FromBody] PositionFilter model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var data = await dbweb.GetPossitionFilter(model, LoginPosition, username);
                return Ok(new { message = "Success", data = data });
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.GetSalesEnquiryMasterPagination)]
        public async Task<IActionResult> GetSalesEnquiryMasterpagination([FromBody] SalesEnquiryMasterFiltered model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(ms => ms.Value.Errors.Any()) // take only invalid ones
                        .Select(ms => ms.Key)               // just the field name
                        .ToList();

                    return BadRequest(new
                    {
                        Code = 400,
                        Message = _r.MissingParam,
                        MissingFields = errors
                    });
                }

                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.GetSalesEnquiryMasterPagination(model, LoginPosition, username);
                //var result1 = await dbweb.GetLeads(model, LoginPosition, username);
                return Ok(new { Message = "Success", Data = result });
            }
            catch (Exception ex) { throw new Exception(ex.Message); }



        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.GetInventoryDataPagination)]
        public async Task<IActionResult> GetInventoryDataPagination([FromBody] StockFilterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(ms => ms.Value.Errors.Any()) // take only invalid ones
                        .Select(ms => ms.Key)               // just the field name
                        .ToList();

                    return BadRequest(new
                    {
                        Code = 400,
                        Message = _r.MissingParam,
                        MissingFields = errors
                    });
                }
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await dbweb.GetInventoryDataWeb(model, LoginPosition, username);
                var firstRow = result.FirstOrDefault();
                var totalCount = firstRow?.TotalCount?.ToString();
                totalCount = string.IsNullOrEmpty(totalCount) ? "0" : totalCount;
                return Ok(new { message = "Success", data = result, TotalCount = totalCount });
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.GetInventoryDataPaginationv1)]
        public async Task<IActionResult> GetInventoryDataPaginationv1([FromBody] StockFilterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(ms => ms.Value.Errors.Any()) // take only invalid ones
                        .Select(ms => ms.Key)               // just the field name
                        .ToList();

                    return BadRequest(new
                    {
                        Code = 400,
                        Message = _r.MissingParam,
                        MissingFields = errors
                    });
                }
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await dbweb.GetInventoryDataWebv1(model, LoginPosition, username);
                var firstRow = result.FirstOrDefault();
                var totalCount = firstRow?.TotalCount?.ToString();
                totalCount = string.IsNullOrEmpty(totalCount) ? "0" : totalCount;
                return Ok(new { message = "Success", data = result, TotalCount = totalCount });
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.GetInventoryDatawebv1)]
        public async Task<IActionResult> GetInventoryDatawebv1([FromBody] StockFilterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(ms => ms.Value.Errors.Any()) // take only invalid ones
                        .Select(ms => ms.Key)               // just the field name
                        .ToList();

                    return BadRequest(new
                    {
                        Code = 400,
                        Message = _r.MissingParam,
                        MissingFields = errors
                    });
                }
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await dbweb.GetInventoryDatav1(model, LoginPosition, username);
                return Ok(new { message = "Success", data = result });
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }
        #endregion

        #region CommonApis
        //web
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.GetSalesEnquirySearchByMobileNumber)]
        public async Task<IActionResult> GetSalesEnquirySearchByMobileNumber([FromBody] SalesEnquirySearchdto model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.GetSalesEnquirySearchByMobileNumberdb(model, LoginPosition, username);
                return Ok(new { Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }
        //web
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.DownloadInventoryReport)]
        public async Task<IActionResult> DownloadInventoryReport([FromBody] StockFilterModel model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.DonwloadInventoryReportdb(model, LoginPosition, username);
                return Ok(new { Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }
        //web
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.SearchInventoryReport)]
        public async Task<IActionResult> SearchInventoryReport([FromBody] SearchInventoryReportDto model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.SearchInventoryReportdb(model, LoginPosition, username);
                return Ok(new { Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }
        //web
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.SearchAvailableInventoryReport)]
        public async Task<IActionResult> SearchAvailableInventoryReport([FromBody] SearchInventoryReportDto model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.SearchAvailableInventoryReportdb(model, LoginPosition, username);
                return Ok(new { Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }
        //web
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.DownloadAvailableInventoryReport)]
        public async Task<IActionResult> DownloadAvailableInventoryReport([FromBody] StockFilterModel model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.DonwloadAvailableInventoryReportdb(model, LoginPosition, username);
                return Ok(new { Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.InstallationMaster.Getv1)]
        public async Task<IActionResult> GetInstallationListv1([FromBody] InstallationMasterdto model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.GetInstallationMasterv1(model, LoginPosition, username);

                if (model.IsDownload == "Yes")
                {
                    var downloadList = result.Tables[0];
                    var json = JsonConvert.SerializeObject(new
                    {
                        InstallationList = downloadList,
                        Message = "Success"
                    });
                    return Content(json, "application/json");
                }
                else
                {
                    var json = JsonConvert.SerializeObject(new
                    {
                        InstallationCount = result.Tables[0],
                        InstallationList = result.Tables[1],
                        ImageUrlList = result.Tables[2],
                        Message = "Success"
                    });
                    return Content(json, "application/json");
                }
            }
            catch (Exception Ex)
            {
                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.MarkStockSoldv1)]
        public async Task<IActionResult> MarkStockSoldv1([FromBody] MarkStockSoldModel model)
        {
            try
            {
                var PlatformType = User.FindFirst("PlatformType")?.Value;
                var username = User.FindFirst("username")?.Value;
                var result = await _db.MarkStockSolddbv1(model, PlatformType, username);
                if (result > 0)
                    return Ok(new { Message = "Success", Data = "" });
                else
                    return Ok(new { Message = "Failed", Data = "" });
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.FinanceMaster.CheckFillFinanceData)]
        public async Task<IActionResult> CheckFillFinanceData([FromBody] SalesEnquiryMasterdto model)
        {
            try
            {
                var result = await _db.CheckFillFinanceDatadb(model);
                return Ok(new { Message = "Success", data = result });
                //if (result > 0)
                //    return Ok(new { Message = "Success", Data = "" });
                //else
                //    return Ok(new { Message = "Failed", Data = "" });
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesmanMaster.Get)]
        public async Task<IActionResult> GetSalesmanMasterlist([FromBody] SalesmanGetdto model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.GetSalemanListdb(model, LoginPosition, username);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
                //var table0 = result.Tables[0];
                //var table1 = result.Tables[1];
                //return Ok(new { Message = "Success",data = result});
                //var json = JsonConvert.SerializeObject(new
                //{
                //    InstallationCount = table0,
                //    InstallationList = table1,
                //    Message = "Success"
                //    //Department = table2
                //});

                // return Ok(new { Message = "Success", Header = table0, All = table1, Department = table2 });
                //return Ok(result);
                //return Content(json, "application/json");
            }
            catch (Exception Ex)
            {

                //return BadRequest(new { Message = Ex.Message });
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesmanMaster.Insert)]

        public async Task<IActionResult> InsertSalesman([FromBody] SalesmanMaster model)
        {
            try
            {
                string PlatformType = User.FindFirst("PlatformType")?.Value;
                var result = await _db.InsertSalesmandb(model, PlatformType);
                if (result > 0)
                    return Ok(ApiResponse<string>.Created());
                else
                    return Ok(ApiResponse<string>.Fail("Failed"));

            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail(ex.Message));
            }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.InstallationMaster.GetImages)]
        public async Task<IActionResult> GetInstallationImages([FromBody] InstallationImage model)
        {
            try
            {
                var result = await _db.GetInstallationImagesdb(model);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex) { return Ok(ApiResponse<string>.Fail(ex.Message)); }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesmanMaster.GetByDealerCode)]
        public async Task<IActionResult> GetSalesmanByDealerCode([FromBody] SalesmanMaster model)
        {
            try
            {
                var result = await _db.GetSalesmanByDealerCodedb(model);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex) { return Ok(ApiResponse<string>.Fail(ex.Message)); }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.BussinessPerformance.DownloadBussinessPerformace)]
        public async Task<IActionResult> DownloadBussinessPerformace([FromBody] BussinessPerformance model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(ms => ms.Value.Errors.Any()) // take only invalid ones
                        .Select(ms => ms.Key)               // just the field name
                        .ToList();

                    return BadRequest(new
                    {
                        Code = 400,
                        Message = _r.MissingParam,
                        MissingFields = errors
                    });
                }
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.DownloadBussinessPerformancedb(model, LoginPosition, username);
                return Ok(ApiResponse<BussinessPerformanceResult>.Success(result));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<BussinessPerformanceResult>.Fail(ex.Message));
            }


        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.GetSalesEnqForFollowByMobileNo)]
        public async Task<IActionResult> GetSalesEnquiryForFollowUpSearchByMobileNumber([FromBody] SalesEnquirySearchdto model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.GetSalesEnquiryForFollowSearchByMobileNumberdb(model, LoginPosition, username);
                return Ok(new { Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.DownloadGetPendingFillowupList)]//PendingSalesFollowupList
        public async Task<IActionResult> DownloadPendingSalesFollowupList([FromBody] EnquiryFilterRequest model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string dealerCode = User.FindFirst("dealercode")?.Value;
                //string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string mobileNumber = User.FindFirst("mobilenumber")?.Value;
                var result = await _db.DownloadGetPendingSalesFollowupListdb(model, mobileNumber, dealerCode, LoginPosition, username);
                //int totalCount = result != null && result.Count() > 0 ? result?.Select(r => r.TotalCount).FirstOrDefault() : 0;
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
                //return BadRequest(new { Message = Ex.Message });
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.Other.GetUserDetail)]
        public async Task<IActionResult> GetUserDetail()
        {
            try
            {
                var MobileNo = User.FindFirst("mobilenumber")?.Value;
                var result = await _db.GetUserDetaildb(MobileNo);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.CustomerExists)]
        public async Task<IActionResult> CustomerExistsInSalesEnquiry([FromBody] SalesEnquiryMasterdto model)
        {
            try
            {
                var result = await _db.CustomerExistsdb(model);
                if (result == 1)
                    return Ok(ApiResponse<string>.Success(default, "Exist"));
                else
                    return Ok(ApiResponse<string>.Success(default, "Not Exist"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.FirebaseNotification.FirebaseNotificationApi)]
        public async Task<IActionResult> Send()
        {
            var result = "";
            var FcmTokenList = await _db.GetFcmTokendb();

            //DateTime currentTime = DateTime.Now;
            //string formattedTime = currentTime.ToString("HH:mm");

            //int TotToken = FcmTokenList.Count();
            foreach (var Token in FcmTokenList)
            {
                //if (Convert.ToString(Token.FCMToken) ==
                //    "dqvAmcP3RQGpgBpH7SJLvt:APA91bGpPpGJl2Qd_4taami1nHYCgD7fX5GiNwA3geD0RVtW0ftKibsSH90fe_RLTtqR5DFxsQn-Ey0hM7bCoj7wvzbTO0njvgfyjLX8h-KN5zEo0L1-ZLM")
                //{
                string token = Convert.ToString(Token.FCMToken);
                string TotalOpen = Convert.ToString(Token.TotalOpen);
                string TotalOverDue = Convert.ToString(Token.TotalOverDue);
                string TotalSevenDayDelivery = Convert.ToString(Token.TotalSevenDayDelivery);
                //var result = await GromaxMobileApis.Utilities.UtilityFunctions.SendNotificationAsync(token);
                result = await GromaxMobileApis.Utilities.UtilityFunctions.SendNotificationAsync(
                    token,
                    TotalOpen, TotalOverDue, TotalSevenDayDelivery);
                if (result == "")
                {
                    break;
                }
                //}

            }
            ////var result = await GromaxMobileApis.Utilities.UtilityFunctions.SendNotificationAsync(request.DeviceToken, request.Title, request.Body);
            ////return Ok(new { message = result });
            ////return Ok(new { message = FcmTokenList });
            return Ok(new { message = result });
            //return Ok();

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.Other.UpdateFcmToken)]
        public async Task<IActionResult> UpdateFcmToken([FromBody] FcmTokendto model)
        {
            try
            {
                var result = await _db.UpdateFcmTokendb(model.MobileNumber, model.FCMToken);
                if (result > 0)
                    return Ok(ApiResponse<string>.Success(default));
                else
                    return Ok(ApiResponse<string>.Success(default, "Failed"));
            }
            catch (Exception ex) { return Ok(ApiResponse<string>.Fail("Error")); }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.DealerMaster.DealerListStatewise)]
        public async Task<IActionResult> DealerListStatewise([FromBody] DealerMasterDto model)
        {
            try
            {
                var result = await _db.DealerlistStatewisedb(model.StateCode);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.DlrToDlrStockTransfer)]
        public async Task<IActionResult> DlrToDlrStockTransfer([FromBody] StockMasterdto model)
        {
            try
            {
                var result = await _db.DlrToDlrStockTrfdb(model);
                if (result > 0)
                    return Ok(ApiResponse<string>.Success(default));
                else
                    return Ok(ApiResponse<string>.Success(default, "Failed"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }
        }
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.StockTrfApproval)]
        public async Task<IActionResult> StockTrfApproval([FromBody] StockTrfAprovalDto model)
        {
            try
            {
                var result = await _db.StockTrfApprovaldb(model);
                if (result > 0)
                    return Ok(ApiResponse<string>.Success(default));
                else
                    return Ok(ApiResponse<string>.Success(default, "Failed"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.GetStockTrfApproval)]
        public async Task<IActionResult> GetStockTrfApproval()
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.GetStockTrfApprovaldb(LoginPosition, username);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch { return Ok(ApiResponse<string>.Fail("Error")); }


        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.GetStockForStockTransfer)]
        public async Task<IActionResult> GetStockForStockTransfer([FromBody] GetStockForStkTrfdto model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.GetStockForStockTransferdb(model, LoginPosition, username);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));

            }
            catch { return Ok(ApiResponse<string>.Fail("Error")); }




        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesmanMaster.UpdateSalesManStatus)]
        public async Task<IActionResult> UpdateSalesManStatus([FromBody] SalesmanStatusdto model)
        {
            try
            {
                var result = await _db.UpdateSalesmanStatusdb(model);
                if (result > 0)
                    return Ok(ApiResponse<string>.Success(default));
                else
                    return Ok(ApiResponse<string>.Success(default, "Failed"));

            }
            catch { return Ok(ApiResponse<string>.Fail("Error")); }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.Retiledsales.NotRetailedList)]
        public async Task<IActionResult> NotRetailedList([FromBody] RetailedSalesFilter filter)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.GetNotRetailedSales(filter, LoginPosition, username);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }

        }
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.Retiledsales.UpdateRetailSale)]
        public async Task<IActionResult> UpdateRetailSale([FromBody] RetailSaleRequestv1 model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string PlatformType = User.FindFirst("PlatformType")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.UpdateRetailSaledb(model, username, PlatformType);
                if (result > 0)
                    return Ok(ApiResponse<string>.Success(default));
                else
                    return Ok(ApiResponse<string>.Fail("Error"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail(ex.Message));
            }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.GenerateEnquiryV2)]
        public async Task<IActionResult> GenerateEnquiryV2([FromBody] EnquiryMainModel model)
        {
            try
            {
                //if (string.IsNullOrEmpty(EnquiryMainModels))
                //{
                //    return Ok(ApiResponse<string>.Fail("Failed"));
                //}
                //EnquiryMainModel model = JsonConvert.DeserializeObject<EnquiryMainModel>(EnquiryMainModels);
                //if (file == null)
                //{
                //    return Ok(ApiResponse<string>.Fail("Failed"));
                //}
                //var file1 = await _getFilename._getFileName(file);
                //var file1 = "http//loadimage.com/image.pdf";

                if (string.IsNullOrEmpty(model.dealershipCode) && string.IsNullOrEmpty(model.Enquiry.prospectMobile))
                    return Ok("Failed");
                string dealerCode = User.FindFirst("dealercode")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string mobileNumber = User.FindFirst("mobilenumber")?.Value;
                string PlatformType = User.FindFirst("PlatformType")?.Value;
                /////var result = await _db.GenerateSalesEnquiryv1(model, dealerCode, username, mobileNumber);
                var result = await _db.GenerateSalesEnquiryv2(model, dealerCode, username, mobileNumber, PlatformType, null);
                //var result = 1;

                if (result > 0)
                    return Ok(ApiResponse<string>.Success(default));
                return Ok(ApiResponse<string>.Fail("Error"));
            }
            catch (Exception Ex)
            {

                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.GenerateEnquiryV3)]
        public async Task<IActionResult> GenerateEnquiryV3([FromForm] string EnquiryMainModels, IFormFile file)
        {
            try
            {
                if (string.IsNullOrEmpty(EnquiryMainModels))
                {
                    return Ok(ApiResponse<string>.Fail("Failed"));
                }
                EnquiryMainModel model = JsonConvert.DeserializeObject<EnquiryMainModel>(EnquiryMainModels);
                if (model.Sale.prospectType == "Exchange" && file == null)
                {
                    return Ok(ApiResponse<string>.Fail("Failed"));
                }

                string file1 = null;

                if (file != null)
                {
                    file1 = await _getFilename._getFileName(file);
                }
                //var file1 = "http//loadimage.com/image.pdf";

                if (string.IsNullOrEmpty(model.dealershipCode) && string.IsNullOrEmpty(model.Enquiry.prospectMobile))
                    return Ok("Failed");
                string dealerCode = User.FindFirst("dealercode")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string mobileNumber = User.FindFirst("mobilenumber")?.Value;
                string PlatformType = User.FindFirst("PlatformType")?.Value;
                /////var result = await _db.GenerateSalesEnquiryv1(model, dealerCode, username, mobileNumber);
                var result = await _db.GenerateSalesEnquiryv2(model, dealerCode, username, mobileNumber, PlatformType, file1);
                //var result = 1;

                if (result > 0)
                    return Ok(ApiResponse<string>.Success(default));
                return Ok(ApiResponse<string>.Fail("Error"));
            }
            catch (Exception Ex)
            {

                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.GetsalesEnq2)]
        public async Task<IActionResult> GetsalesEnq2([FromBody] SalesEnquirySearchdto model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.GetSalesEnq2db(LoginPosition, username, model);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch { return Ok(ApiResponse<string>.Fail("Error")); }


        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.Rc.GetRcStatusList)]
        public async Task<IActionResult> GetRcStatusList([FromBody] Rc model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.getRcrecords(model, LoginPosition, username);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex) { return Ok(ApiResponse<string>.Fail(ex.Message)); }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.Rc.UpdateRcStatus)]
        public async Task<IActionResult> UpdateRcStatus([FromBody] Rc model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.updRcrecords(model, LoginPosition, username);
                if (result > 0)
                    return Ok(ApiResponse<string>.Success(default));
                else
                    return Ok(ApiResponse<string>.Fail("Error"));
                //return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex) { return Ok(ApiResponse<string>.Fail(ex.Message)); }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.UpdtGenerateEnquiryv2)]
        public async Task<IActionResult> UpdtGenerateEnquiryv2([FromBody] UpdateEnquiryMainModel model)
        {
            try
            {
                //if (string.IsNullOrEmpty(UpdateEnquiryMainModels))
                //{
                //    return Ok(ApiResponse<string>.Fail("Failed"));
                //}
                //UpdateEnquiryMainModel model = JsonConvert.DeserializeObject<UpdateEnquiryMainModel>(UpdateEnquiryMainModels);
                //if (file == null)
                //{
                //    return Ok(ApiResponse<string>.Fail("Failed"));
                //}
                //var file1 = await _getFilename._getFileName(file);
                //var file1 = "http//loadimage.com/image.pdf";
                if (string.IsNullOrEmpty(model.enquiryMainModel.dealershipCode) && string.IsNullOrEmpty(model.enquiryMainModel.Enquiry.prospectMobile))
                    return Ok(ApiResponse<string>.Fail("Failed"));
                string dealerCode = User.FindFirst("dealercode")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string mobileNumber = User.FindFirst("mobilenumber")?.Value;
                string PlatformType = User.FindFirst("PlatformType")?.Value;
                //   //var result = await _db.GenerateSalesEnquiryv1(model, dealerCode, username, mobileNumber);
                var result = await _db.updtGenerateSalesEnquiryv2(model, dealerCode, username, mobileNumber, PlatformType, null);
                //var result = 0;
                if (result > 0)
                    return Ok(ApiResponse<string>.Success(default));
                return Ok(ApiResponse<string>.Fail("Error"));
            }
            catch (Exception Ex)
            {

                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }

        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.UpdtGenerateEnquiryv3)]
        public async Task<IActionResult> UpdtGenerateEnquiryv3([FromForm] string UpdateEnquiryMainModels, IFormFile file)
        {
            try
            {
                if (string.IsNullOrEmpty(UpdateEnquiryMainModels))
                {
                    return Ok(ApiResponse<string>.Fail("Failed"));
                }
                UpdateEnquiryMainModel model = JsonConvert.DeserializeObject<UpdateEnquiryMainModel>(UpdateEnquiryMainModels);
                if (model.enquiryMainModel.Sale.prospectType == "Exchange" && file == null)
                {
                    return Ok(ApiResponse<string>.Fail("Failed"));
                }

                string file1 = null;

                if (file != null)
                {
                    file1 = await _getFilename._getFileName(file);
                }
                //var file1 = "http//loadimage.com/image.pdf";
                if (string.IsNullOrEmpty(model.enquiryMainModel.dealershipCode) && string.IsNullOrEmpty(model.enquiryMainModel.Enquiry.prospectMobile))
                    return Ok(ApiResponse<string>.Fail("Failed"));
                string dealerCode = User.FindFirst("dealercode")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string mobileNumber = User.FindFirst("mobilenumber")?.Value;
                string PlatformType = User.FindFirst("PlatformType")?.Value;
                //   //var result = await _db.GenerateSalesEnquiryv1(model, dealerCode, username, mobileNumber);
                var result = await _db.updtGenerateSalesEnquiryv2(model, dealerCode, username, mobileNumber, PlatformType, file1);
                //var result = 0;
                if (result > 0)
                    return Ok(ApiResponse<string>.Success(default));
                return Ok(ApiResponse<string>.Fail("Error"));
            }
            catch (Exception Ex)
            {

                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }

        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.GetSalesEnquiryById2)]
        public async Task<IActionResult> GetSalesEnquiryById2([FromBody] salesEnquiryIdDto model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.GetSalesEnqbyId2db(LoginPosition, username, model);
                return Ok(ApiResponse<dynamic>.Success(result));
            }
            catch { return Ok(ApiResponse<string>.Fail("Error")); }


        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.GetAvailableChassisNo)]
        public async Task<IActionResult> GetAvailableChassisNo([FromBody] StockMaster stockMaster)
        {
            try
            {
                var result = await _db.GetChassisNumber(stockMaster.DealerCode);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
            ;

        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.PopUp.GetPopUpCount)]
        public async Task<IActionResult> GetPopUpCount()
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.GetPopUpCountDb(LoginPosition, username);
                return Ok(ApiResponse<dynamic>.Success(result));
            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
            ;

        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.PopUp.ReportLastDateHeading)]
        public async Task<IActionResult> ReportLastDateHeading()
        {
            try
            {
                //string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                //string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.ReportLastDateHeading();
                return Ok(ApiResponse<dynamic>.Success(result));
            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
            ;

        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.FinanceMaster.AddtionalPaymentRec)]
        public async Task<IActionResult> AddtionalPaymentRec([FromBody] salesEnquiryIdDto id)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.AddtionalPaymentRecdb(id);
                return Ok(ApiResponse<dynamic>.Success(result));
            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
            ;

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.GetExchangeStock.GetExch)]
        public async Task<IActionResult> GetExch([FromBody] getExchangeStock excModel)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.getExchangeStockdb(excModel, LoginPosition, username);
                return Ok(ApiResponse<dynamic>.Success(result));
            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
            ;

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.GetExchangeStock.SoldMarkExch)]
        public async Task<IActionResult> SoldMarkExch([FromBody] SoldExchStock Model)
        {
            try
            {
                string platfmtype = User.FindFirst("PlatformType")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.SoldExchdb(Model, platfmtype, username);
                return Ok(ApiResponse<dynamic>.Success(default));
            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
            ;

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.OldEnquiryTemp.GetOldEnquiry)]
        public async Task<IActionResult> GetOldEnquiry([FromBody] getExchangeStock excModel)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.GetOldEnquirydb(excModel, LoginPosition, username);
                return Ok(ApiResponse<dynamic>.Success(result));
            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
            ;

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.OldEnquiryTemp.UpdateOldEnquiry)]
        public async Task<IActionResult> UpdateOldEnquiry([FromBody] oldEnquiryUpdate Model)
        {
            try
            {
                string platfmtype = User.FindFirst("PlatformType")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.UpdateOldEnquirydb(Model, platfmtype, username);
                return Ok(ApiResponse<dynamic>.Success(default));
            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
            ;

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.NDAForm.InsertNDAForm)]
        public async Task<IActionResult> InsertNDAForm([FromBody] NDAFormModel model)
        {
            try
            {
                var result = await _db.InsertNDAFormdb(model);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var userDetails = await _db.getStateHeadByStateName(result);
                    DealerAppointmentRequest dealerAppointmentRequest = new DealerAppointmentRequest()
                    {
                        MobileNumber = userDetails.MobileNumber,
                        //MobileNumber = "8952031297",
                        WhatsappUserName = userDetails.Name,
                        ProspectName = model.NDAProspectName,
                        ProspectMobile = model.MobileNo,
                        InterestedDistrict = model.DistrictName,
                        InterestedLocation = model.CityName,
                        CurrentBusiness = model.CurrentBussiness
                    };

                    string res = await GromaxMobileApis.Utilities.WhatsappMessageSend.SendDealerAppointmentAsync(dealerAppointmentRequest);
                    string messageString = await GromaxMobileApis.Utilities.WhatsappMessageSend.getMessageString(dealerAppointmentRequest);

                    if (!string.IsNullOrEmpty(res) && !res.Contains("error") && res.Contains("wamid."))
                    {
                        smartpingresponseRound response = Newtonsoft.Json.JsonConvert.DeserializeObject<smartpingresponseRound>(res);
                        await _db.updateNDAWhatsappCount(response.messages[0].id, messageString, userDetails.MobileNumber, "SENT", "Gromax NDA");

                    }
                    else
                    {
                        await _db.updateNDAWhatsappCount(null, messageString, userDetails.MobileNumber, "FAILED", "Gromax NDA");

                    }
                    return Ok(ApiResponse<string>.Created());

                }
                else
                    return Ok(ApiResponse<string>.Fail("Something went wrong while processing your request. Please try again later."));

            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.NDAForm.GetDistrict)]
        public async Task<IActionResult> getDistrict([FromBody] GetDistrict model)
        {
            try
            {
                var result = await _db.GetDistrict(model);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));


            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.NDAForm.GetTehsil)]
        public async Task<IActionResult> GetTehsil([FromBody] List<GetTehsil> model)
        {
            try
            {
                var result = await _db.GetTehsil(model);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));


            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.NDAForm.GetCity)]
        public async Task<IActionResult> GetCity([FromBody] List<GetCity> model)
        {
            try
            {
                var result = await _db.GetCity(model);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));


            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.NDAForm.GetNda)]
        public async Task<IActionResult> GetNda([FromQuery] RequestGetNDA m)
        {
            try
            {
                var result = await _db.GetNdadb(m);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));


            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.NDAForm.updtNDA)]
        public async Task<IActionResult> updtNDA([FromBody] NDAEnquiryModel model)
        {
            try
            {
                var result = await _db.updtNDAdb(model);
                if (result > 0)
                    return Ok(ApiResponse<string>.Created());
                else
                    return Ok(ApiResponse<string>.Fail("Something went wrong while processing your request. Please try again later."));



            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.NDAForm.GetNDAById)]
        public async Task<IActionResult> GetNDAById([FromBody] GetNDAByIDModel model)
        {
            try
            {
                var result = await _db.GetNDAByIddb(model);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));



            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.PossitionMaster.GetStateHeadByState)]
        public async Task<IActionResult> GetStateHeadByState([FromBody] StateCodeModel model)
        {
            try
            {
                var result = await _db.GetStateHeadByStatedb(model);
                return Ok(new { message = "Success", data = result });



            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.OldEnquiryTemp.GetOldRetailedEnquiry)]
        public async Task<IActionResult> GetOldRetailedEnquiry([FromBody] getExchangeStock excModel)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.GetOldRetailedEnquirydb(excModel, LoginPosition, username);
                return Ok(ApiResponse<dynamic>.Success(result));
            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
           ;

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.Retiledsales.DownloadNotRetailedList)]
        public async Task<IActionResult> DownloadNotRetailedList([FromBody] RetailedSalesFilter filter)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.DownloadNotRetailedList(filter, LoginPosition, username);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.GetExchangeStock.DownloadExch)]
        public async Task<IActionResult> DownloadExch([FromBody] getExchangeStock excModel)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.downloadExchangeStockdb(excModel, LoginPosition, username);
                return Ok(ApiResponse<dynamic>.Success(result));
            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
           ;

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.NDAForm.GetNDAHistoryById)]
        public async Task<IActionResult> GetNDAHistoryById([FromBody] GetNDAByIDModel model)
        {
            try
            {
                var result = await _db.GetNDAHistoryById(model);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));



            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesmanMaster.Insertv1)]

        public async Task<IActionResult> InsertSalesmanv1(SalesmanMasterv1 model)
        {
            try
            {
                string file1 = ""; string file2 = ""; string file3 = "";
                string PlatformType = User.FindFirst("PlatformType")?.Value;
                if (model.kycstatus == "Yes")
                {
                    if (model.file1 == null || model.file2 == null || model.file3 == null)
                    {
                        return Ok(ApiResponse<string>.Fail("Failed"));
                    }
                    file1 = await _getFilename._getFileName(model.file1);
                    if (string.IsNullOrEmpty(file1))
                    {
                        return Ok(ApiResponse<string>.Fail("File1 blank"));
                    }
                    file2 = await _getFilename._getFileName(model.file2);
                    if (string.IsNullOrEmpty(file2))
                    {
                        return Ok(ApiResponse<string>.Fail("File2 blank"));
                    }
                    file3 = await _getFilename._getFileName(model.file3);
                    if (string.IsNullOrEmpty(file3))
                    {
                        return Ok(ApiResponse<string>.Fail("File3 blank"));
                    }
                }
                var result = await _db.InsertSalesmandbv1(model, PlatformType, file1, file2, file3);
                if (result > 0)
                    return Ok(ApiResponse<string>.Created());
                else
                    return Ok(ApiResponse<string>.Fail("Failed"));

            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail(ex.Message));
            }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesmanMaster.ApproveSalesmanKyc)]

        public async Task<IActionResult> ApproveSalesmanKyc([FromBody] ApprovalSalesmanKyc model)
        {
            try
            {

                var result = await _db.ApprovalSalesmanKyc(model);
                if (result > 0)
                    return Ok(ApiResponse<string>.Created());
                else
                    return Ok(ApiResponse<string>.Fail("Failed"));

            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail(ex.Message));
            }

        }
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesmanMaster.GetKycPendingList)]

        public async Task<IActionResult> GetKycPendingList([FromBody] SalesmanGetdto model)
        {
            try
            {

                var result = await _db.GetKycPendingListdb(model);

                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));


            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail(ex.Message));
            }

        }

        //[HttpPost]
        //[Route(GromaxMobileApis.Utilities.ApiRoutes.Other.getLocationList)]

        //public async Task<IActionResult> getLocationList([FromBody] PositionFilter model)
        //{
        //    try
        //    {

        //        var result = await _db.getLocationListdb(model);
        //        return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));


        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ApiResponse<string>.Fail(ex.Message));
        //    }

        //}

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.Other.updatedealer)]

        public async Task<IActionResult> updatedealer([FromBody] DealerUpdate model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.DealerCode) || string.IsNullOrEmpty(model.DealerStatus))
                {
                    return Ok(ApiResponse<string>.BadRequest("Invalid request. Please check the provided data."));
                }
                var result = await _db.updatedealerdb(model);
                if (result > 0)
                    return Ok(ApiResponse<string>.Created());
                else
                    return Ok(ApiResponse<string>.Fail("Fail"));



            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail(ex.Message));
            }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.GetChassisForReturnBilling)]

        public async Task<IActionResult> GetChassisForReturnBilling([FromBody] Pagignation model)
        {
            try
            {

                var result = await _db.GetChassisForReturnBillingdb(model);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));




            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail(ex.Message));
            }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.StockMaster.ReturnBilling)]
        public async Task<IActionResult> ReturnBilling([FromBody] ReturnBillingModel model)
        {
            try
            {

                if (string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(model.ReturnDate))
                    return Ok(ApiResponse<string>.BadRequest("Invalid request. Please check the provided data."));

                var result = await _db.ReturnBillingdb(model);
                //var result = 1;
                if (result > 0)
                    return Ok(ApiResponse<string>.Created());
                else
                    return Ok(ApiResponse<string>.Fail("Fail"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail(ex.Message));
            }

        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.GetExchangeStock.GetExchangeModelList)]
        public async Task<IActionResult> GetExchangeModelList()
        {
            try
            {
                var result = await _db.GetExchangeModelListdb();
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));


            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.getSourceSubSource)]
        public async Task<IActionResult> getSourceSubSource()
        {
            try
            {
                var result = await _db.getSourceSubSourcedb();
                //var Source = result.Select(x => x.SourceName).Distinct();
                //var subSource = result.Select(x => x.sub).Distinct();
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));


            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.getSourceSubSourceForRepFilter)]
        public async Task<IActionResult> getSourceSubSourceForRepFilter()
        {
            try
            {
                var result = await _db.getSourceSubSourceForRepFilter();
                //var Source = result.Select(x => x.SourceName).Distinct();
                //var subSource = result.Select(x => x.sub).Distinct();
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));


            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.Other.getbillingReqData)]
        public async Task<IActionResult> getbillingReqData([FromBody] BillingReqModel model)
        {
            try
            {
                var result = await _db.getbillingReqData(model);
                //var Source = result.Select(x => x.SourceName).Distinct();
                //var subSource = result.Select(x => x.sub).Distinct();
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));


            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }



        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.Other.getDealerByTehsil)]
        public async Task<IActionResult> getDealerByTehsil([FromBody] DealerListByLocationModel model)
        {
            try
            {
                var data = await dbweb.getDealerByTehsildb(model);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(data));
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

        }



        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.NotAssign.notAssignDealerList)]
        public async Task<IActionResult> notAssignDealerList([FromBody] NotAssign model)
        {
            try
            {
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var data = await dbweb.notAssignDealerList(model, username);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(data));
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.NotAssign.assignDealerToEnquiry)]
        public async Task<IActionResult> assignDealerToEnquiry([FromBody] AssignDealerModel model)
        {
            try
            {
                if (model.Ids == null || model.Ids.Count == 0)
                    return BadRequest(new { Code = 400, Message = "No enquiry ids provided." });

                if (string.IsNullOrEmpty(model.DealerCode))
                    return BadRequest(new { Code = 400, Message = "DealerCode is required." });

                var result = await dbweb.assignDealerToEnquiry(model);
                return Ok(ApiResponse<bool>.Success(result));
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.getUnassignedCount)]
        public async Task<IActionResult> getUnassignedCount()
        {
            try
            {
                var result = await _db.getUnassignedCountdb();
                //var Source = result.Select(x => x.SourceName).Distinct();
                //var subSource = result.Select(x => x.sub).Distinct();
                return Ok(ApiResponse<dynamic>.Success(result));


            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.InstallationMaster.InsertImgv1)]
        public async Task<IActionResult> InsertInstallationImgv1([FromForm] installationReq form)
        {
            try
            {
                string installationId = form.InstallationMasterId;
                string Latitude = form.Latitude;
                string Longitude = form.Longitude;
                string Address = form.Address;
                if (string.IsNullOrEmpty(installationId))
                    return BadRequest("Error.");

                if (form.installationImages == null || !form.installationImages.Any())
                {
                    return BadRequest("All images are required.");
                }

                foreach (var file in form.installationImages)
                {
                    if (file.Image == null || file.Image.Length == 0)
                    {
                        return BadRequest("All images are required.");
                    }
                }



                var modellist = new List<InstallationImageData>();
                string fileUrl = "";
                foreach (var file in form.installationImages)
                {
                    if (file.Image.Length > 0)
                    {
                        string baseUrl = "https://loadinfotechdb.blob.core.windows.net/gromaxwebprod1/";
                        string fileName = Path.GetFileNameWithoutExtension(file.Image.FileName).Trim().Replace(" ", "_") +
                                          "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                                          Path.GetExtension(file.Image.FileName);
                        using (var stream = file.Image.OpenReadStream())
                        {
                            fileUrl = await azureStorageService.UploadAsync(stream, fileName, file.Image.ContentType);


                        }
                        baseUrl = baseUrl + fileName;
                        modellist.Add(new InstallationImageData
                        {
                            InstallationMasterId = installationId,
                            ImgUrl = baseUrl,
                            Latitude = Latitude,
                            Longitude = Longitude,
                            Address = Address,
                            TagName = file.TagName,
                        });
                    }
                }

                DataTable dt = GromaxMobileApis.Utilities.UtilityFunctions.ConvertListToDataTableV2(modellist);
                var result = await _db.InsertInstallationImgv1(dt, form.WorkingHrs);

                return result > 0 ? Ok("Inserted successfully") : BadRequest("Insert failed.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.SalesEnquiry.getThreeDaysOdEnquiries)]
        public async Task<IActionResult> getThreeDaysOdEnquiries([FromQuery] SuperHotEnquiry model)
        {
            try
            {
                var result = await _db.getThreeDaysOdEnquiriesdb(model);

                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.NDAForm.getPendingConversionByList)]
        public async Task<IActionResult> getPendingConversionByList([FromQuery] RequestGetNDA m)
        {
            try
            {
                var result = await _db.getPendingConversionByListdb(m);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));


            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.NDAForm.getIndustryByTaluka)]
        public async Task<IActionResult> getIndustryByTaluka([FromQuery] int talukaCode)
        {
            try
            {
                var result = await _db.getIndustryByTalukadb(talukaCode);
                return Ok(ApiResponse<int>.Success(result));
            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.Menu.getMenuList)]
        public async Task<IActionResult> getMenuList()
        {
            try
            {
                var result = await _db.getMenuListdb();
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.Employee.getEmployeeList)]

        public async Task<IActionResult> getEmployeeList([FromQuery] RequestEmployeeMaster m)
        {
            try
            {
                var result = await _db.getEmployeeMasterList(m);
                return Ok(ApiResponse<IEnumerable<ResponseEmployeeMaster>>.Success(result));
            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.NDAForm.updtConversionBy)]

        public async Task<IActionResult> updtConversionBy([FromBody] reqForUpdateConversion m)
        {
            try
            {
                var result = await _db.updtConversionBydb(m);
                if (result > 0)
                    return Ok(ApiResponse<string>.Created());
                return NotFound(ApiResponse<string>.NotFound("Record not found."));
            }
            catch (Exception Ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(Ex.Message));
            }

        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.DealerMaster.DealerDetailByDealerCode)]
        public async Task<IActionResult> DealerDetailByDealerCode([FromQuery] string dealerCode)
        {
            try
            {
                var result = await _db.DealerDetailByDealerCodedb(dealerCode);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.DealerMaster.updateDealerAssignments)]

        public async Task<IActionResult> updateDealerAssignments([FromBody] UpdateDealerFormRequest m)
        {
            try
            {
                var result = await _db.updateDealerAssignmentsdb(m);
                if (result > 0)
                    return Ok(ApiResponse<string>.Created());
                return NotFound(ApiResponse<string>.NotFound("Record not found."));
            }
            catch (Exception Ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(Ex.Message));
            }

        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.Employee.getEmployeeListByPosition)]

        public async Task<IActionResult> getEmployeeListByPosition([FromQuery] string position)
        {
            try
            {
                var result = await _db.getEmployeeMasterListByPositionDb(position);
                return Ok(ApiResponse<IEnumerable<ResponseEmployeeByPosition>>.Success(result));
            }
            catch (Exception Ex)
            {
                return Ok(ApiResponse<string>.Fail(Ex.Message));
            }

        }

    }


    #endregion
}
