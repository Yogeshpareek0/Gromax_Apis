using Dapper;
using Google.Api.Gax;
using GromaxMobileApis.Interfaces;
using GromaxMobileApis.Models;
using GromaxMobileApis.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static GromaxMobileApis.Models.ReportMasterModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GromaxMobileApis.Services
{
    public class ReportMaster : IReportMaster
    {
        private IDbConnection _db;
        private readonly string _conn;
        private readonly DapperParameterHelper _parameterHelper;
        private readonly IUser _User;
        public ReportMaster(IDbConnection db, IConfiguration con, DapperParameterHelper parameterHelper, IUser user)
        {
            _db = db;
            _conn = con.GetConnectionString("DefaultConnection");
            _parameterHelper = parameterHelper;
            _User = user;
        }
        public async Task<UploadMaster> GetUploadMasterById(int uploadID)
        {
            try
            {
                var parameter = new
                {
                    uploadID = uploadID
                };
                var result = await _db.QueryFirstOrDefaultAsync<UploadMaster>("usp_GetUploadMaster", param: parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<List<UploadColumnMapping>> GetMappings(int uploadID)
        {
            try
            {
                var parameter = new
                {
                    uploadID = uploadID
                };
                var result = (await _db.QueryAsync<UploadColumnMapping>("usp_GetColumnMapping", param: parameter, commandType: CommandType.StoredProcedure)).ToList();
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> MoveDatadb(int Month, int Year, string StagingTable, int UploadId, string createuser)
        {
            try
            {

                if (UploadId == 1)
                {
                    var parameter = new
                    {
                        Month = Month,
                        Year = Year,
                        SourceTable = StagingTable,
                        created_user = createuser
                    };
                    var result = (await _db.QueryFirstOrDefaultAsync<int>("usp_AddUpdateIndustryOnTalukaPerformancetbl", param: parameter, commandType: CommandType.StoredProcedure));
                    return result;

                }
                else if (UploadId == 2)
                {
                    var parameter = new
                    {
                        //Month = Month,
                        //Year = Year,
                        SourceTable = StagingTable,
                        created_user = createuser
                    };
                    var result = (await _db.QueryFirstOrDefaultAsync<int>("usp_AddUpdatePricePosition", param: parameter, commandType: CommandType.StoredProcedure));
                    return result;

                }
                else if (UploadId == 3)
                {
                    var parameter = new
                    {
                        Month = Month,
                        Year = Year,
                        SourceTable = StagingTable,
                        created_user = createuser
                    };
                    var result = (await _db.QueryFirstOrDefaultAsync<int>("usp_AddUpdateBDRCtbl", param: parameter, commandType: CommandType.StoredProcedure));
                    return result;

                }
                else if (UploadId == 4)
                {
                    var parameter = new
                    {
                        Month = Month,
                        Year = Year,
                        SourceTable = StagingTable,
                        created_user = createuser
                    };
                    var result = (await _db.QueryFirstOrDefaultAsync<int>("usp_AddUpdateForecastplan", param: parameter, commandType: CommandType.StoredProcedure));
                    return result;

                }
                else if (UploadId == 5)
                {
                    try
                    {
                        var parameter = new
                        {
                            Month = Month,
                            Year = Year,
                            SourceTable = StagingTable,
                            created_user = createuser
                        };
                        var result = (await _db.QueryFirstOrDefaultAsync<int>("usp_AddUpdateIndustrytbl", param: parameter, commandType: CommandType.StoredProcedure));
                        return result;
                    }
                    catch (Exception ex) { throw new Exception(ex.Message); }
                }
                else if (UploadId == 6)
                {
                    var parameter = new
                    {
                        Month = Month,
                        Year = Year,
                        SourceTable = StagingTable,
                        created_user = createuser
                    };
                    var result = (await _db.QueryFirstOrDefaultAsync<int>("usp_AddUpdateSalesPlanningActivity", param: parameter, commandType: CommandType.StoredProcedure));
                    return result;

                }
                else if (UploadId == 7)
                {
                    var parameter = new
                    {
                        Month = Month,
                        Year = Year,
                        SourceTable = StagingTable,
                        created_user = createuser
                    };
                    var result = (await _db.QueryFirstOrDefaultAsync<int>("usp_AddUpdateBusinessAccounttbl", param: parameter, commandType: CommandType.StoredProcedure));
                    return result;

                }
                int rows = 0;
                return rows;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> MoveDatadbv1(int Month, int Year, string StagingTable, int UploadId, string createuser, string loginposition, string loginName)
        {
            try
            {
                IEnumerable<dynamic> result = new List<dynamic>();
                if (UploadId == 1)
                {
                    var parameter = new
                    {
                        Month = Month,
                        Year = Year,
                        SourceTable = StagingTable,
                        created_user = createuser,
                        loginas = loginposition,
                        Name_user = loginName
                    };
                    result = await _db.QueryAsync("usp_AddUpdateIndustryOnTalukaPerformancetblv1", param: parameter, commandType: CommandType.StoredProcedure);

                }
                else if (UploadId == 3)
                {
                    var parameter = new
                    {
                        Month = Month,
                        Year = Year,
                        SourceTable = StagingTable,
                        created_user = createuser,
                        loginas = loginposition,
                        Name_user = loginName
                    };
                    result = await _db.QueryAsync("usp_AddUpdateBDRCtblv1", param: parameter, commandType: CommandType.StoredProcedure);

                }
                else if (UploadId == 4)
                {
                    var parameter = new
                    {
                        Month = Month,
                        Year = Year,
                        SourceTable = StagingTable,
                        username = createuser,
                        loginas = loginposition,
                        Name_user = loginName,
                        ReportId = UploadId
                    };
                    result = await _db.QueryAsync("usp_AddUpdateForecastplanv1", param: parameter, commandType: CommandType.StoredProcedure);

                }
                else if (UploadId == 7)
                {
                    var parameter = new
                    {
                        Month = Month,
                        Year = Year,
                        SourceTable = StagingTable,
                        created_user = createuser,
                        loginas = loginposition,
                        Name_user = loginName
                    };
                    result = await _db.QueryAsync("usp_AddUpdateBusinessAccounttblv1", param: parameter, commandType: CommandType.StoredProcedure);
                    return result;

                }
                else if (UploadId == 5)
                {
                    try
                    {
                        var parameter = new
                        {
                            Month = Month,
                            Year = Year,
                            SourceTable = StagingTable,
                            created_user = createuser,
                            loginas = loginposition,
                            Name_user = loginName
                        };
                        result = await _db.QueryAsync("usp_AddUpdateIndustrytblupd", param: parameter, commandType: CommandType.StoredProcedure);
                        return result;
                    }
                    catch (Exception ex) { throw new Exception(ex.Message); }
                }
                else if (UploadId == 10)
                {
                    var parameter = new
                    {
                        Month = Month,
                        Year = Year,
                        SourceTable = StagingTable,
                        created_user = createuser
                        //loginas = loginposition,
                        //Name_user = loginName
                    };
                    result = await _db.QueryAsync("usp_AddUpdateOutlookReport", param: parameter, commandType: CommandType.StoredProcedure);

                }
                else if (UploadId == 11)
                {
                    var parameter = new
                    {
                        Month = Month,
                        Year = Year,
                        StagingTableName = StagingTable,
                        CreatedUser = createuser
                        //loginas = loginposition,
                        //Name_user = loginName
                    };
                    result = await _db.QueryAsync("usp_UpdateBdrcRevised", param: parameter, commandType: CommandType.StoredProcedure);

                }

                else if (UploadId == 12)
                {
                    var parameter = new
                    {
                        StagingTableName = StagingTable,
                        CreatedUser = createuser
                        //loginas = loginposition,
                        //Name_user = loginName
                    };
                    result = await _db.QueryAsync("USP_InsertUpdatePDD", param: parameter, commandType: CommandType.StoredProcedure);

                }
                return result;


            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetDashboardData(string LoginPosition, string username)
        {
            try
            {
                var param = new { Loginas = LoginPosition, username = username };
                var result = await _db.QueryAsync("usp_GetDashboardData", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<dynamic> GetDashboardConfigJson(int Id)
        {
            try
            {
                var parameter = new
                {
                    Id = Id
                };
                var result = await _db.QueryFirstOrDefaultAsync<dynamic>("usp_GetDashboardConfig", param: parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<dynamic> GetSavedReports(int Id)
        {
            try
            {
                var parameter = new
                {
                    Id = Id
                };
                var result = await _db.QueryFirstOrDefaultAsync<dynamic>("usp_GetSavedReports", param: parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetUserDefineTablesdb()
        {
            try
            {

                var result = await _db.QueryAsync("usp_GetUserDefineTablesName", commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetUserDefineColumnsdb(int Id)
        {
            try
            {
                var parameter = new
                {
                    TableId = Id
                };
                var result = await _db.QueryAsync("usp_GetColumnsName", param: parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetUserDefineProceduresdb()
        {
            try
            {

                var result = await _db.QueryAsync("usp_GetStoreProceduresName", commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetUserDefineParameterdb(int Id)
        {
            try
            {
                var parameter = new
                {
                    StoreProcedureId = Id
                };
                var result = await _db.QueryAsync("usp_GetParameterName", param: parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> InsertReportConfigdb(ReportsConfigModel model)
        {
            try
            {
                var parameter = new
                {
                    Name = model.reportName,
                    reportConfigJson = model.reportConfigJson
                };
                var result = await _db.ExecuteAsync("usp_InsertSavedReports", param: parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> SaveDashboardConfigdb(DashboardViewModel model)
        {
            try
            {
                string ConfigJson = JsonConvert.SerializeObject(model);
                var parameter = new
                {
                    Name = model.Name,
                    Theme = model.Theme,
                    ConfigJson = model.Sections
                };
                var result = await _db.ExecuteAsync("usp_SaveDashboardConfig", param: parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<DashboardViewModel> GetDashboardConfigJsonv1(int Id)
        {
            try
            {
                var parameter = new
                {
                    Id = Id
                };
                var result = await _db.QueryFirstOrDefaultAsync<DashboardViewModel>("usp_GetDashboardConfig", param: parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> ExecuteStoredProcedure(string procedureName, List<ReportParameterV5> parameter)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
                throw new ArgumentException("Stored procedure name is required", nameof(procedureName));

            try
            {
                using (var con = new SqlConnection(_conn))
                {
                    var parameters = _parameterHelper.BuildParameters(parameter);
                    System.Diagnostics.Debug.WriteLine(
                        $"Executing SP: {procedureName} with {parameters.ParameterNames.Count()} params");

                    var result = await con.QueryAsync(
                        procedureName,
                        param: parameters,
                        commandType: CommandType.StoredProcedure
                    //commandTimeout: 120
                    );

                    return result;
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {sqlEx.Message}");
                throw new Exception(
                    $"Error executing stored procedure '{procedureName}': {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                throw new Exception(
                    $"Error executing stored procedure '{procedureName}': {ex.Message}", ex);
            }
        }


        //private object ConvertToDbValue(object raw, string dataType, out SqlDbType dbType)
        //{
        //    dbType = SqlDbType.NVarChar;
        //    if (raw == null) return null;
        //    var s = raw.ToString();
        //    if (string.IsNullOrWhiteSpace(s)) return null;

        //    switch ((dataType ?? "text").ToLowerInvariant())
        //    {
        //        case "text":
        //            dbType = SqlDbType.NVarChar; return s;
        //        case "number":
        //            dbType = SqlDbType.Decimal;
        //            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var dec)) return dec;
        //            return DBNull.Value;
        //        case "date":
        //        case "datetime":
        //            dbType = SqlDbType.DateTime;
        //            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt)) return dt;
        //            if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) return dt;
        //            return DBNull.Value;
        //        case "dropdown":
        //            dbType = SqlDbType.NVarChar; return s;
        //        default:
        //            dbType = SqlDbType.NVarChar; return s;
        //    }
        //}

        public async Task<IEnumerable<dynamic>> ExecuteSqlQuery(string Sql, List<ReportParameterV5> Param)
        {
            var dapperParams = _parameterHelper.BuildParameters(Param);

            using (var con = new SqlConnection(_conn))
            {
                var result = await con.QueryAsync(Sql, dapperParams);
                return result;
            }

        }

        public async Task<IEnumerable<dynamic>> GetUploadMasterListdb()
        {
            try
            {

                var result = await _db.QueryAsync("usp_GetUploadMasterList", commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<List<string>> Dropdownlistdb(string q)
        {
            try
            {
                if (!q.Trim().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Only SELECT queries are allowed.");
                //var parameter = new { query = q };
                var result = (await _db.QueryAsync<string>(q, commandType: CommandType.Text)).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<string>> GetStateList(string username, string loginas)
        {
            try
            {
                var parameter = new
                {
                    Username = username,
                    loginas = loginas
                };
                var values = await _db.QueryAsync<string>("usp_GetStateListByLogin",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetDealerByState(string username, string loginas, StateModel model)
        {
            try
            {
                var parameter = new
                {
                    Username = username,
                    loginas = loginas,
                    StateName = model.StateName
                };
                var values = await _db.QueryAsync<dynamic>("usp_GetDealerByState",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }


        public async Task<IEnumerable<dynamic>> GetDealerAccByState(string username, string loginas, StateModel model)
        {
            try
            {
                var parameter = new
                {
                    Username = username,
                    loginas = loginas,
                    StateName = model.StateName
                };
                var values = await _db.QueryAsync<dynamic>("usp_GetDealerByStateForAccounttbl",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetModelListByState(string username, string loginas, StateModel model)
        {
            try
            {
                var parameter = new
                {
                    Username = username,
                    loginas = loginas,
                    StateName = model.StateName
                };
                var values = await _db.QueryAsync<dynamic>("usp_GetModelListByState",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> getBrand_HpForIndustry(string username, string loginas, StateModel model)
        {
            try
            {
                var parameter = new
                {
                    //Username = username,
                    //loginas = loginas,
                    StateName = model.StateName
                };
                var values = await _db.QueryAsync<dynamic>("usp_getBrand_HpForIndustry",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetDistrictAndTalukaList(string username, string loginas, StateModel model)
        {
            try
            {
                var parameter = new
                {
                    Username = username,
                    loginas = loginas,
                    StateName = model.StateName
                };
                var values = await _db.QueryAsync<dynamic>("usp_getDistrictAndTalukaList",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetActivityNameListdb(string username, string loginas)
        {
            try
            {
                var parameter = new
                {
                    username = username,
                    loginas = loginas,

                };
                var values = await _db.QueryAsync<dynamic>("usp_GetActivityNameList",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<string> GetGroupColumnJsondb(string username, string loginas, ReportParameterV5 model)
        {
            try
            {
                var parameter = new
                {

                    reportName = model.Name,

                };
                var values = await _db.QueryAsync<string>("usp_getgroupcolumnjson",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values.FirstOrDefault();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<DynamicGrouping>> GetDynamicGroupColumnJsondb(string username, string loginas, ReportParameterV5 model)
        {
            try
            {
                var parameter = new
                {

                    reportName = model.Name,

                };
                var values = await _db.QueryAsync<DynamicGrouping>("usp_GetDynamicGroupColumns",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> ReportTrackingdb(ReportTrackingFilter model, string username, string loginas)
        {
            try
            {
                var parameter = new
                {

                    loginas = loginas,
                    username = username,
                    month = model.Month,
                    year = model.Year,
                    stateName = model.StateName,
                    ReportId = model.UploadId,
                    model.SHMail,
                    model.AmMail,
                    model.TmMail,
                    model.billingTtl

                };
                var values = await _db.QueryAsync("usp_getReportReviewTrackingTblforAppro",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> updtBDRCModel(DataTable dt, string username, string loginas, string s, string rr, string LoginName)
        {
            try
            {
                var parameter = new
                {

                    loginas = loginas,
                    username = username,
                    typetable = dt,
                    Status = s,
                    Remark = rr,
                    Name_user = LoginName


                };
                var values = await _db.ExecuteAsync("usp_ApproveBDRCReport",
                    param: parameter, commandType: CommandType.StoredProcedure, commandTimeout: 300);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> updtForecasttbl(DataTable dt, string username, string loginas, string s, string rr, string LoginName)
        {
            try
            {
                var parameter = new
                {

                    loginas = loginas,
                    username = username,
                    typetable = dt,
                    Status = s,
                    Remark = rr,
                    Name_user = LoginName


                };
                var values = await _db.ExecuteAsync("usp_ApproveForecastReport",
                    param: parameter, commandType: CommandType.StoredProcedure, commandTimeout: 300);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }


        public async Task<int> insertPricePositiondb(PricePositionModel model, string mopProofString, string rcCopyString)
        {
            try
            {
                var parameter = new
                {

                    loginas = _User.GetPositionName(),
                    username = _User.GetUserName(),
                    model.stateName,
                    model.hpRange,
                    model.hp,
                    model.make,
                    model = model.bom,
                    avgVolPerMonth = Convert.ToDouble(model.avgVolPerMonth),
                    model.variantCode,
                    ndp = Convert.ToDouble(model.ndp),
                    freight = Convert.ToDouble(model.freight),
                    accessories = Convert.ToDouble(model.accessories),
                    dlrMargin = Convert.ToDouble(model.dlrMargin),
                    mop = Convert.ToDouble(model.mop),
                    AccessoryName = model.AccessoryName,
                    mopDate = Convert.ToDateTime(model.mopDate),
                    mopProofString,
                    rcCopyString,
                    implementPrice = Convert.ToDouble(model.implementPrice),
                    model.driveType,
                    rtoInsurance = Convert.ToDouble(model.rtoInsurance),
                    offerPrice = Convert.ToDouble(model.offerPrice)



                };
                var values = await _db.ExecuteAsync("usp_AddUpdatePricePosition",
                    param: parameter, commandType: CommandType.StoredProcedure, commandTimeout: 300);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }


        public async Task<int> UpdatePricePosiReportdb(DataTable tb)
        {
            try
            {
                var parameter = new
                {
                    loginas = _User.GetPositionName(),
                    username = _User.GetUserName(),
                    pricePositiontype = tb,
                    Name_user = _User.GetName()
                };
                var values = await _db.ExecuteAsync("usp_updatePricePosition",
                    param: parameter, commandType: CommandType.StoredProcedure, commandTimeout: 300);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> UpdatePricePosiReportdbv1(DataTable tb)
        {
            try
            {
                var parameter = new
                {
                    loginas = _User.GetPositionName(),
                    username = _User.GetUserName(),
                    pricePositiontype = tb,
                    Name_user = _User.GetName()
                };
                var values = await _db.ExecuteAsync("usp_updatePricePositionv1",
                    param: parameter, commandType: CommandType.StoredProcedure, commandTimeout: 300);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> ApprovalPricePosiReportdb(string jsonIds, string Remarks, int Iscommitted)
        {
            try
            {
                var parameter = new
                {
                    loginas = _User.GetPositionName(),
                    username = _User.GetUserName(),
                    Name_user = _User.GetName(),
                    jsonIds,
                    Remarks,
                    Iscommitted

                };
                var values = await _db.ExecuteAsync("usp_ApprovalPricePosition",
                    param: parameter, commandType: CommandType.StoredProcedure, commandTimeout: 300);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetPricePositionReportdb(string username, string loginas)
        {
            try
            {
                var parameter = new
                {
                    username = username,
                    loginas = loginas,

                };
                var values = await _db.QueryAsync<dynamic>("usp_getPricePositionReport",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> UpdatePricePosiReportdbv2(DataTable tb)
        {
            try
            {
                var parameter = new
                {
                    loginas = _User.GetPositionName(),
                    username = _User.GetUserName(),
                    pricePositiontype = tb,
                    Name_user = _User.GetName()
                };
                var values = await _db.ExecuteAsync("usp_updatePricePositionv2",
                    param: parameter, commandType: CommandType.StoredProcedure, commandTimeout: 300);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetStateListNew()
        {
            try
            {
                var parameter = new
                {
                    Username = _User.GetUserName(),
                    loginas = _User.GetPositionName()
                };
                var values = await _db.QueryAsync<dynamic>("usp_GetStateListByLoginNew",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> getOutlookFormatDatadb(int month, int year)
        {
            try
            {
                var parameter = new
                {
                    userName = _User.GetUserName(),
                    loginAs = _User.GetPositionName(),
                    month,
                    year
                };
                var values = await _db.QueryAsync<dynamic>("usp_getOutlookFormatData",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> getRevisedForExceldb(RevisedBDRCRequest m)
        {
            try
            {
                var parameter = new
                {
                    userName = _User.GetUserName(),
                    loginAs = _User.GetPositionName(),
                    m.month,
                    m.year,
                    m.week,
                    m.stateName
                };
                var values = await _db.QueryAsync<dynamic>("usp_fetchRevisedForExcel",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<List<AdvanceMoreThan90Whatsapp>> get90DaysAdvancedb()
        {
            try
            {
                var values = await _db.QueryAsync<AdvanceMoreThan90Whatsapp>("usp_advMoreThan90ForWhatsapp",
                     commandType: CommandType.StoredProcedure);
                return values.ToList();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> getTMByDistCodedb(int distcode)
        {
            try
            {
                var parameter = new
                {
                    distCode = distcode,
                };
                var values = await _db.QueryAsync<dynamic>("usp_getTmByDistrictCode",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }


        public async Task<IEnumerable<dynamic>> getPdddb(int month, int year)
        {
            try
            {
                var parameter = new
                {
                    userName = _User.GetUserName(),
                    loginAs = _User.GetPositionName(),
                    month,
                    year
                };
                var values = await _db.QueryAsync<dynamic>("usp_getPddData",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
