using GromaxMobileApis.Interfaces;
using GromaxMobileApis.Models;
using GromaxMobileApis.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static GromaxMobileApis.Models.CircularModel;

namespace GromaxMobileApis.Controllers
{
    [ApiController]
    [Authorize]
    public class CircularController : ControllerBase
    {
        private readonly ICircularService _circularService;
        private readonly getFileName _getFileName;
        private readonly ILogger<CircularController> _logger;

        public CircularController(ICircularService circularService, getFileName getFileName, ILogger<CircularController> logger)
        {
            _circularService = circularService;
            _getFileName = getFileName;
            _logger = logger;
        }

        [HttpPost]
        [Route(ApiRoutes.Circular.getCircularsList)]
        public async Task<IActionResult> getCircularsList([FromBody] CircularListRequest m)
        {
            try
            {
                var l = await _circularService.getCircularsListdb(m);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPost]
        [Route(ApiRoutes.Circular.addCirculars)]
        public async Task<IActionResult> addCirculars([FromForm] AddCircularRequest m)
        {
            try
            {
                if (m.Files == null || !m.Files.Any(f => f != null && f.Length > 0))
                    return BadRequest(ApiResponse<string>.BadRequest("At least one file is required."));

                List<CircularFileModel> fileList;
                try
                {
                    fileList = await UploadFiles(m.Files);
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponse<string>.BadRequest(ex.Message));
                }

                var model = new AddCircularModel
                {
                    CircularName = m.CircularName.Trim(),
                    CircularDate = m.CircularDate,
                    Description = m.Description.Trim(),
                    FinancialYear = m.FinancialYear,
                    CircularType = m.CircularType,
                    FileUrls = fileList
                };

                var r = await _circularService.addCirculardb(model);

                if (r == -1)
                    return BadRequest(ApiResponse<string>.BadRequest("Circular with this name already exists."));
                if (r <= 0)
                    return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail("Unable to add circular."));

                return Ok(ApiResponse<string>.Success("Circular added successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in addCirculars");
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPost]
        [Route(ApiRoutes.Circular.getCircularDetails)]
        public async Task<IActionResult> getCircularDetails([FromBody] GetCircularDetailRequest m)
        {
            try
            {
                var d = await _circularService.getCircularDetaildb(m.CircularName);
                if (d == null)
                    return NotFound(ApiResponse<string>.NotFound("Circular not found."));

                return Ok(ApiResponse<object>.Success(d));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPost]
        [Route(ApiRoutes.Circular.updateCircular)]
        public async Task<IActionResult> updateCircular([FromForm] AddCircularRequest m)
        {
            try
            {
                var fileList = new List<CircularFileModel>();
                if (m.Files != null && m.Files.Any(f => f != null && f.Length > 0))
                {
                    try
                    {
                        fileList = await UploadFiles(m.Files);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ApiResponse<string>.BadRequest(ex.Message));
                    }
                }

                var model = new AddCircularModel
                {
                    CircularName = m.CircularName,
                    NewCircularName = string.IsNullOrWhiteSpace(m.NewCircularName) ? m.CircularName : m.NewCircularName.Trim(),
                    CircularDate = m.CircularDate,
                    Description = m.Description.Trim(),
                    FinancialYear = m.FinancialYear,
                    CircularType = m.CircularType,
                    FileUrls = fileList
                };

                var r = await _circularService.updateCirculardb(model);

                if (r == -1)
                    return NotFound(ApiResponse<string>.NotFound("Circular not found."));
                if (r == -2)
                    return BadRequest(ApiResponse<string>.BadRequest("Circular with this name already exists."));
                if (r <= 0)
                    return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail("Unable to update circular."));

                return Ok(ApiResponse<string>.Success("Circular updated successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in updateCircular");
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Route(ApiRoutes.Circular.getCircularDetails_app)]
        //[AllowAnonymous]
        public async Task<IActionResult> getCircularDetails_app([FromBody] GetCircularDetailAppRequest m)
        {
            try
            {
                var d = await _circularService.getCircularDetailAppDb(m);
                if (d == null)
                    return NotFound(ApiResponse<string>.NotFound("Circular not found."));

                return Ok(ApiResponse<IEnumerable<CircularDetailsAppResponse>>.Success(d));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpGet]
        [Route(ApiRoutes.Circular.dropDownList)]
        //[AllowAnonymous]
        public async Task<IActionResult> dropDownList()
        {
            try
            {
                var d = await _circularService.dropDownListDb();
                if (d == null)
                    return NotFound(ApiResponse<string>.NotFound("DropDownn List Not found."));

                return Ok(ApiResponse<CircularDropdownResponse>.Success(d));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
            }
        }

        //[HttpPost]
        //[Route(ApiRoutes.Circular.updateCircularSentStatus)]
        //public async Task<IActionResult> updateCircularSentStatus([FromBody] UpdateCircularSentStatusRequest m)
        //{
        //    try
        //    {
        //        if (m.Type != "DPs" && m.Type != "Company")
        //            return BadRequest(ApiResponse<string>.BadRequest("Invalid type."));

        //        var r = await _circularService.updateCircularSentStatusdb(m);
        //        if (r <= 0)
        //            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail("Failed to start campaign. Please try again."));

        //        return Ok(ApiResponse<string>.Success("Campaign started! Messages are being sent in background."));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
        //    }
        //}

        //[HttpPost]
        //[Route(ApiRoutes.Circular.getMsgSentReport)]
        //public async Task<IActionResult> getMsgSentReport([FromBody] GetMsgSentReportRequest m)
        //{
        //    try
        //    {
        //        var l = await _circularService.getMsgSentReportdb(m);
        //        return Ok(ApiResponse<IEnumerable<dynamic>>.Success(l));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.Fail(ex.Message));
        //    }
        //}

        // ─── Helper ───────────────────────────────────────────
        private async Task<List<CircularFileModel>> UploadFiles(List<IFormFile> files)
        {
            var list = new List<CircularFileModel>();
            foreach (var file in files.Where(f => f != null && f.Length > 0))
            {
                if (!Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                    throw new Exception($"{file.FileName} is not a PDF file.");

                var url = await _getFileName._getFileNamev1(file, "Circular");
                if (string.IsNullOrWhiteSpace(url))
                    throw new Exception($"Failed to upload {file.FileName}.");

                list.Add(new CircularFileModel { FileUrl = url, FileName = file.FileName });
            }
            return list;
        }
    }
}