using GromaxMobileApis.Interfaces;
using GromaxMobileApis.Models.DealerMaster;
using GromaxMobileApis.Models.Services;
using GromaxMobileApis.Models.VerifyWebhook;
using GromaxMobileApis.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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
        private readonly ILogger<CustomerServiceControlle> _logger;
        public CustomerServiceControlle(ICustomerService customerService, getFileName getFileName, IAzureStorageService azure, ILogger<CustomerServiceControlle> logger, ServiceInvoicePdfGenerator serviceInvoice)
        {
            _customerService = customerService;
            _getFileName = getFileName;
            _azure = azure;
            _logger = logger;
            _pdfGenerator = serviceInvoice;
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
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
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
                try
                {
                    dt = UtilityFunctions.ConvertIdsToDataTable(Ids);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.Fail(ex.Message));
                }
                var res = await _customerService.generateServiceInvoicedb(dt);
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
                var res = await _customerService.generateInstallationInvoicedb(dt);
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
        public async Task<IActionResult> getReimbursementList()
        {
            try
            {
                var l = await _customerService.getReimbursementListdb();
                return Ok(ApiResponse<IEnumerable<ReimbursementScoreMasterModel>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Authorize(Roles = "Dealer")]
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
                return Ok(ApiResponse<IEnumerable<MechanicMaster>>.Success(l));
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
