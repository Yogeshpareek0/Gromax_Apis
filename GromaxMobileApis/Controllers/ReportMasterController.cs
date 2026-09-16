using Google.Apis.Http;
using GromaxMobileApis.Interfaces;
using GromaxMobileApis.Models;
using GromaxMobileApis.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Azure;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GromaxMobileApis.Models.ReportMasterModel;
using static GromaxMobileApis.Utilities.WhatsappMessageSend;
using static System.Collections.Specialized.BitVector32;

namespace GromaxMobileApis.Controllers
{
    // [Route("Api/[Controller]")]
    [Authorize]
    [ApiController]

    public class ReportMasterController : ControllerBase
    {
        private readonly IReportMaster _reportMaster;
        private readonly DynamicExcel _dynamicExcel;
        private getFileName _getFilename;

        private readonly IDatabaseService _db;


        public ReportMasterController(IReportMaster reportMaster, DynamicExcel dynamic, getFileName getFilename, IDatabaseService db)
        {
            _reportMaster = reportMaster;
            _dynamicExcel = dynamic;
            _getFilename = getFilename;
            _db = db;
        }
        [HttpPost]
        [Route(GromaxReports.Preview)]
        public async Task<IActionResult> Preview([FromForm] IFormFile file, [FromForm] int UploadId)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Ok(ApiResponse<string>.BadRequest("File not found"));
                var master = await _reportMaster.GetUploadMasterById(UploadId);
                if (master == null)
                    return Ok(ApiResponse<string>.Fail("Upload configuration not found."));
                var mappings = await _reportMaster.GetMappings(UploadId);
                if (mappings == null || mappings.Count == 0)
                    return Ok(ApiResponse<string>.Fail("No column mappings defined for this upload."));
                var preview = _dynamicExcel.Preview(UploadId, file, mappings, master);
                return Ok(ApiResponse<UploadPreviewResponse>.Success(preview));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Route(GromaxReports.Commit)]
        public async Task<IActionResult> Commit([FromBody] UploadPreviewResponse model)
        {
            try
            {
                string createuser = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrWhiteSpace(model.StagingTable) || model.UploadId == 0 || model.Year == 0 || model.Month == 0)
                    return Ok(ApiResponse<string>.Fail("No staging data found in session. Please re-upload."));
                var MoveData = await _reportMaster.MoveDatadb(model.Month, model.Year, model.StagingTable, model.UploadId, createuser);
                if (MoveData > 0)
                    return Ok(ApiResponse<string>.Created());
                else
                    return Ok(ApiResponse<string>.Fail("Server Error"));

            }
            catch (Exception ex) { return Ok(ApiResponse<string>.Fail(ex.Message)); }
            finally
            {
                try
                {
                    _dynamicExcel.DropStaging(model.StagingTable);
                }
                catch
                {

                }
            }
        }

        [HttpPost]
        [Route(GromaxReports.Commitv1)]
        public async Task<IActionResult> Commitv1([FromBody] UploadPreviewResponse model)
        {
            try
            {
                string createuser = User.FindFirst(ClaimTypes.Name)?.Value;
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string LoginName = User.FindFirst("LoginName")?.Value;
                if (string.IsNullOrWhiteSpace(model.StagingTable) || model.UploadId == 0 || model.Year == 0 || model.Month == 0)
                    return Ok(ApiResponse<string>.Fail("No staging data found in session. Please re-upload."));
                var MoveData = await _reportMaster.MoveDatadbv1(model.Month, model.Year, model.StagingTable, model.UploadId, createuser, LoginPosition, LoginName);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(MoveData));

            }
            catch (Exception ex) { return Ok(ApiResponse<string>.Fail(ex.Message)); }
            finally
            {
                try
                {
                    _dynamicExcel.DropStaging(model.StagingTable);
                }
                catch
                {

                }
            }
        }
        [HttpGet]
        [Route(GromaxReports.GetAllDashboards)]

        public async Task<IActionResult> GetAllDashboards()
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _reportMaster.GetDashboardData(LoginPosition, username);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex) { return Ok(ApiResponse<string>.Fail(ex.Message)); }
        }
        [HttpPost]
        [Route(GromaxReports.GetDashboardsConfig)]
        public async Task<IActionResult> GetDashboardsConfig([FromBody] Dashboard model)
        {
            try
            {
                var result = await _reportMaster.GetDashboardConfigJson(model.Id);

                DashboardViewModel ress =
                    JsonConvert.DeserializeObject<DashboardViewModel>(result.ConfigJson);

                var dashboardResponse = new SendDashboardViewModel
                {
                    DashboardId = ress.DashboardId,
                    Name = ress.Name,
                    Theme = ress.Theme,
                    Sections = new List<SendDashboardSection>()
                };

                foreach (var section in ress.Sections)
                {
                    QueryRequestV5 reportconfi =
                        JsonConvert.DeserializeObject<QueryRequestV5>(section.ReportConfigJson);

                    var paramValues = new Dictionary<string, object>();

                    var dt = await _dynamicExcel.ExecuteQueryForExport(
                        reportconfi,
                        reportconfi.Parameters
                    );

                    var secc = new SendDashboardSection
                    {
                        SavedReportId = section.SavedReportId,
                        Id = section.Id,
                        ColumnWidth = section.ColumnWidth,
                        Title = section.Title,
                        Files = section.Files,
                        ReportConfigJson = section.ReportConfigJson,
                        Report = dt
                    };

                    dashboardResponse.Sections.Add(secc);
                }

                return Ok(ApiResponse<SendDashboardViewModel>.Success(dashboardResponse));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpPost]
        [Route(GromaxReports.GetDashboardsConfigv1)]
        public async Task<IActionResult> GetDashboardsConfigv1([FromBody] Dashboard model)
        {
            try
            {
                DashboardViewModel result = await _reportMaster.GetDashboardConfigJsonv1(model.Id);

                return Ok(ApiResponse<DashboardViewModel>.Success(result));
            }
            catch (Exception ex) { return Ok(ApiResponse<string>.Fail(ex.Message)); }
        }


        [HttpPost]
        [Route(GromaxReports.GetSavedReport)]
        public async Task<IActionResult> GetSavedReport([FromBody] GetSectionWise model)
        {
            try
            {
                var result = await _reportMaster.GetSavedReports(model.Id);
                //ExtractReportConfigModel ExtractCofigColumn = JsonConvert.DeserializeObject<ExtractReportConfigModel>(result.JsonRequest);
                QueryRequestV5 reportconfi = JsonConvert.DeserializeObject<QueryRequestV5>(result.JsonRequest);
                var dt = await _dynamicExcel.ExecuteQueryForExport(
                        reportconfi,
                        model.Parameters
                    );
                var secc = new SendDashboardSection
                {
                    Report = dt
                };
                //return Ok(ApiResponse<ExtractReportConfigModel>.Success(reportconfi));
                return Ok(ApiResponse<SendDashboardSection>.Success(secc));
            }
            catch (Exception ex) { return Ok(ApiResponse<string>.Fail(ex.Message)); }
        }


        [HttpGet]
        [Route(GromaxReports.GetUserDefineTable)]
        public async Task<IActionResult> GetUserDefineTable()
        {
            try
            {
                var result = await _reportMaster.GetUserDefineTablesdb();
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex) { return Ok(ApiResponse<string>.Fail(ex.Message)); }
        }

        [HttpPost]
        [Route(GromaxReports.GetUserDefineColumns)]
        public async Task<IActionResult> GetUserDefineColumns([FromBody] Dashboard model)
        {
            try
            {
                var result = await _reportMaster.GetUserDefineColumnsdb(model.Id);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex) { return Ok(ApiResponse<string>.Fail(ex.Message)); }
        }

        [HttpGet]
        [Route(GromaxReports.GetUserDefineProcedures)]
        public async Task<IActionResult> GetUserDefineProcedures()
        {
            try
            {
                var result = await _reportMaster.GetUserDefineProceduresdb();
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex) { return Ok(ApiResponse<string>.Fail(ex.Message)); }
        }

        [HttpPost]
        [Route(GromaxReports.GetUserDefineParameter)]
        public async Task<IActionResult> GetUserDefineParameter([FromBody] Dashboard model)
        {
            try
            {
                var result = await _reportMaster.GetUserDefineParameterdb(model.Id);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex) { return Ok(ApiResponse<string>.Fail(ex.Message)); }
        }
        [HttpPost]
        [Route(GromaxReports.InsertReportConfig)]
        public async Task<IActionResult> InsertReportConfig([FromBody] ReportsConfigModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.reportConfigJson) || string.IsNullOrEmpty(model.reportName))
                    return Ok(ApiResponse<string>.BadRequest("Config Not Found"));
                var result = await _reportMaster.InsertReportConfigdb(model);
                if (result > 1)
                    return Ok(ApiResponse<string>.Created());
                return Ok(ApiResponse<string>.Fail("Error"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail(ex.Message));
            }

        }

        [HttpPost]
        [Route(GromaxReports.SaveDashboardConfig)]
        public async Task<IActionResult> SaveDashboardConfig([FromBody] DashboardViewModel model)
        {
            try
            {
                if (model.Sections == null || string.IsNullOrEmpty(model.Name))
                    return Ok(ApiResponse<string>.BadRequest("Config Not Found"));
                var result = await _reportMaster.SaveDashboardConfigdb(model);
                if (result > 1)
                    return Ok(ApiResponse<string>.Created());
                return Ok(ApiResponse<string>.Fail("Error"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpGet]
        [Route(GromaxReports.GetUploadMasterList)]
        public async Task<IActionResult> GetUploadMasterList()
        {
            try
            {
                var result = await _reportMaster.GetUploadMasterListdb();
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex) { return Ok(ApiResponse<string>.Fail(ex.Message)); }
        }

        [HttpPost]
        [Route(GromaxReports.GetDropdownList)]
        public async Task<IActionResult> GetDropdownList([FromBody] DropdownList model)
        {
            try
            {
                //DropdownList mm = new DropdownList
                //{
                //    dropdowns = new List<dropdowns>()

                //};
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                sendDropdownres dd = new sendDropdownres
                {
                    statusCode = 200,
                    message = "Success",
                    Data = new Dictionary<string, object>()


                };

                foreach (var item in model.dropdowns)
                {
                    if (string.IsNullOrWhiteSpace(item.query))
                        return Ok(ApiResponse<string>.Fail("Not Found Query"));
                    if (item.paramName.ToLower() == "statename")
                    {
                        item.query = $"SELECT * FROM ( SELECT 'ALL' AS StateName \r\nUNION \r\nSELECT DISTINCT UPPER(StateName) FROM StateMaster a \r\ninner join (select distinct sss from fn_GetDealerByLoginAs('{LoginPosition}','{username}'))\r\nb on a.StateName=b.sss ) a ORDER BY CASE WHEN StateName = 'All' THEN 1 ELSE 2 END";
                    }
                    var s = item.query.Trim();
                    var up = s.ToUpperInvariant();
                    if (!up.StartsWith("SELECT"))
                        return Ok(ApiResponse<string>.Fail("Only SELECT allowed"));


                    var forbidden = new[] { "INSERT ", "UPDATE ", "DELETE ", "DROP ", "ALTER ", "TRUNCATE ", ";", "--", "/*" };
                    if (forbidden.Any(x => up.Contains(x)))
                        return Ok(ApiResponse<string>.Fail("Forbidden keywords in SQL"));

                    var list = await _reportMaster.Dropdownlistdb(up);

                    //dd.Data = new Dictionary<string, object>
                    //    {
                    //        { item.paramName, list }
                    //    };

                    dd.Data.Add(item.paramName, list);
                    //DropdownList dropdownList = new DropdownList()
                    //{
                    //    DropdownobjList = list,
                    //    Query = model.Query
                    //};
                }
                return Ok(dd);


            }
            catch (Exception ex)
            { return Ok(ApiResponse<string>.Fail(ex.Message)); }

        }


        [HttpGet]
        [Route(GromaxReports.GetStateListByLogin)]
        public async Task<IActionResult> GetStateListByLogin()
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _reportMaster.GetStateList(username, LoginPosition);
                return Ok(ApiResponse<IEnumerable<string>>.Success(result));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }


        }

        [HttpPost]
        [Route(GromaxReports.GetDealerByState)]
        public async Task<IActionResult> GetDealerByState([FromBody] StateModel model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _reportMaster.GetDealerByState(username, LoginPosition, model);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }


        }


        [HttpPost]
        [Route(GromaxReports.GetDealerAccByState)]
        public async Task<IActionResult> GetDealerAccByState([FromBody] StateModel model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _reportMaster.GetDealerAccByState(username, LoginPosition, model);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }


        }

        [HttpPost]
        [Route(GromaxReports.GetModelListByState)]
        public async Task<IActionResult> GetModelListByState([FromBody] StateModel model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _reportMaster.GetModelListByState(username, LoginPosition, model);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }


        }

        [HttpPost]
        [Route(GromaxReports.getBrand_HpForIndustry)]
        public async Task<IActionResult> getBrand_HpForIndustry([FromBody] StateModel model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _reportMaster.getBrand_HpForIndustry(username, LoginPosition, model);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }


        }

        [HttpPost]
        [Route(GromaxReports.GetDistrictAndTalukaList)]
        public async Task<IActionResult> GetDistrictAndTalukaList([FromBody] StateModel model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _reportMaster.GetDistrictAndTalukaList(username, LoginPosition, model);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }


        }


        [HttpGet]
        [Route(GromaxReports.GetActivityNameList)]
        public async Task<IActionResult> GetActivityNameList()
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _reportMaster.GetActivityNameListdb(username, LoginPosition);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }


        }


        [HttpPost]
        [Route(GromaxReports.GetGroupColumnJson)]
        public async Task<IActionResult> GetGroupColumnJson([FromBody] ReportParameterV5 model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                if (model.Name == "Modelwise DEL" || model.Name == "Stock Report(Modelwise)" || model.Name == "Modelwise BILL SH")
                {
                    var table = await _reportMaster.GetDynamicGroupColumnJsondb(username, LoginPosition, model);

                    GroupColumnJson md = new GroupColumnJson
                    {
                        ReportName = model.Name,
                        columnGroups = new List<ColumnGroup>()
                    };

                    List<string> groupNames = table
                        .Where(x => x.GroupName != null)
                        .Select(x => x.GroupName.ToString())
                        .Distinct()
                        .ToList();

                    foreach (string groupName in groupNames)
                    {
                        ColumnGroup clList = new ColumnGroup
                        {
                            GroupName = groupName,
                            Columns = new List<ReportColumn>()
                        };

                        var cols = table
                            .Where(x => x.GroupName != null && x.GroupName.ToString() == groupName)
                            .Select(x => new
                            {
                                ActualName = x.ActualName?.ToString(),
                                DisplayName = x.DisplayName?.ToString()
                            });

                        foreach (var col in cols)
                        {
                            clList.Columns.Add(new ReportColumn
                            {
                                ActualName = col.ActualName,
                                DisplayName = col.DisplayName
                            });
                        }

                        md.columnGroups.Add(clList);
                    }

                    return Ok(ApiResponse<dynamic>.Success(md));

                }
                var result = await _reportMaster.GetGroupColumnJsondb(username, LoginPosition, model);
                var jsondes = JsonConvert.DeserializeObject<GroupColumnJson>(result);
                return Ok(ApiResponse<dynamic>.Success(jsondes));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }


        }

        [HttpPost]
        [Route(GromaxReports.GetReportTracking)]
        public async Task<IActionResult> GetReportTracking([FromBody] ReportTrackingFilter model)
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _reportMaster.ReportTrackingdb(model, username, LoginPosition);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }
        }

        [HttpPost]
        [Route(GromaxReports.BDRCReportApproval)]
        public async Task<IActionResult> BDRCReportApproval([FromBody] BDRCModel model)
        {
            try
            {
                var Resultt = Utilities.UtilityFunctions.BDRCModelToConvertToDataTable(model.Bdrcclist);
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string LoginName = User.FindFirst("LoginName")?.Value;

                var result = await _reportMaster.updtBDRCModel(Resultt, username, LoginPosition, model.Status, model.Remark, LoginName);
                if (result > 1)
                    return Ok(ApiResponse<string>.Created());
                else
                    return Ok(ApiResponse<string>.Fail("Not Change Any row"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }
        }

        [HttpPost]
        [Route(GromaxReports.ForecastReportApproval)]
        public async Task<IActionResult> ForecastReportApproval([FromBody] ForecastModel model)
        {
            try
            {
                var Resultt = Utilities.UtilityFunctions.ForecastModelToConvertToDataTable(model.forecastlist);
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                string LoginName = User.FindFirst("LoginName")?.Value;
                var result = await _reportMaster.updtForecasttbl(Resultt, username, LoginPosition, model.Status, model.Remark, LoginName);
                if (result > 1)
                    return Ok(ApiResponse<string>.Created());
                else
                    return Ok(ApiResponse<string>.Fail("Not Change Any row"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }
        }

        [HttpPost]
        [Route(GromaxReports.insertPricePosition)]
        public async Task<IActionResult> insertPricePosition([FromForm] PricePositionModel model)
        {
            try
            {
                string mopProofString = null;
                string rcCopyString = null;
                if (model.mopProof == null)
                    return Ok(ApiResponse<string>.BadRequest("MOP Proof file is required."));
                else
                    mopProofString = await _getFilename._getFileName(model.mopProof);
                if (model.rcCopy != null)
                    rcCopyString = await _getFilename._getFileName(model.rcCopy);
                var result = await _reportMaster.insertPricePositiondb(model, mopProofString, rcCopyString);

                if (result > 0)
                    return Ok(ApiResponse<string>.Created());
                else
                    return Ok(ApiResponse<string>.Fail("Not Change Any row"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }
        }


        //[HttpPost]
        //[Route(GromaxReports.PricePosiReportApproval)]
        //public async Task<IActionResult> PricePosiReportApproval([FromBody] List<PricePositionApproval> model)
        //{
        //    try
        //    {
        //        //var Resultt = Utilities.UtilityFunctions.ForecastModelToConvertToDataTable(model.forecastlist);
        //        //string LoginPosition = User.FindFirst("LoginPosition")?.Value;
        //        //string username = User.FindFirst(ClaimTypes.Name)?.Value;
        //        //string LoginName = User.FindFirst("LoginName")?.Value;
        //        //var result = await _reportMaster.updtForecasttbl(Resultt, username, LoginPosition, model.Status, model.Remark, LoginName);
        //        var result = 1;
        //        if (result > 1)
        //            return Ok(ApiResponse<string>.Created());
        //        else
        //            return Ok(ApiResponse<string>.Fail("Not Change Any row"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ApiResponse<string>.Fail("Error"));
        //    }
        //}

        [HttpPost]
        [Route(GromaxReports.UpdatePricePosiReport)]
        public async Task<IActionResult> UpdatePricePosiReport([FromBody] List<UpdatePricePosition> model)
        {
            try
            {
                var Resultt = Utilities.UtilityFunctions.PricePositionModelToConvertToDataTable(model);
                var result = await _reportMaster.UpdatePricePosiReportdb(Resultt);

                if (result > 0)
                    return Ok(ApiResponse<string>.Created());
                else
                    return Ok(ApiResponse<string>.Fail("Not Change Any row"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }
        }



        [HttpPost]
        [Route(GromaxReports.UpdatePricePosiReportv1)]
        public async Task<IActionResult> UpdatePricePosiReportv1([FromBody] List<UpdatePricePositionv1> model)
        {
            try
            {
                var Resultt = Utilities.UtilityFunctions.PricePositionModelToConvertToDataTablev1(model);
                var result = await _reportMaster.UpdatePricePosiReportdbv1(Resultt);

                if (result > 0)
                    return Ok(ApiResponse<string>.Created());
                else
                    return Ok(ApiResponse<string>.Fail("Not Change Any row"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }
        }

        [HttpPost]
        [Route(GromaxReports.PricePosiReportApproval)]
        public async Task<IActionResult> ApprovalPricePosiReport([FromBody] PricePositionApproval model)
        {
            try
            {
                var guidList = model.IDs
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => Guid.TryParse(x, out var g) ? g : Guid.Empty)
                .Where(x => x != Guid.Empty)
                .ToList();

                // ✅ JSON banana (sirf Ids ke liye)
                var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(
                    guidList.Select(x => new { Id = x })
                );
                var result = await _reportMaster.ApprovalPricePosiReportdb(jsonData, model.Remark, model.isCommited);

                if (result > 0)
                    return Ok(ApiResponse<string>.Created());
                else
                    return Ok(ApiResponse<string>.Fail("Not Change Any row"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }
        }



        [HttpGet]
        [Route(GromaxReports.GetPricePositionReport)]
        public async Task<IActionResult> GetPricePositionReport()
        {
            try
            {
                string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _reportMaster.GetPricePositionReportdb(username, LoginPosition);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }


        }


        [HttpPost]
        [Route(GromaxReports.UpdatePricePosiReportv2)]
        public async Task<IActionResult> UpdatePricePosiReportv2([FromBody] List<UpdatePricePositionv1> model)
        {
            try
            {
                var Resultt = Utilities.UtilityFunctions.PricePositionModelToConvertToDataTablev2(model);
                var result = await _reportMaster.UpdatePricePosiReportdbv2(Resultt);

                if (result > 0)
                    return Ok(ApiResponse<string>.Created());
                else
                    return Ok(ApiResponse<string>.Fail("Not Change Any row"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }
        }


        [HttpGet]
        [Route(GromaxReports.GetStateListByLoginNew)]
        public async Task<IActionResult> GetStateListByLoginNew()
        {
            try
            {
                //string LoginPosition = User.FindFirst("LoginPosition")?.Value;
                //string username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _reportMaster.GetStateListNew();
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail("Error"));
            }


        }

        [HttpGet]
        [Route(GromaxReports.getOutlookFormatData)]
        public async Task<IActionResult> getOutlookFormatData([FromQuery] int month, int year)
        {
            try
            {
                var result = await _reportMaster.getOutlookFormatDatadb(month, year);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpGet]
        [Route(GromaxReports.getRevisedForExcel)]
        public async Task<IActionResult> getRevisedForExcel([FromQuery] RevisedBDRCRequest m)
        {
            try
            {
                var result = await _reportMaster.getRevisedForExceldb(m);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(GromaxReports.send90DaysPendingMessage)]
        public async Task<IActionResult> send90DaysPendingMessage()
        {
            try
            {
                List<AdvanceMoreThan90Whatsapp> result = await _reportMaster.get90DaysAdvancedb();
                List<string> uniqStateHead = result.Select(x => x.stateHeadMobile.ToString()).Distinct().ToList();
                foreach (string mobile in uniqStateHead)
                {
                    if (string.IsNullOrEmpty(mobile) || mobile.Length < 10)
                    {
                        continue;
                    }
                    List<AdvanceMoreThan90Whatsapp> seperateRecords = result.Where(x => x.stateHeadMobile == mobile).ToList();
                    foreach (var row in seperateRecords)
                    {

                        string res = await GromaxMobileApis.Utilities.WhatsappMessageSend.SendAdvances90DaysMoreAsync(row);
                        string messageString = await GromaxMobileApis.Utilities.WhatsappMessageSend.Get90DaysAgeingMessage(row);

                        if (!string.IsNullOrEmpty(res) && !res.Contains("error") && res.Contains("wamid."))
                        {
                            smartpingresponseRound response = Newtonsoft.Json.JsonConvert.DeserializeObject<smartpingresponseRound>(res);
                            await _db.updateNDAWhatsappCount(response.messages[0].id, messageString, row.stateHeadMobile, "SENT", "Gromax Advance");

                        }
                        else
                        {
                            await _db.updateNDAWhatsappCount(null, messageString, row.stateHeadMobile, "FAILED", "Gromax Advance");

                        }
                    }
                }

                return Ok(ApiResponse<string>.Success("All Messages Sent Successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpGet]
        [Route(GromaxReports.getTMByDistCode)]
        public async Task<IActionResult> getTMByDistCode([FromQuery] int distCode)
        {
            try
            {
                var result = await _reportMaster.getTMByDistCodedb(distCode);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpGet]
        [Route(GromaxReports.getPdd)]
        public async Task<IActionResult> getPdd([FromQuery] int month, int year)
        {
            try
            {
                var result = await _reportMaster.getPdddb(month, year);
                return Ok(ApiResponse<IEnumerable<dynamic>>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message));
            }
        }

    }
}
