using Azure;
using Google.Apis.Util;
using GromaxMobileApis.Interfaces;
using GromaxMobileApis.Models;
using GromaxMobileApis.Models.DealerMaster;
using GromaxMobileApis.Models.Services;
using GromaxMobileApis.Models.VerifyWebhook;
using GromaxMobileApis.Services;
using GromaxMobileApis.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static GromaxMobileApis.Models.Services.FieldTechnicalReportModel;
using static GromaxMobileApis.Models.Services.SparePartsModel;
using static GromaxMobileApis.Models.Services.WarrantyClaimModel;

namespace GromaxMobileApis.Controllers
{
    [ApiController]
    [Authorize]
    public class CustomerServiceControlle : ControllerBase
    {
        private ICustomerService _customerService;
        private getFileName _getFileName;
        private IAzureStorageService _azure;
        private readonly ServiceInvoicePdfGenerator _pdfGenerator;
        private readonly JobCardPdfGenerator _JobCardpdfGenerator;
        private readonly ILogger<CustomerServiceControlle> _logger;
        private readonly IDatabaseService _db;
        public CustomerServiceControlle(ICustomerService customerService, getFileName getFileName,
            IAzureStorageService azure, ILogger<CustomerServiceControlle> logger, ServiceInvoicePdfGenerator serviceInvoice,
            IDatabaseService databaseService, JobCardPdfGenerator jobCardpdfGenerator)
        {
            _customerService = customerService;
            _getFileName = getFileName;
            _azure = azure;
            _logger = logger;
            _pdfGenerator = serviceInvoice;
            _db = databaseService;
            _JobCardpdfGenerator = jobCardpdfGenerator;
        }
        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.pdiList)]
        public async Task<IActionResult> PDIPendingList([FromQuery] PDIListResponse r)
        {
            try
            {
                var l = await _customerService.getPDIPendingList(r);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.addPDI)]
        public async Task<IActionResult> addPDI([FromBody] PdiRequest m)
        {
            try
            {
                DataTable p = new DataTable();
                try
                {
                    p = GromaxMobileApis.Utilities.UtilityFunctions.GetPdiInspectionItemsDataTable(m.fields);
                }
                catch
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.BadRequest("DT Convert Error"));
                }
                var r = await _customerService.addPDIdb(m, p);

                if (r <= 0)
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ApiResponse<string>.Fail("Unable to save PDI."));

                return Ok(ApiResponse<string>.Created());
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.uploadPDIImage)]
        public async Task<IActionResult> uploadPDIImage([FromForm] IFormFile file)
        {
            try
            {
                var fileName = await _getFileName._getFileNamev1(file, "PDI");
                return Ok(ApiResponse<string>.Success(fileName));
            }
            catch
            {
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.BadRequest("Image Upload Error"));
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.getWorkNature)]
        public async Task<IActionResult> getWorkNature()
        {
            try
            {
                var l = await _customerService.getWorkNatureListAsync();
                return Ok(ApiResponse<List<WorkNatureMasterResponse>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.removeImage)]
        public async Task<IActionResult> removeImage([FromBody] RemoveImageRequest fileName)
        {
            try
            {
                try
                {
                    await _azure.DeleteAsyncv1(fileName.FileName);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status404NotFound, ApiResponse<string>.Fail(ex.Message));
                }
                return Ok(ApiResponse<string>.Success("File deleted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.jobCardEligibleChassis)]
        public async Task<IActionResult> jobCardEligibleChassis([FromQuery] EligibleChassisListRequest r)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(r.searchQuery) || string.IsNullOrWhiteSpace(r.searchBy))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.BadRequest("Please select a valid search criteria."));
                }
                var l = await _customerService.getEligibleChassisList(r);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.spareParts.getSpareParts)]
        public async Task<IActionResult> getSpareParts()
        {
            try
            {
                var l = await _customerService.getSparePartsdb();
                return Ok(ApiResponse<List<SparesPartMaster>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Authorize(Roles = "Dealer")]
        [Route(GromaxMobileApis.Utilities.customerServices.jobCard.addJobCard)]
        public async Task<IActionResult> addJobCard([FromBody] JobCardMaster j)
        {
            try
            {


                JobCardDataTables tables = new JobCardDataTables();
                try
                {
                    tables = UtilityFunctions.CreateJobCardDataTables(j);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.BadRequest(ex.Message));
                }
                AddJobCardRequest request = new AddJobCardRequest()
                {
                    SalesMasterId = j.SalesMasterId,
                    ChassiNumber = j.ChassiNumber,
                    JobCardDate = j.JobCardDate,
                    JobCardType = j.JobCardType,
                    Fuel = j.Fuel,
                    DealerName = j.DealerName,
                    CustomerName = j.CustomerName,
                    CustomerAddress = j.CustomerAddress,
                    TractorSlNo = j.TractorSlNo,
                    RegnNo = j.RegnNo,
                    DateOfSale = j.DateOfSale,
                    WorkDoneBy = j.WorkDoneBy,
                    MobileNo = j.MobileNo,
                    AlternateMobileNo = j.AlternateMobileNo,

                    FrontTyrePressureLeft = j.FrontTyrePressureLeft,
                    FrontTyrePressureRight = j.FrontTyrePressureRight,
                    RearTyrePressureLeft = j.RearTyrePressureLeft,
                    RearTyrePressureRight = j.RearTyrePressureRight,

                    Hours = j.Hours,
                    TimeEstimate = j.TimeEstimate,
                    TimeActual = j.TimeActual,
                    CostEstimate = j.CostEstimate,
                    CostActual = j.CostActual,

                    SpareTotal = j.SpareTotal,
                    LocalTotal = j.LocalTotal,
                    SubletTotal = j.SubletTotal,
                    GrandTotal = j.GrandTotal,
                    TotalAmountPaid = j.TotalAmountPaid,
                    SubletDescription = j.SubletDescription,
                    LabourTotal = j.LabourTotal,

                    Tables = tables
                };
                var res = await _customerService.addJobCard(request);

                return Ok(ApiResponse<string>.Created());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }

        }



        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.GetServiceTimeline)]
        public async Task<IActionResult> GetServiceTimeline([FromQuery] Guid salesMasterId)
        {
            try
            {
                var l = await _customerService.GetServiceTimelinedb(salesMasterId);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }





        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.jobCard.jobCardReport)]
        public async Task<IActionResult> jobCardReport([FromQuery] JobCardMasterReportRequest m)
        {
            try
            {
                var l = await _customerService.jobCardReportdb(m);
                return Ok(ApiResponse<JObCardReportResponse>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.Webhook)]
        public async Task<IActionResult> Webhook([FromBody] VerifyWebhook input,
            [FromQuery(Name = "hub.mode")] string mode,
            [FromQuery(Name = "hub.verify_token")] string token)
        {
            try
            {
                const string secretKey = "Gromax@123";

                if (string.IsNullOrWhiteSpace(token) || token != secretKey || mode != "subscribe")
                {
                    _logger.LogWarning("Invalid API Key. Received: {ApiKey}", token);

                    return Unauthorized(ApiResponse<string>.Unauthorized("Invalid API Key"));

                }
                _logger.LogInformation("Request Save : {Request}", input);

                var l = await _customerService.addOnlineEnqdb(input);
                if (l > 0)
                {
                    return Ok(ApiResponse<string>.Created());
                }
                else
                {
                    _logger.LogError("Failed to insert online enquiry details into database.");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        ApiResponse<string>.Fail("Failed to save enquiry details. Please try again later."));
                }
                //_logger.LogInformation("Response : {Response}",
                //    JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Webhook API");

                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail("Failed"));
            }
        }



        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.jobCard.getJobCardById)]
        public async Task<IActionResult> getJobCardById([FromQuery][Required] string jobCardMasterId)
        {
            try
            {
                var l = await _customerService.getJobCardByIddb(jobCardMasterId);
                return Ok(ApiResponse<ResponseJobCardMaster>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Authorize(Roles = "Dealer")]
        [Route(GromaxMobileApis.Utilities.customerServices.jobCard.updateJobCard)]
        public async Task<IActionResult> updateJobCard([FromBody] JobCardMaster j)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(j.Id))
                {
                    return BadRequest(
                        ApiResponse<string>.BadRequest("Invalid Id.")
                    );
                }
                JobCardDataTables tables = new JobCardDataTables();
                try
                {
                    tables = UtilityFunctions.UpdateJobCardDataTables(j);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.BadRequest(ex.Message));
                }
                UpdateJobCardRequest request = new UpdateJobCardRequest()
                {
                    JobCardMasterId = Guid.TryParse(j.Id, out var id) ? id : (Guid?)null,
                    SalesMasterId = j.SalesMasterId,
                    ChassiNumber = j.ChassiNumber,
                    JobCardDate = j.JobCardDate,
                    JobCardType = j.JobCardType,
                    Fuel = j.Fuel,
                    DealerName = j.DealerName,
                    CustomerName = j.CustomerName,
                    CustomerAddress = j.CustomerAddress,
                    TractorSlNo = j.TractorSlNo,
                    RegnNo = j.RegnNo,
                    DateOfSale = j.DateOfSale,
                    WorkDoneBy = j.WorkDoneBy,
                    MobileNo = j.MobileNo,
                    AlternateMobileNo = j.AlternateMobileNo,

                    FrontTyrePressureLeft = j.FrontTyrePressureLeft,
                    FrontTyrePressureRight = j.FrontTyrePressureRight,
                    RearTyrePressureLeft = j.RearTyrePressureLeft,
                    RearTyrePressureRight = j.RearTyrePressureRight,

                    Hours = j.Hours,
                    TimeEstimate = j.TimeEstimate,
                    TimeActual = j.TimeActual,
                    CostEstimate = j.CostEstimate,
                    CostActual = j.CostActual,

                    SpareTotal = j.SpareTotal,
                    LocalTotal = j.LocalTotal,
                    SubletTotal = j.SubletTotal,
                    SubletDescription = j.SubletDescription,

                    GrandTotal = j.GrandTotal,
                    TotalAmountPaid = j.TotalAmountPaid,
                    LabourTotal = j.LabourTotal,

                    Tables = tables
                };
                var res = await _customerService.updateJobCard(request);

                return Ok(ApiResponse<string>.Created());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }

        }




        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.ntirList)]
        public async Task<IActionResult> NTIRPendingList([FromQuery] NTIRListResponse r)
        {
            try
            {
                var l = await _customerService.getNTIRPendingList(r);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.addNTIR)]
        public async Task<IActionResult> addNTIR([FromBody] NTIRRequest m)
        {
            try
            {
                DataTable p = new DataTable();
                try
                {
                    p = GromaxMobileApis.Utilities.UtilityFunctions.GetNTIRInspectionItemsDataTable(m.fields);
                }
                catch
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.BadRequest("DT Convert Error"));
                }
                var r = await _customerService.addNTIRdb(m, p);

                if (r <= 0)
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ApiResponse<string>.Fail("Unable to save NTIR."));

                return Ok(ApiResponse<string>.Created());
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message));
            }
        }



        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.getNTIRByID)]
        public async Task<IActionResult> getNTIRByID([FromQuery][Required] Guid Id)
        {
            try
            {
                var l = await _customerService.getNTIRByIDdb(Id);
                return Ok(ApiResponse<NTIRMasterReponse>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.updateNTIR)]
        public async Task<IActionResult> updateNTIR([FromBody] NTIRRequest m)
        {
            try
            {
                DataTable p = new DataTable();
                try
                {
                    p = GromaxMobileApis.Utilities.UtilityFunctions.GetNTIRInspectionItemsDataTable(m.fields);
                }
                catch
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.BadRequest("DT Convert Error"));
                }
                var r = await _customerService.updateNTIRdb(m, p);

                if (r <= 0)
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ApiResponse<string>.Fail("Unable to save NTIR."));

                return Ok(ApiResponse<string>.Created());
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.uploadNTIRImage)]
        public async Task<IActionResult> uploadNTIRImage([FromForm] IFormFile file)
        {
            try
            {
                var fileName = await _getFileName._getFileNamev1(file, "NTIR");
                return Ok(ApiResponse<string>.Success(fileName));
            }
            catch
            {
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.BadRequest("Image Upload Error"));
            }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.addPDIv1)]
        public async Task<IActionResult> addPDIv1([FromBody] NTIRRequest m)
        {
            try
            {
                DataTable p = new DataTable();
                try
                {
                    p = GromaxMobileApis.Utilities.UtilityFunctions.GetNTIRInspectionItemsDataTable(m.fields);
                }
                catch
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.BadRequest("DT Convert Error"));
                }
                var r = await _customerService.addPDIdbv1(m, p);

                if (r <= 0)
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ApiResponse<string>.Fail("Unable to save PDI."));

                return Ok(ApiResponse<string>.Created());
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.updatePDI)]
        public async Task<IActionResult> updatePDI([FromBody] NTIRRequest m)
        {
            try
            {
                DataTable p = new DataTable();
                try
                {
                    p = GromaxMobileApis.Utilities.UtilityFunctions.GetNTIRInspectionItemsDataTable(m.fields);
                }
                catch
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.BadRequest("DT Convert Error"));
                }
                var r = await _customerService.updatePDIdb(m, p);

                if (r <= 0)
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ApiResponse<string>.Fail("Unable to save PDI."));

                return Ok(ApiResponse<string>.Created());
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.getPDIByID)]
        public async Task<IActionResult> getPDIByID([FromQuery][Required] Guid Id)
        {
            try
            {
                var l = await _customerService.getPDIByIDdb(Id);
                return Ok(ApiResponse<NTIRMasterReponse>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.MetaLead)]
        public async Task<IActionResult> MetaLead(
        [FromBody] MetaLeadRequest input,
        [FromQuery] string token)
        {
            try
            {
                const string secretKey = "Gromax@123";

                // Token validation
                if (string.IsNullOrWhiteSpace(token) || token != secretKey)
                {
                    _logger.LogWarning(
                        "Invalid API Token received for Meta Lead Webhook: {Token}",
                        token);

                    return Unauthorized(
                        ApiResponse<string>.Unauthorized("Invalid API Token"));
                }

                _logger.LogInformation(
                   "Meta Lead Webhook Request Received: {@Request}",
                   input);
                var l = await _customerService.AddMetaLead(input);

                // Request logging


                return Ok(
                    ApiResponse<string>.Success("Meta lead received successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error in Meta Lead Webhook API");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.DealerMaster.addDealerMaster)]
        public async Task<IActionResult> addDealerMaster([FromBody] DealerRequestModel m)
        {
            try
            {
                var r = await _customerService.addDealerMasterdb(m);

                if (r <= 0)
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ApiResponse<string>.Fail("Unable to save Dealer Master."));

                return Ok(ApiResponse<string>.Created());
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.jobCard.getOpenJobCard)]
        public async Task<IActionResult> getOpenJobCard([FromQuery][Required] int pageNo)
        {
            try
            {
                var l = await _customerService.getOpenJobCarddb(pageNo);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }





        [HttpPost]
        [Authorize(Roles = "Dealer")]
        [Route(GromaxMobileApis.Utilities.customerServices.invoice.generateService)]
        public async Task<IActionResult> generateServiceInvoice([FromBody][Required] List<Guid> Ids)
        {
            try
            {
                DataTable dt = new DataTable();
                var res = string.Empty;
                try
                {
                    dt = UtilityFunctions.ConvertIdsToDataTable(Ids);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.Fail(ex.Message));
                }

                try
                {
                    res = await _customerService.generateServiceInvoicedb(dt);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.ErrorWithErrorCode(ex.Message, 500, "500002"));
                }

                if (!string.IsNullOrWhiteSpace(res))
                {
                    var invoiceModel = await _customerService.getServiceInvByIddb(Guid.Parse(res), dt);
                    ////TEstingConde
                    //if (invoiceModel != null)
                    //    return Ok(ApiResponse<ServiceInvoiceResponse>.Success(invoiceModel));
                    ////TEstingConde

                    string fileName = await _pdfGenerator.GenerateAndSave(invoiceModel, "FreeServiceInv");
                    //string fileName = "http//Testing.pdf";
                    int urlUpdate = 0;
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        urlUpdate = await _customerService.updateServiceInvUrldb(Guid.Parse(res), fileName);

                        if (urlUpdate > 0)
                            return Ok(ApiResponse<string>.Success(fileName));
                        else
                            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail("Failed Update Invoice Url"));
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.Fail("Failed to save Pdf"));
                    }


                    //byte[] pdfBytes = await _pdfGenerator.Generate(invoiceModel.Master, invoiceModel.Items);
                    //string fileName = $"{invoiceModel.Master.InvoiceNo.Replace("/", "_")}.pdf";
                    //return File(pdfBytes, "application/pdf", fileName);

                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail("Something went wrong"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }



        [HttpGet]
        [Authorize(Roles = "Dealer")]
        [Route(GromaxMobileApis.Utilities.customerServices.jobCard.getFreeServiceClosedJobCard)]
        public async Task<IActionResult> getFreeServiceClosedJobCard()
        {
            try
            {
                var l = await _customerService.getFreeServiceClosedJobCarddb();
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpGet]
        [Authorize(Roles = "Dealer")]
        [Route(GromaxMobileApis.Utilities.customerServices.GetPendingServiceInvoiceInstallation)]
        public async Task<IActionResult> GetPendingServiceInvoiceInstallation()
        {
            try
            {
                var l = await _customerService.GetPendingServiceInvoiceInstallationdb();
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }



        [HttpPost]
        [Authorize(Roles = "Dealer")]
        [Route(GromaxMobileApis.Utilities.customerServices.invoice.generateInstallationInvoice)]
        public async Task<IActionResult> generateInstallationInvoice([FromBody][Required] List<Guid> Ids)
        {
            try
            {
                DataTable dt = new DataTable();
                try
                {
                    dt = UtilityFunctions.ConvertIdsToDataTable(Ids);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.Fail(ex.Message));
                }
                var res = string.Empty;
                try
                {
                    res = await _customerService.generateInstallationInvoicedb(dt);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.ErrorWithErrorCode(ex.Message, 500, "500002"));
                }

                if (!string.IsNullOrWhiteSpace(res))
                {
                    var invoiceModel = await _customerService.getServiceInvByIddb(Guid.Parse(res), dt);
                    string fileName = await _pdfGenerator.GenerateAndSave(invoiceModel, "IntallationInv");
                    //string fileName = "http//Testing.pdf";
                    int urlUpdate = 0;
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        urlUpdate = await _customerService.updateServiceInvUrldb(Guid.Parse(res), fileName);

                        if (urlUpdate > 0)
                            return Ok(ApiResponse<string>.Success(fileName));
                        else
                            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail("Failed Update Invoice Url"));
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.Fail("Failed to save Pdf"));
                    }


                    //byte[] pdfBytes = await _pdfGenerator.Generate(invoiceModel.Master, invoiceModel.Items);
                    //string fileName = $"{invoiceModel.Master.InvoiceNo.Replace("/", "_")}.pdf";
                    //return File(pdfBytes, "application/pdf", fileName);

                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail("Something went wrong"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.invoice.getInvoiceList)]
        public async Task<IActionResult> getInvoiceList([FromQuery] ServiceInvoiceListResp m)
        {
            try
            {
                var l = await _customerService.getInvoiceListdb(m);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.updateReimbursementPaymentStatus)]
        public async Task<IActionResult> UpdateReimbursementPaymentStatus([FromBody] UpdateReimbursementStatusRequest m)
        {
            try
            {
                var r = await _customerService.UpdateReimbursementPaymentStatusDb(m);

                if (r <= 0)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ApiResponse<string>.Fail("Unable to update reimbursement payment status."));
                }

                return Ok(ApiResponse<string>.Success("Reimbursement payment status updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message));
            }
        }



        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.reimbursement.getReimbursementList)]
        public async Task<IActionResult> getReimbursementList([FromQuery][Required] string dealerCode)
        {
            try
            {
                var l = await _customerService.getReimbursementListdb(dealerCode);
                return Ok(ApiResponse<IEnumerable<ReimbursementScoreMasterModel>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Authorize(Roles = "Dealer,Service CCM")]
        [Route(GromaxMobileApis.Utilities.customerServices.mechanicServices.insertMechanic)]
        public async Task<IActionResult> InsertMechanic([FromBody] MechanicCreateRequest m)
        {
            try
            {
                var r = await _customerService.InsertMechanicDb(m);

                if (r <= 0)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ApiResponse<string>.Fail("Unable to insert mechanic."));
                }

                return Ok(
                    ApiResponse<string>.Success(
                        "Mechanic inserted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.mechanicServices.getMechanicList)]
        public async Task<IActionResult> getMechanicList([FromQuery] MechanicApprovalFilterRequest m)
        {
            try
            {
                var l = await _customerService.getMechanicListdb(m);
                return Ok(ApiResponse<IEnumerable<MechanicMasterDataModel>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Authorize(Roles = "Service CCM")]
        [Route(GromaxMobileApis.Utilities.customerServices.mechanicServices.approvalstatus)]
        public async Task<IActionResult> approvalstatus([FromBody] MechanicApprovalRequest m)
        {
            try
            {
                var r = await _customerService.MechanicApprovalDb(m);

                if (r <= 0)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ApiResponse<string>.Fail("Unable to insert mechanic."));
                }

                return Ok(
                    ApiResponse<string>.Success(
                        "Mechanic inserted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.mechanicServices.getMechanicsPendingList)]
        public async Task<IActionResult> getMechanicsPendingList()
        {
            try
            {
                var l = await _customerService.getMechanicsPendingListDb();
                return Ok(ApiResponse<IEnumerable<MechanicMaster>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Authorize(Roles = "Service CCM")]
        [Route(GromaxMobileApis.Utilities.customerServices.mechanicServices.updateMechanic)]
        public async Task<IActionResult> updateMechanic([FromBody] MechanicUpdateRequest m)
        {
            try
            {
                var r = await _customerService.UpdateMechanicDb(m);

                if (r <= 0)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ApiResponse<string>.Fail("Unable to update mechanic."));
                }

                return Ok(
                    ApiResponse<string>.Success(
                        "Mechanic update successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message));
            }
        }




        [HttpGet]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.DealerMaster.allDealerList)]
        public async Task<IActionResult> allDealerList([FromQuery] int status = 2)
        {
            try
            {
                var l = await _customerService.getAllDealerDb(status);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.reimbursement.insertReimbursementScore)]
        public async Task<IActionResult> insertReimbursementScore([FromBody] ReimbursementRequest m)
        {
            try
            {
                ReimbursementDataTables dataTables = new ReimbursementDataTables();
                try
                {
                    dataTables = UtilityFunctions.CreateReimbursementDataTables(m);
                }
                catch
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.BadRequest("Unable to proceed dt conversion."));
                }
                int r = 0;
                try
                {
                    r = await _customerService.insertReimbursementScoreDb(m, dataTables);
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 500001)
                        return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.SqlExceptionWithErrorCode(ex.Message));
                }
                if (r <= 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail("Unable to Add Reimbursement Score."));
                }

                return Ok(
                    ApiResponse<string>.Success("Reimbursement Score update successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.DealerMaster.addBaseLocation)]
        public async Task<IActionResult> addBaseLocation([FromBody] EmployeeBaseLocationRequest m)
        {
            try
            {
                var result = await _db.addBaseLocationdb(m);
                if (result > 0)
                    return Ok(ApiResponse<string>.Created());
                return NotFound(ApiResponse<string>.NotFound("Record not found."));

            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail(ex.Message));
            }



        }





        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.pdiReport)]
        public async Task<IActionResult> pdiReport([FromQuery] PdiReportReqModel m)
        {
            try
            {
                var l = await _customerService.getPdiReportDb(m);
                return Ok(ApiResponse<PdiReportResponse>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.ntirReport)]
        public async Task<IActionResult> ntirReport([FromQuery] NtirReportReqModel m)
        {
            try
            {
                var l = await _customerService.getNtirReportDb(m);
                return Ok(ApiResponse<NtirReportResponse>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.jobCard.addJobCardPdf)]
        public async Task<IActionResult> addJobCardPdf([FromQuery][Required] string jobCardMasterId)
        {
            try
            {
                var data = await _customerService.getJobCardByIddb(jobCardMasterId);
                var master = data?.resJobCardMaster?.FirstOrDefault();
                var URL = master.pdfURL;

                if (master == null)
                    return NotFound(ApiResponse<string>.NotFound("Job card not found"));

                if (!string.IsNullOrEmpty(URL))
                    return StatusCode(500, ApiResponse<string>.Fail("Job card URL already exists"));

                string path = await _JobCardpdfGenerator.GenerateAndSave(data, "JobCardPdf");
                int res = await _customerService.updateJobCardPdfURLdb(jobCardMasterId, path);
                if (res > 0)
                    return Ok(ApiResponse<string>.Success(path));
                else
                    return NotFound(ApiResponse<string>.NotFound("Job card not found"));
            }
            catch
            {
                return StatusCode(500, ApiResponse<string>.Fail("Something wrong."));
            }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.jobCard.removeJobCardDraft)]
        public async Task<IActionResult> removeJobCardDraft([FromBody][Required] JobCardCancelRequestModel m)
        {
            try
            {
                var r = await _customerService.removeJobCardDraftdb(m.JobCardMasterId);
                if (r <= 0)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ApiResponse<string>.Fail("Unable to Delete Job Card."));
                }

                return Ok(
                    ApiResponse<string>.Success(
                        "Jobcard Removed successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.mechanicServices.getMechanicDropdownList)]
        [AllowAnonymous]
        public async Task<IActionResult> getMechanicDropdownList()
        {
            try
            {
                var l = await _customerService.getMechanicDropdownListdb();
                return Ok(ApiResponse<MechanicDropdownResponse>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.jobCard.GetJobCardPdf)]
        public async Task<IActionResult> GetJobCardPdf(string jobCardMasterId)
        {
            try
            {
                var data = await _customerService.getJobCardByIddb(jobCardMasterId);
                var master = data?.resJobCardMaster?.FirstOrDefault();
                if (master == null)
                    return NotFound(ApiResponse<string>.NotFound("Job card not found"));

                byte[] pdf = _JobCardpdfGenerator.Generate(data);
                string fileName = $"JobCard_{master.JobCardNo?.Replace("/", "_")}.pdf";

                return File(pdf, "application/pdf", fileName);
            }
            catch
            {
                return StatusCode(500, ApiResponse<string>.Fail("Something wrong."));
            }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.InstallationMaster.updateInstallationApproval)]
        public async Task<IActionResult> updateInstallationApproval(
     [FromBody] InstallationApprovalRequest m)
        {
            try
            {
                var r = await _customerService.updateInstallationApprovaldb(m);

                if (r <= 0)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ApiResponse<string>.Fail(
                            "Unable to update installation approval status."));
                }

                string message = m.ApprovalStatus == 1
                    ? "Installation approved successfully."
                    : "Installation rejected successfully.";

                return Ok(ApiResponse<string>.Success(message));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(
                        "An error occurred while updating the installation approval status."));
            }
        }



        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.FTR.GetChassisDetailsForFTR)]
        public async Task<IActionResult> GetChassisDetailsForFTR([FromQuery] GetFTRAllDataRequestModel m)
        {
            try
            {
                IEnumerable<GetChassisForFTRResponseModel> data = await _customerService.GetChassisDetailsForFTRdb(m);
                return Ok(ApiResponse<IEnumerable<GetChassisForFTRResponseModel>>.Success(data));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail("Something wrong."));
            }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.FTR.addFTR)]
        [Authorize(Roles = "Dealer")]
        public async Task<IActionResult> addFTR([FromBody] AddFieldTechRequestModel m)
        {
            try
            {
                int result = 0;
                try
                {
                    result = await _customerService.addFTRdb(m);
                }
                catch (SqlException ex)
                {
                    if (ex?.Number == 500001)
                    {
                        return StatusCode(500, ApiResponse<string>.SqlExceptionWithErrorCode(ex.Message));

                    }
                }
                if (result > 0)
                    return Ok(ApiResponse<string>.Created());
                return NotFound(ApiResponse<string>.NotFound("Record not found."));

            }
            catch
            {
                return StatusCode(500, ApiResponse<string>.Fail("Something Wrong"));
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.FTR.updateFTR)]
        [Authorize(Roles = "Service CCM")]
        public async Task<IActionResult> updateFTR([FromBody] UpdateFTRRequestModel m)
        {
            try
            {
                int result = 0;
                try
                {
                    result = await _customerService.updateFTRdb(m);
                }
                catch (SqlException ex)
                {
                    if (ex?.Number == 500001)
                    {
                        return StatusCode(500, ApiResponse<string>.SqlExceptionWithErrorCode(ex.Message));

                    }
                }
                if (result > 0)
                    return Ok(ApiResponse<string>.Created());
                return NotFound(ApiResponse<string>.NotFound("Record not found."));

            }
            catch
            {
                return StatusCode(500, ApiResponse<string>.Fail("Something Wrong"));
            }
        }


        //Uncommented the `getFTRById` API and added the `JobCardId` parameter to fetch FTR details based on the selected Job Card.
        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.FTR.getFTRById)]
        public async Task<IActionResult> getFTRById([FromQuery] string Ftrid, string JobCardId)
        {
            try
            {
                Guid? FTR_ID = null;
                Guid? JOB_ID = null;
                if (!string.IsNullOrEmpty(Ftrid))
                {
                    FTR_ID = Guid.Parse(Ftrid);
                }
                if (!string.IsNullOrEmpty(JobCardId))
                {
                    JOB_ID = Guid.Parse(JobCardId);
                }
                if (FTR_ID is null && JOB_ID is null)
                {
                    return BadRequest(ApiResponse<string>.BadRequest("Either FTR ID or Job Card ID is required. Please provide at least one."));
                }
                var l = await _customerService.getFtrById(FTR_ID, JOB_ID);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.FTR.getFTRData)]
        public async Task<IActionResult> getFTRData([FromQuery] GetFTRAllDataRequestModel m)
        {
            try
            {
                var l = await _customerService.getFtrAlldataDb(m);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }










        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.Other.imageSave)]
        public async Task<IActionResult> imageSave([Required] IFormFile file, [Required] string FileName)
        {
            try
            {
                try
                {
                    _getFileName.ImageValidation(file, FileName);
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponse<string>.BadRequest(ex.Message));
                }
                string fileUrl = await _getFileName._getFileNamev1(file, FileName);
                return Ok(ApiResponse<string>.Success(fileUrl));
            }
            catch
            {
                return StatusCode(
               StatusCodes.Status500InternalServerError,
               ApiResponse<string>.BadRequest("An error occurred while uploading the image."));
            }
        }



        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.reimbursement.getReimbursementMasterListApps)]
        public async Task<IActionResult> getReimbursementMasterListApps()
        {
            try
            {
                var l = await _customerService.getReimbursementAppListdb();
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.mechanicServices.getMechanicCountOnnDealer)]
        public async Task<IActionResult> getMechanicCountOnnDealer([FromQuery][Required] string dealerCode)
        {
            try
            {
                var l = await _customerService.getMechanicCountOnDealerDb(dealerCode);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.reimbursement.getItemScoreReport)]
        public async Task<IActionResult> getItemScoreReport([FromQuery] string dealerCode, [Required] int halfYearType, [Required] int year)
        {
            try
            {
                var l = await _customerService.getItemScoreReportDb(dealerCode, halfYearType, year);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        /// <summary>
        /// Spare parts
        /// </summary>
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.spareParts.saveSparePartsOrder)]
        [Authorize(Roles = "Dealer")]
        public async Task<IActionResult> SaveSparePartsOrder([FromBody] SparePartsOrderRequest m)
        {
            try
            {
                SparePartsOrderResponse result = new SparePartsOrderResponse();

                try
                {
                    result = await _customerService.SaveSparePartsOrder(m);
                }
                catch (SqlException ex)
                {
                    if (ex?.Number == 500001)
                    {
                        return StatusCode(
                            500,
                            ApiResponse<string>.SqlExceptionWithErrorCode(ex.Message)
                        );
                    }
                }

                if (result != null && result.StatusCode > 0)
                    return Ok(ApiResponse<string>.Created());

                return NotFound(
                    ApiResponse<string>.NotFound("Record not found.")
                );
            }
            catch
            {
                return StatusCode(
                    500,
                    ApiResponse<string>.Fail("Something Wrong")
                );
            }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.spareParts.updateSparePartsOrderPayment)]
        [Authorize(Roles = "Dealer")]
        public async Task<IActionResult> UpdateSparePartsOrderPayment([FromBody] UpdateSparePartsPaymentRequest m)
        {
            try
            {
                if (m == null)
                {
                    return BadRequest(
                        ApiResponse<string>.BadRequest(
                            "Request data is required."
                        )
                    );
                }

                if (string.IsNullOrWhiteSpace(m.OrderId))
                {
                    return BadRequest(
                        ApiResponse<string>.BadRequest(
                            "OrderId is required."
                        )
                    );
                }

                if (!Guid.TryParse(m.OrderId, out _))
                {
                    return BadRequest(
                        ApiResponse<string>.BadRequest(
                            "Invalid OrderId."
                        )
                    );
                }

                UpdateSparePartsPaymentResponse result = null;

                try
                {
                    result = await _customerService.UpdateSparePartsOrderPayment(m);
                }
                catch (SqlException ex)
                {
                    if (ex?.Number == 500001)
                    {
                        return StatusCode(
                            500,
                            ApiResponse<string>.SqlExceptionWithErrorCode(
                                ex.Message
                            )
                        );
                    }

                    return StatusCode(
                        500,
                        ApiResponse<string>.Fail(
                            "Database error occurred."
                        )
                    );
                }

                if (result == null)
                {
                    return StatusCode(
                        500,
                        ApiResponse<string>.Fail(
                            "No response received from database."
                        )
                    );
                }

                if (result.StatusCode > 0)
                {
                    return Ok(ApiResponse<UpdateSparePartsPaymentResponse>.Success(result));
                }

                return BadRequest(ApiResponse<UpdateSparePartsPaymentResponse>.Fail(result.Message));
            }
            catch
            {
                return StatusCode(
                    500,
                    ApiResponse<string>.Fail(
                        "Something Wrong"
                    )
                );
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.spareParts.getSparePartsOrderDropdown)]
        //[Authorize(Roles = "Dealer")]
        public async Task<IActionResult> GetSparePartsOrderDropdown()
        {
            try
            {
                var result = await _customerService.GetSparePartsOrderDropdown();

                if (result != null && result.Any())
                {
                    return Ok(
                        ApiResponse<List<SparePartsOrderDropdownResponse>>.Success(result)
                    );
                }

                return NotFound(
                    ApiResponse<string>.NotFound("Record not found.")
                );
            }
            catch (SqlException ex)
            {
                if (ex?.Number == 500001)
                {
                    return StatusCode(
                        500,
                        ApiResponse<string>.SqlExceptionWithErrorCode(ex.Message)
                    );
                }

                return StatusCode(
                    500,
                    ApiResponse<string>.Fail("Database error occurred.")
                );
            }
            catch
            {
                return StatusCode(
                    500,
                    ApiResponse<string>.Fail("Something Wrong")
                );
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.spareParts.getSparePartsPagination)]
        //[Authorize(Roles = "Dealer")]
        public async Task<IActionResult> getSparePartsPagination(int pageNumber = 1, int pageSize = 20, string partCodeOrName = null, string category = null)
        {
            try
            {
                var result = await _customerService.getSparePartsPaginationdb(
                    pageNumber,
                    pageSize,
                    partCodeOrName,
                    category);

                return Ok(
                    ApiResponse<SparePartsPaginationResponse>.Success(result)
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message)
                );
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.spareParts.getSparePartsOrders)]
        //[Authorize(Roles = "Dealer")]
        public async Task<IActionResult> GetSparePartsOrders(
                 int pageNumber = 1,
                 int pageSize = 20,
                 string DealerCode = "All",
                 string StateName = "All",
                 string Duration = "this Month",
                 DateTime? StDate = null,
                 DateTime? EnDate = null,
                 string OrderNo = null,
                 int IsDownload = 0)
        {
            try
            {
                if (pageNumber <= 0)
                    return BadRequest(
                        ApiResponse<string>.Fail("PageNumber must be greater than zero.")
                    );

                if (pageSize <= 0)
                    return BadRequest(
                        ApiResponse<string>.Fail("PageSize must be greater than zero.")
                    );

                var result = await _customerService
     .GetSparePartsOrdersdb(pageNumber, pageSize, DealerCode, StateName, Duration, StDate, EnDate, OrderNo, IsDownload);

                if (result == null)
                {
                    return NotFound(
                        ApiResponse<string>.NotFound("Record not found.")
                    );
                }

                return Ok(
                    ApiResponse<SparePartsOrderListResponse>.Success(result)
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message)
                );
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.spareParts.getSparePartsOrderDetails)]
        //[Authorize(Roles = "Dealer")]
        public async Task<IActionResult> GetSparePartsOrderDetails([FromQuery][Required] string orderId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orderId))
                {
                    return BadRequest(
                        ApiResponse<string>.Fail("OrderId is required.")
                    );
                }

                if (!Guid.TryParse(orderId, out _))
                {
                    return BadRequest(
                        ApiResponse<string>.Fail("OrderId must be a valid GUID.")
                    );
                }

                var result = await _customerService
                    .GetSparePartsOrderDetailsdb(orderId);

                if (result == null || !result.Any())
                {
                    return NotFound(
                        ApiResponse<string>.NotFound("Order details not found.")
                    );
                }

                return Ok(
                    ApiResponse<List<GetSparePartsOrderDetailResponse>>.Success(result)
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message)
                );
            }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.spareParts.updateSparePartsOrderItem)]
        [Authorize(Roles = "Dealer")]
        public async Task<IActionResult> UpdateSparePartsOrderItem([FromBody] UpdateSparePartsOrderItemRequest m)
        {
            try
            {
                UpdateSparePartsOrderItemResponse result;

                try
                {
                    result = await _customerService.UpdateSparePartsOrderItem(m);
                }
                catch (SqlException ex)
                {
                    if (ex?.Number == 500001)
                    {
                        return StatusCode(
                            500,
                            ApiResponse<string>.SqlExceptionWithErrorCode(ex.Message)
                        );
                    }

                    throw;
                }

                if (result != null && result.StatusCode > 0)
                {
                    return Ok(
                        ApiResponse<UpdateSparePartsOrderItemResponse>.Success(result)
                    );
                }

                return BadRequest(
                    ApiResponse<string>.BadRequest("Unable to update order item.")
                );
            }
            catch (FormatException)
            {
                return BadRequest(
                    ApiResponse<string>.BadRequest(
                        "Invalid OrderDetailId. OrderDetailId must be a valid GUID."
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message)
                );
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.spareParts.createSpareOrderInvoice)]
        [Authorize(Roles = "National Service Head")]
        public async Task<IActionResult> CreateSpareOrderInvoice([FromForm] CreateSpareOrderInvoiceRequest m)
        {
            try
            {
                if (!m.IsInvoicing)
                {
                    _getFileName.PdfValidation(
                        m.PDF,
                        "spare_Inv" + m.OrderId
                    );

                    m.PDFUrl = await _getFileName._getFileNamev1(
                        m.PDF,
                        "spare_Inv_" + m.OrderId
                    );
                }
                var result = await _customerService.CreateSpareOrderInvoice(m);

                if (result != null && result.StatusCode > 0)
                {
                    return Ok(
                        ApiResponse<CreateSpareOrderInvoiceResponse>.Success(result)
                    );
                }

                return BadRequest(
                    ApiResponse<string>.BadRequest(
                        result?.Message ?? "Invoice could not be created."
                    )
                );
            }
            catch (SqlException ex)
            {
                if (ex.Number == 500001)
                {
                    return StatusCode(
                        500,
                        ApiResponse<string>.SqlExceptionWithErrorCode(ex.Message)
                    );
                }

                return StatusCode(
                    500,
                    ApiResponse<string>.Fail(ex.Message)
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message)
                );
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.spareParts.GetSpareOrderInvoice)]

        public async Task<IActionResult> GetSpareOrderInvoice(string orderId)
        {
            try
            {
                var res = await _customerService.GetSpareOrderInvoice(orderId);

                if (res == null)
                {
                    return NotFound(ApiResponse<String>.NotFound("Invoice not found for this order."));
                }

                return Ok(ApiResponse<SpareOrderInvoiceResponse>.Success(res));

            }
            catch (SqlException ex)
            {
                if (ex.Number == 500001)
                {
                    return StatusCode(
                        500,
                        ApiResponse<string>.SqlExceptionWithErrorCode(ex.Message)
                    );
                }

                return StatusCode(
                    500,
                    ApiResponse<string>.Fail(ex.Message)
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message)
                );
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.warrantyClaim.getClosedJobCardList)]
        public async Task<IActionResult> GetClosedJobCardList([FromQuery] ClosedJobCardListRequest request)
        {
            try
            {
                var result = await _customerService.GetClosedJobCardListdb(request);

                if (result == null)
                {
                    return NotFound(
                        ApiResponse<string>.NotFound("Record not found.")
                    );
                }

                return Ok(
                    ApiResponse<IEnumerable<dynamic>>.Success(result)
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message)
                );
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.warrantyClaim.getSparePartListByJobCardId)]
        public async Task<IActionResult> getSparePartListByJobCardId([FromQuery][Required] Guid JobCardMasterId)
        {
            try
            {
                var result = await _customerService.getSparePartListByJobCardIddb(JobCardMasterId);

                if (result == null)
                {
                    return NotFound(
                        ApiResponse<string>.NotFound("Record not found.")
                    );
                }

                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message)
                );
            }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.warrantyClaim.saveWarrantyClaim)]
        public async Task<IActionResult> SaveWarrantyClaim([FromBody] WarrantyClaimRequest request)
        {
            try
            {
                var result = await _customerService.SaveWarrantyClaimdb(request);
                if (result < 1)
                    return BadRequest(ApiResponse<string>.BadRequest("Failed to create warranty claim. Please try again."));
                return Ok(ApiResponse<int>.Created());
            }
            catch (SqlException ex)
            {
                if (ex?.Number == 500001)
                {
                    return StatusCode(500, ApiResponse<string>.SqlExceptionWithErrorCode(ex.Message));
                }
                return StatusCode(500, ApiResponse<string>.Fail("Something Wrong."));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.warrantyClaim.getPendingWarrantyClaimList)]
        public async Task<IActionResult> getPendingWarrantyClaimList([FromQuery][Required] string ClaimStatus)
        {
            try
            {
                var result = await _customerService.getPendingWarrantyClaimListdb(ClaimStatus);

                if (result == null)
                {
                    return NotFound(
                        ApiResponse<string>.NotFound("Record not found.")
                    );
                }

                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message)
                );
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.warrantyClaim.getWarrantySparesByMasterId)]
        public async Task<IActionResult> getWarrantySparesByMasterId([FromQuery][Required] Guid WarrantyClaimMasterId)
        {
            try
            {
                var result = await _customerService.getWarrantySparesByMasterIddb(WarrantyClaimMasterId);

                if (result == null)
                {
                    return NotFound(
                        ApiResponse<string>.NotFound("Record not found.")
                    );
                }

                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message)
                );
            }
        }



        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.warrantyClaim.UpdateWarrantyClaimApproval)]
        public async Task<IActionResult> UpdateWarrantyClaimApproval([FromBody] WarrantyClaimApprovalRequest request)
        {
            try
            {
                var result = await _customerService.UpdateWarrantyClaimApprovaldb(request);
                if (result < 1)
                    return BadRequest(ApiResponse<string>.BadRequest("Failed to Approve warranty claim. Please try again."));
                return Ok(ApiResponse<int>.Created());
            }
            catch (SqlException ex)
            {
                if (ex?.Number == 500001)
                {
                    return StatusCode(500, ApiResponse<string>.SqlExceptionWithErrorCode(ex.Message));
                }
                return StatusCode(500, ApiResponse<string>.Fail("Something Wrong."));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.warrantyClaim.UpdateWarrantyClaim)]
        public async Task<IActionResult> UpdateWarrantyClaim([FromBody] UpdateWarrantyClaimRequest request)
        {
            try
            {
                var result = await _customerService.UpdateWarrantyClaimdb(request);
                if (result < 1)
                    return BadRequest(ApiResponse<string>.BadRequest("Failed to Update warranty claim. Please try again."));
                return Ok(ApiResponse<int>.Created());
            }
            catch (SqlException ex)
            {
                if (ex?.Number == 500001)
                {
                    return StatusCode(500, ApiResponse<string>.SqlExceptionWithErrorCode(ex.Message));
                }
                return StatusCode(500, ApiResponse<string>.Fail("Something Wrong."));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }





        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.reimbursement.getReimbursementScoreListById)]
        public async Task<IActionResult> getReimbursementScoreListById([FromQuery][Required] string ScoreId)
        {
            try
            {
                var l = await _customerService.getReimbursementScoreListById(ScoreId);
                return Ok(ApiResponse<getReimbursementScoreListByIdModel>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.reimbursement.getItemScoreApprocalPendList)]
        public async Task<IActionResult> getItemScoreApprocalPendList()
        {
            try
            {
                var l = await _customerService.getItemScoreApprovalPendingReportDb();
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.reimbursement.updateReimbursementScore)]
        public async Task<IActionResult> updateReimbursementScore([FromBody] UpdateReimbursementRequest m)
        {
            try
            {
                ReimbursementDataTables dataTables = new ReimbursementDataTables();
                try
                {
                    dataTables = UtilityFunctions.UpdateReimbursementDataTables(m);
                }
                catch
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.BadRequest("Unable to proceed dt conversion."));
                }
                int r = 0;
                try
                {
                    r = await _customerService.UpdateReimbursementScoreDb(m, dataTables);
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 500001)
                        return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.SqlExceptionWithErrorCode(ex.Message));
                }
                if (r <= 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail("Unable to Add Reimbursement Score."));
                }

                return Ok(
                    ApiResponse<string>.Success("Reimbursement Score update successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }



        [HttpPost]
        [Route(GromaxMobileApis.Utilities.customerServices.reimbursement.approveReimbursementScore)]
        public async Task<IActionResult> approveReimbursementScore([FromBody] ApproveReimbursementRequest m)
        {
            try
            {
                int r = 0;
                try
                {
                    r = await _customerService.ApproveReimbursementScoreDb(m);
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 500001)
                        return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.SqlExceptionWithErrorCode(ex.Message));
                }
                if (r <= 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail("Unable to Approve Reimbursement Score."));
                }
                //if (m.Status == 1)
                //{
                //    ItemScoreEmailDataModel emailData = new ItemScoreEmailDataModel();
                //    try
                //    {
                //        emailData = await _customerService.getItemScoreEmailRequiredDataDb(m.ApprovalId);
                //    }
                //    catch
                //    {
                //        var i = await _customerService.UpdateItemScoreMailStatusdb(false, "error found on getItemScoreEmailRequiredDataDb Function", m.ApprovalId);
                //        return Ok(ApiResponse<string>.Success("Reimbursement Score Approved successfully."));
                //    }
                //    if (emailData._To.Count < 1)
                //    {
                //        var i = await _customerService.UpdateItemScoreMailStatusdb(false, "Data not found in (TO) List", m.ApprovalId);
                //        return Ok(ApiResponse<string>.Success("Reimbursement Score Approved successfully."));
                //    }
                //    if (emailData._Cc.Count < 1)
                //    {
                //        var i = await _customerService.UpdateItemScoreMailStatusdb(false, "Data not found in (CC) List", m.ApprovalId);
                //        return Ok(ApiResponse<string>.Success("Reimbursement Score Approved successfully."));
                //    }
                //    bool result = false;
                //    try
                //    {
                //        string emailBody = EmailTemplateHelper.GenerateItemScoreEmailBody(emailData.ItemScoreEmail);

                //        // Send Email
                //        result = await EmailService.SendEmailAsync(new EmailService.EmailRequest
                //        {
                //            To = emailData._To,
                //            Cc = emailData._Cc,
                //            Bcc = emailData._Bcc.Count > 1 ? emailData._Bcc : new(),
                //            Subject = $"ITEM Score Assessment Report & Dealer Category - {emailData.ItemScoreEmail.AssessmentMonth}",
                //            Body = emailBody,
                //            IsHtml = true
                //        });
                //    }
                //    catch (Exception ex)
                //    {
                //        var i = await _customerService.UpdateItemScoreMailStatusdb(false, ex.Message, m.ApprovalId);
                //        return Ok(ApiResponse<string>.Success("Reimbursement Score Approved successfully."));
                //    }

                //    try
                //    {
                //        var i = await _customerService.UpdateItemScoreMailStatusdb(result, null, m.ApprovalId);
                //    }
                //    catch
                //    {
                //        return Ok(ApiResponse<string>.Success("Reimbursement Score Approved successfully."));
                //    }
                //}

                return Ok(ApiResponse<string>.Success("Reimbursement Score Approved successfully."));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpGet]
        [Route(GromaxMobileApis.Utilities.customerServices.warrantyClaim.GetWarrantyClaimsCount)]
        public async Task<IActionResult> GetWarrantyClaimsCount()
        {
            try
            {
                var result = await _customerService.GetWarrantyClaimsCountdb();

                if (result == null)
                {
                    return NotFound(
                        ApiResponse<string>.NotFound("Record not found.")
                    );
                }

                return Ok(ApiResponse<dynamic>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(ex.Message)
                );
            }
        }



        //[HttpPost("SendAssessmentEmail")]
        //public async Task<IActionResult> SendAssessmentEmail([FromBody] ItemScoreEmailModel model)
        //{
        //    try
        //    {
        //        if (model == null)
        //            return BadRequest("Invalid request.");

        //        // Generate Email Body
        //        string emailBody =
        //            EmailTemplateHelper.GenerateItemScoreEmailBody(model);

        //        // Send Email
        //        bool result = await EmailService.SendEmailAsync(
        //            new EmailService.EmailRequest
        //            {
        //                To = new List<string> { "ykp900155@gmail.com" },
        //                Subject = $"ITEM Score Assessment Report & Dealer Category - {model.AssessmentMonth}",
        //                Body = emailBody,
        //                IsHtml = true
        //            });

        //        if (!result)
        //        {
        //            return StatusCode(500, ApiResponse<string>.Fail("Email sending failed."));
        //        }

        //        return StatusCode(200, ApiResponse<string>.Success("Email sent successfully."));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ApiResponse<string>.Fail(ex.Message));
        //    }
        //}



        //[AllowAnonymous]
        //[HttpGet]
        //[Route(GromaxMobileApis.Utilities.customerServices.invoice.service)]
        //public async Task<IActionResult> DownloadInvoicePdf([FromQuery] Guid Id)
        //{
        //    //var invoiceModel = StaticInvoiceModel.GetSampleInvoice();
        //    try
        //    {
        //        var invoiceModel = await _customerService.getServiceInvByIddb(Id);

        //        byte[] pdfBytes = await _pdfGenerator.Generate(invoiceModel.Master, invoiceModel.Items);
        //        string fileName = $"{invoiceModel.Master.InvoiceNo.Replace("/", "_")}.pdf";
        //        return File(pdfBytes, "application/pdf", fileName);


        //        //string fileName = await _pdfGenerator.GenerateAndSave(invoiceModel.Master, invoiceModel.Items, "FreeServiceInv");
        //        //return Ok(ApiResponse<string>.Created());

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
        //    }
        //}




        //[AllowAnonymous]
        //[HttpGet("Api/Services/Webhook")]
        //public async Task<IActionResult> VerifyWebhook()
        //{
        //    try
        //    {
        //        var mode = HttpContext.Request.Query["hub.mode"];
        //        var token = HttpContext.Request.Query["hub.verify_token"];
        //        var challenge = HttpContext.Request.Query["hub.challenge"];

        //        VerifyWebhook model = null;
        //        if (!string.IsNullOrWhiteSpace(challenge))
        //        {
        //            try
        //            {
        //                var deserializer = new DeserializerBuilder()
        //                    .WithNamingConvention(UnderscoredNamingConvention.Instance)
        //                    .IgnoreUnmatchedProperties()
        //                    .Build();

        //                model = deserializer.Deserialize<VerifyWebhook>(challenge);

        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "YAML parsing failed for hub.challenge");
        //                return BadRequest(ApiResponse<string>.BadRequest("Invalid YAML challenge format."));
        //            }
        //        }
        //        if (model != null)
        //        {
        //            if (string.IsNullOrWhiteSpace(model.Name))
        //            {
        //                return BadRequest(ApiResponse<string>.BadRequest("Name is required and cannot be empty."));
        //            }

        //            if (string.IsNullOrWhiteSpace(model.Mobile))
        //            {
        //                return BadRequest(ApiResponse<string>.BadRequest("Mobile number is required and cannot be empty."));
        //            }

        //            if (string.IsNullOrWhiteSpace(model.City))
        //            {
        //                return BadRequest(ApiResponse<string>.BadRequest("City is required and cannot be empty."));
        //            }
        //        }

        //        _logger.LogInformation("Webhook Verify Request => mode: {mode}, token: {token}, challenge: {challenge}",
        //            mode, token, challenge);

        //        const string VERIFY_TOKEN = "Gromax@123";

        //        if (mode == "subscribe" && token == VERIFY_TOKEN)
        //        {
        //            var l = await _customerService.addOnlineEnqdb(model);
        //            if (l > 0)
        //            {
        //                return Ok(ApiResponse<dynamic>.Success(challenge));
        //            }
        //            else
        //            {
        //                _logger.LogError("Failed to insert online enquiry details into database.");

        //                return StatusCode(StatusCodes.Status500InternalServerError,
        //                    ApiResponse<string>.Fail("Failed to save enquiry details. Please try again later."));
        //            }
        //        }


        //        _logger.LogWarning("Webhook verification failed. Invalid token or mode.");
        //        return BadRequest(ApiResponse<string>.BadRequest("Invalid verify token"));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while verifying webhook");
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            ApiResponse<string>.Fail("Error verifying webhook"));
        //    }
        //}

        //[AllowAnonymous]
        //[HttpPost("Api/Services/Webhook")]
        //public IActionResult MultiEventGetResponse([FromBody] JsonElement input)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Webhook Verify Request : {Response}",
        //            input.GetRawText());

        //        var response = new
        //        {
        //            Status = true,
        //            Message = "EVENT_RECEIVED"
        //        };

        //        _logger.LogInformation("Response : {Response}",
        //            JsonConvert.SerializeObject(response));

        //        return Ok(ApiResponse<string>.Success("EVENT_RECEIVED"));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while receiving webhook event");
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            ApiResponse<string>.Fail("Failed"));
        //    }
        //}

    }
}
