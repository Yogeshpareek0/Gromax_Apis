using GromaxMobileApis;
using GromaxMobileApis.Interfaces;
using GromaxMobileApis.Models;
using GromaxMobileApis.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GromaxMobileApis.Controllers
{
    [Authorize]
    public class WebHomeController : ControllerBase
    {
        private readonly IDatabaseServicesweb _db;
        private readonly IDatabaseService _Mdb;
        private readonly ResponseClass _r;
        public WebHomeController(IDatabaseServicesweb db, ResponseClass r, IDatabaseService mdb)
        {
            _r = r;
            _db = db;
            _Mdb = mdb;
        }
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.PossitionMaster.PossitionFilter)]
        public async Task<IActionResult> PossitionFilter([FromBody] PositionFilter model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var data = await _db.GetPossitionFilter(model, LoginPosition, username);
                return Ok(new { message = "Success", data = data });
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.SalesEnquiryMaster.GetSalesEnquiryMaster)]
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
                var result1 = await _db.GetLeads(model, LoginPosition, username);
                return Ok(new { Message = "Success", Data = result, Leads = result1 });
            }
            catch (Exception ex) { throw new Exception(ex.Message); }



        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.SalesEnquiryMaster.GetSalesEnquiryMasterv1)]
        public async Task<IActionResult> GetSalesEnquiryMasterv1([FromBody] SalesEnquiryMasterFiltered model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(ms => ms.Value.Errors.Any()).Select(ms => ms.Key).ToList();
                    return BadRequest(new { Code = 400, Message = _r.MissingParam, MissingFields = errors });
                }

                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.GetSalesEnquiryMasterv1(model, LoginPosition, username);
                var result1 = await _db.GetLeads(model, LoginPosition, username);
                return Ok(new { Message = "Success", Data = result, Leads = result1 });
            }
            catch (Exception ex) { throw new Exception(ex.Message); }



        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.StockMaster.GetInventoryData)]
        public async Task<IActionResult> GetInventoryData([FromBody] StockFilterModel model)
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
                var result = await _db.GetInventoryDataWeb(model, LoginPosition, username);
                var firstRow = result.FirstOrDefault();
                string totalCount = firstRow?.TotalCount?.ToString();
                totalCount = string.IsNullOrEmpty(totalCount) ? "0" : totalCount;
                return Ok(new { message = "Success", data = result, totalCount = totalCount });
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        //[HttpPost]
        //[Route(GromaxMobileApis.Utilities.WebApiRoutes.SalesEnquiryMaster.GetSalesTarget)]
        //public async Task<IActionResult> GetSalesTarget([FromBody] FilterModel model)
        //{
        //    try
        //    {//[TotalHot],[TotalCold],[TotalWarm],[TotalLeads]
        //        string LoginPosition = User.FindFirst("LoginPosition")?.Value;
        //        string username = User.FindFirst(ClaimTypes.Name)?.Value;
        //        var result = await _db.GetLeads(model, LoginPosition, username);
        //        return Ok(new { message = "Success", data = result });
        //    }
        //    catch (Exception Ex)
        //    {

        //        return BadRequest(new { Message = Ex.Message });
        //    }
        //}

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.SalesEnquiryMaster.GetLeadsByFilter)]
        public async Task<IActionResult> GetLeadsFollowUpWebDashboard()
        {//TotalLeads,OverDueTarget,TodayLeads,HotLeads,WarmLeads,ColdLeads
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _db.GetLeadsFollowUpFilterWeb(LoginPosition, username);

                return Ok(new { message = "Success", data = result });
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.BussinessPerformance.Get)]
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
                var result = await _db.GetBussinessPerformanceWeb(model, LoginPosition, username);
                return Ok(new { message = "Success", data = result });
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }


        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.BussinessPerformance.Getv1)]
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
                var result = await _db.GetBussinessPerformanceWebv1(model, LoginPosition, username);
                return Ok(new { message = "Success", data = result });
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }


        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.SalesEnquiryMaster.Insert)]
        public async Task<IActionResult> GenerateEnquiry([FromBody] SalesEnquiryMaster model)
        {
            try
            {
                string dealerCode = User.FindFirst("dealercode")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string mobileNumber = User.FindFirst("mobilenumber")?.Value;
                //var result = await _Mdb.GenerateSalesEnquiryv1(model, dealerCode, username, mobileNumber);
                var result = await _Mdb.GenerateSalesEnquiryv1(model, dealerCode, username, mobileNumber, "web");
                if (result > 0)
                    return Ok(new { message = "Success", data = "" });
                return Ok("Failed");
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.SalesEnquiryMaster.Get)]
        public async Task<IActionResult> GetSalesEnquiry([FromBody] SalesEnquiryMasterdto model)
        {
            try
            {
                string dealerCode = User.FindFirst("dealercode")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string mobileNumber = User.FindFirst("mobilenumber")?.Value;
                var result = await _Mdb.GetSalesEnquiry(model, dealerCode, username, LoginPosition);
                return Ok(new { Message = "Success", date = result });
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.ModelMaster.Get)]
        public async Task<IActionResult> Getmodelmaster()
        {
            try
            {
                var result = await _Mdb.Getmodelmasterlist();
                return Ok(new { Message = "Success", data = result });
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.StateDistrictTehsilMaster.GetState)]
        public async Task<IActionResult> GetState(string DealerCode)
        {
            try
            {
                var result = await _Mdb.GetStatedb(DealerCode);
                return Ok(new { Message = "Success", data = result });

            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.StateDistrictTehsilMaster.Filter)]
        public async Task<IActionResult> GetCustomerAddressFilter(string Type, string Name, string code)
        {
            try
            {
                var result = await _Mdb.GetCustomerAddressFilter(Type, Name, code);
                return Ok(new { Message = "Success", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.StockMaster.InsertStock)]
        public async Task<IActionResult> InsertStock([FromForm] IFormFile StockFile)
        {
            try
            {
                var StockFiledt = GromaxMobileApis.Utilities.UtilityFunctions.ConvertStockExcelToDataTablev1(StockFile);
                var result = await _db.InsertStockdb(StockFiledt);
                if (result > 0)
                    return Ok(new { Message = "Success" });
                return Ok(new { Message = "Failed" });
            }
            catch (Exception ex) { return BadRequest(); }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.SalesEnquiryMaster.DownloadSalesEnquiryMaster)]
        public async Task<IActionResult> DownloadSalesEnquiryMaster([FromBody] SalesEnquiryMasterFiltered model)
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
                var result = await _db.DownloadSalesEnquiryMasterdb(model, LoginPosition, username);
                //var result1 = await _db.GetLeads(model, LoginPosition, username);
                return Ok(new { Message = "Success", Data = result });
            }
            catch (Exception ex) { throw new Exception(ex.Message); }



        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.CampaignsalesEnquiry.UploadCampaignsalesEnquiry)]
        public async Task<IActionResult> UploadCampaigningEnquiry([FromForm] IFormFile CampaignFile)
        {
            try
            {
                DataTable CampaignDt = new DataTable();
                if (CampaignFile != null && CampaignFile.Length > 0)
                {
                    CampaignDt = GromaxMobileApis.Utilities.UtilityFunctions.ConvertCampaignExcelToDataTable(CampaignFile);
                }
                var result = await _db.UploadCampaignSalesEnquiry(CampaignDt);
                if (result > 0)
                    return Ok(new { Message = "Sucecss" });
                else
                    return Ok(new { Message = "Failed" });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.SalesEnquiryMaster.GetPendingFillowupList)]
        public async Task<IActionResult> PendingSalesFollowupList([FromBody] EnquiryFilterRequest model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string dealerCode = User.FindFirst("dealercode")?.Value;
                //string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string mobileNumber = User.FindFirst("mobilenumber")?.Value;
                var result = await _db.GetPendingSalesFollowupListv1(model, mobileNumber, dealerCode, LoginPosition, username);
                int totalCount = result != null && result.Count() > 0 ? result?.Select(r => r.TotalCount).FirstOrDefault() : 0;
                return Ok(new { Message = "Success", Data = result, TotalCount = totalCount });
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.SalesEnquiryMaster.GetLeadsFollowup)]
        public async Task<IActionResult> GetLeadsFollowUp()
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string dealerCode = User.FindFirst("dealercode")?.Value;
                var LeadFollowUp = await _db.GetLeadFollowUp(dealerCode, LoginPosition, username);
                //return Ok(LeadFollowUp);
                return Ok(new { Message = "Success", data = LeadFollowUp });
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.StockMaster.GetAvailableStockData)]
        public async Task<IActionResult> GetAvailableStockData([FromBody] StockFilterModel model)
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
                var result = await _db.GetAvailableStockDatadb(model, LoginPosition, username);
                var firstRow = result.FirstOrDefault();
                string totalCount = firstRow?.TotalCount?.ToString();
                totalCount = string.IsNullOrEmpty(totalCount) ? "0" : totalCount;
                return Ok(new { message = "Success", data = result, totalCount = totalCount });
            }
            catch (Exception Ex)
            {

                return BadRequest(new { Message = Ex.Message });
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.StockMaster.InsertBilling)]
        public async Task<IActionResult> InsertBilling([FromForm] IFormFile StockFile)
        {
            try
            {
                var StockFiledt = GromaxMobileApis.Utilities.UtilityFunctions.ConvertStockExcelToDataTablev2(StockFile);
                var result = await _db.InsertBillingdb(StockFiledt);
                if (result > 0)
                    return Ok(ApiResponse<string>.Created());
                return Ok(ApiResponse<string>.Fail("Something Error"));
            }
            catch (Exception ex)
            { return Ok(ApiResponse<string>.Fail(ex.Message)); }



        }
    }
}
