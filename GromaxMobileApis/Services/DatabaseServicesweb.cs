using Dapper;
using GromaxMobileApis.Interfaces;
using GromaxMobileApis.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GromaxMobileApis.Services
{
    public class DatabaseServicesweb : IDatabaseServicesweb
    {
        private IDbConnection _db;
        private string _conString;
        private IUser _user;
        public DatabaseServicesweb(IDbConnection db, IConfiguration configuration, IUser user)
        {
            _db = db;
            _conString = configuration.GetConnectionString("DefaultConnection");
            _user = user;
        }

        public async Task<IEnumerable<dynamic>> DownloadSalesEnquiryMasterdb(SalesEnquiryMasterFiltered model, string LoginPosition, string Username)
        {
            try
            {
                //var parameter = new { Dealercode = DealerCode };
                var parameter = new
                {
                    //PageSize = string.IsNullOrWhiteSpace(model.PageSize) ? 0 : int.Parse(model.PageSize),
                    //RowStart = string.IsNullOrWhiteSpace(model.RowStart) ? 0 : int.Parse(model.RowStart),
                    StateHeadMail = model.ShMail,
                    AmMail = model.AmMail,
                    TmMail = model.TmMail,
                    DealerMail = model.DealerMail,
                    FollowenquiryStatus = model.FollowenquiryStatus,
                    Startdate = model.Startdate,
                    Enddate = model.Enddate,
                    LoginMail = Username,
                    LoginPosition = LoginPosition,
                    Leads = model.Leads,
                    Source = model.Source,
                    model.subSource,
                    model.Location,
                    model.StateCode
                };
                var vals = await _db.QueryAsync("usp_DownloadGetSalesEnquiryMasterSp", param: parameter, commandType: CommandType.StoredProcedure);
                //var vals = await _db.QueryAsync("usp_WebGetSalesEnquiryMasterSpv1", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<BussinessPerformanceResult> GetBussinessPerformance(BussinessPerformance model, string loginas, string username)
        {
            var parameter = new { BoxFilter = model.BoxFilter, Startdate = model.Startdate, Enddate = model.Enddate, AmMail = model.AmMail, ShMail = model.ShMail, TmMail = model.TmMail, DealerMail = model.DealerMail, LoginPosition = loginas, LoginMail = username };

            using var multi = await _db.QueryMultipleAsync(
                "usp_BussinessPerformance",
                param: parameter,
                commandType: CommandType.StoredProcedure
            );

            List<dynamic> CleanNullRows(IEnumerable<dynamic> data) =>
                (data ?? Enumerable.Empty<dynamic>())
                    .Where(row => row is IDictionary<string, object> dict && dict.Values.Any(v => v != null))
                    .ToList();

            return new BussinessPerformanceResult
            {
                GetBoxes = CleanNullRows(await multi.ReadAsync<dynamic>()),
                GetList = CleanNullRows(await multi.ReadAsync<dynamic>())
                //TerritoryManagers = CleanNullRows(await multi.ReadAsync<dynamic>()),
                //Dealers = CleanNullRows(await multi.ReadAsync<dynamic>())
            };
        }

        public async Task<BussinessPerformanceResult> GetBussinessPerformanceWeb(BussinessPerformance model, string loginas, string username)
        {
            var parameter = new { PageSize = model.PageSize, RowStart = model.RowStart, BoxFilter = model.BoxFilter, Startdate = model.Startdate, Enddate = model.Enddate, AmMail = model.AmMail, ShMail = model.ShMail, TmMail = model.TmMail, DealerMail = model.DealerMail, LoginPosition = loginas, LoginMail = username };

            using var multi = await _db.QueryMultipleAsync(
                "usp_BussinessPerformanceWeb",
                param: parameter,
                commandType: CommandType.StoredProcedure
            );

            List<dynamic> CleanNullRows(IEnumerable<dynamic> data) =>
                (data ?? Enumerable.Empty<dynamic>())
                    .Where(row => row is IDictionary<string, object> dict && dict.Values.Any(v => v != null))
                    .ToList();

            return new BussinessPerformanceResult
            {
                GetBoxes = CleanNullRows(await multi.ReadAsync<dynamic>()),
                GetList = CleanNullRows(await multi.ReadAsync<dynamic>())
                //TerritoryManagers = CleanNullRows(await multi.ReadAsync<dynamic>()),
                //Dealers = CleanNullRows(await multi.ReadAsync<dynamic>())
            };
        }

        public async Task<IEnumerable<dynamic>> GetInventoryData(StockFilterModel model, string loginas, string username)
        {
            try
            {
                //var parameter = new { Dealercode = DealerCode };
                var parameter = new { StateHeadMail = model.ShMail, startdate = model.StartDate, enddate = model.EndDate, loginas = loginas, LoginMail = username, AmMail = model.AmMail, TmMail = model.TmMail, DealerMail = model.DealerMail, AgingStartDay = model.AgingStartDay, AgingEndDay = model.AgingEndDay };
                var vals = await _db.QueryAsync("usp_GetStockMasterByFilterv1", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetInventoryDataWeb(StockFilterModel model, string loginas, string username)
        {
            try
            {
                //var parameter = new { Dealercode = DealerCode };
                var parameter = new
                {
                    RowStart = string.IsNullOrWhiteSpace(model.RowStart) ? 0 : int.Parse(model.RowStart),
                    PageSize = string.IsNullOrWhiteSpace(model.PageSize) ? 0 : int.Parse(model.PageSize),
                    StateHeadMail = model.ShMail,
                    startdate = model.StartDate,
                    enddate = model.EndDate,
                    loginas = loginas,
                    LoginMail = username,
                    AmMail = model.AmMail,
                    TmMail = model.TmMail,
                    DealerMail = model.DealerMail,
                    AgingStartDay = model.AgingStartDay,
                    AgingEndDay = model.AgingEndDay,
                    StockStatus = model.StockStatus,
                    model.location,
                    model.StateCode
                };
                var vals = await _db.QueryAsync("usp_GetStockMasterByFilterv1Web", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        //public async Task<IEnumerable<dynamic>> GetLeads(FilterModel model, string loginas, string username)
        //{

        //    try
        //    {
        //        //var parameter = new { Dealercode = DealerCode };
        //        var parameter = new { startdate = model.StartDate.Date, enddate = model.EndDate.Date, loginas = loginas, LoginMail = username, AmMail = model.AmMail, TmMail = model.TmMail, DealerMail = model.DealerMail };
        //        var vals = await _db.QueryAsync("usp_GetLeadsByFilter", param: parameter, commandType: CommandType.StoredProcedure);
        //        return vals;
        //    }
        //    catch (Exception ex) { throw new Exception(ex.Message); }
        //}
       
        public async Task<IEnumerable<dynamic>> GetLeads(SalesEnquiryMasterFiltered model, string loginas, string username)
        {

            try
            {
                //var parameter = new { Dealercode = DealerCode };
                var parameter = new { Leads = model.Leads, FollowenquiryStatus = model.FollowenquiryStatus, StateHeadMail = model.ShMail, startdate = model.Startdate, enddate = model.Enddate, loginas = loginas, LoginMail = username, AmMail = model.AmMail, TmMail = model.TmMail, DealerMail = model.DealerMail, source = model.Source, model.subSource, model.Location, model.StateCode };
                var vals = await _db.QueryAsync("usp_GetLeadsByFilterv1", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<dynamic> GetLeadsFollowUpFilterWeb(string loginas, string username)
        {
            try
            {
                //var parameter = new { Dealercode = DealerCode };
                var parameter = new { loginas = loginas, LoginMail = username };
                var vals = await _db.QueryFirstOrDefaultAsync("usp_GetLeadsFollowUpFilterWeb", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<PositionFilterResult> GetPossitionFilter(PositionFilter model, string loginas, string username)
        {
            try
            {
                var parameter = new
                {
                    StateHeadMail = model.ShMail,
                    AmMail = model.AmMail,
                    TmMail = model.TmMail,
                    DealerMail = model.DealerMail,
                    StateName = model.StateName,
                    Loginas = loginas,
                    username = username
                };

                using var multi = await _db.QueryMultipleAsync(
                    "usp_PossitionFilterv2",
                    param: parameter,
                    commandType: CommandType.StoredProcedure
                );

                List<dynamic> CleanNullRows(IEnumerable<dynamic> data) =>
                    (data ?? Enumerable.Empty<dynamic>())
                        .Where(row => row is IDictionary<string, object> dict && dict.Values.Any(v => v != null))
                        .ToList();

                return new PositionFilterResult
                {
                    StateHead = CleanNullRows(await multi.ReadAsync<dynamic>()),
                    AreaManagers = CleanNullRows(await multi.ReadAsync<dynamic>()),
                    TerritoryManagers = CleanNullRows(await multi.ReadAsync<dynamic>()),
                    Dealers = CleanNullRows(await multi.ReadAsync<dynamic>()),
                    States = CleanNullRows(await multi.ReadAsync<dynamic>()),
                    Location = CleanNullRows(await multi.ReadAsync<dynamic>())
                };
            }
            catch
            {
                throw new Exception("Database Error");
            }
        }

        public async Task<IEnumerable<dynamic>> GetSalesEnquiryMaster(SalesEnquiryMasterFiltered model, string LoginPosition, string Username)
        {
            try
            {
                //var parameter = new { Dealercode = DealerCode };
                var parameter = new
                {
                    PageSize = string.IsNullOrWhiteSpace(model.PageSize) ? 0 : int.Parse(model.PageSize),
                    RowStart = string.IsNullOrWhiteSpace(model.RowStart) ? 0 : int.Parse(model.RowStart),
                    StateHeadMail = model.ShMail,
                    AmMail = model.AmMail,
                    TmMail = model.TmMail,
                    DealerMail = model.DealerMail,
                    FollowenquiryStatus = model.FollowenquiryStatus,
                    Startdate = model.Startdate,
                    Enddate = model.Enddate,
                    LoginMail = Username,
                    LoginPosition = LoginPosition,
                    Leads = model.Leads
                };
                var vals = await _db.QueryAsync("usp_WebGetSalesEnquiryMasterSp", param: parameter, commandType: CommandType.StoredProcedure);
                //var vals = await _db.QueryAsync("usp_WebGetSalesEnquiryMasterSpv1", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<GetLoginmasterweb> GetUserData(Loginmasterdto model)
        {
            try
            {
                var parameters = new { MobileNumber = model.MobileNo, DeviceId = model.DeviceId, DeviceModel = model.ModelDevice, DeviceBrand = model.DeviceBrand, Platform = model.PlatformType, Action = 1 };
                var vals = await _db.QuerySingleOrDefaultAsync<GetLoginmasterweb>(
                "loginuser",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> InsertStockdb(DataTable file)
        {
            try
            {
                var parameters = new { StockTypeTable = file };
                var vals = await _db.ExecuteAsync(
                "usp_InsertStock",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> LoginUser(string MobileNo)
        {

            {
                try
                {
                    var parameters = new { MobileNumber = MobileNo, Action = 0 };
                    var vals = await _db.QueryFirstOrDefaultAsync<int>(
                    "loginuser",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
                    if (vals == 1)
                        return "Successfully";
                    else if (vals == 2)
                        return "User Already Login";
                    else
                        return "User Not Exists";


                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<int> UploadCampaignSalesEnquiry(DataTable dt)
        {
            try
            {
                var parameters = new { CteTable = dt };
                var vals = await _db.ExecuteAsync(
                "usp_UploadCampaignSalesEnquiry",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetPendingSalesFollowupList(SalesEnquiryMaster model, string Mobile, string Dealercode, string LoginPosition, string username)
        {
            try
            {
                var parameters = new
                {
                    PageSize = int.Parse(model.PageSize),
                    RowStart = int.Parse(model.RowStart),
                    mobile = Mobile,
                    dealercode = Dealercode,
                    source = model.EnquirySource,
                    Model = model.InterestedModel,
                    enquirytype = model.EnquiryStatus,
                    followuptype = model.EnquiryType,
                    loginas = LoginPosition,
                    username = username,

                };
                var vals = await _db.QueryAsync(
                "PendingSalesFollowupListWebsp",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task<IEnumerable<dynamic>> GetPendingSalesFollowupListv1(EnquiryFilterRequest model, string Mobile, string Dealercode, string LoginPosition, string username)
        {
            try
            {
                var parameters = new
                {
                    PageSize = int.Parse(model.PageSize),
                    RowStart = int.Parse(model.RowStart),
                    mobile = Mobile,
                    dealercode = model.DealerCode,
                    source = model.EnquirySource,
                    Model = model.InterestedModel,
                    enquirytype = model.EnquiryStatus,
                    followuptype = model.EnquiryType,
                    loginas = LoginPosition,
                    username = username,
                    model.AMName,
                    model.TMName,
                    model.SHName,
                    model.DealerCategory,
                    model.EnquirySubSource,
                    model.location,
                    model.StateCode

                };
                var vals = await _db.QueryAsync(
                "PendingSalesFollowupListWebsp",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> GetLeadFollowUp(string DealerCode, string loginas, string username)
        {
            try
            {
                var parameters = new { dealercode = DealerCode, loginas = loginas, username = username };
                var vals = await _db.QueryAsync(
                "GetLeadsFollowUpWebSp",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> GetInventoryDataWebv1(StockFilterModel model, string loginas, string username)
        {
            try
            {
                //var parameter = new { Dealercode = DealerCode };
                var parameter = new
                {
                    RowStart = string.IsNullOrWhiteSpace(model.RowStart) ? 0 : int.Parse(model.RowStart),
                    PageSize = string.IsNullOrWhiteSpace(model.PageSize) ? 0 : int.Parse(model.PageSize),
                    StateHeadMail = model.ShMail,
                    startdate = model.StartDate,
                    enddate = model.EndDate,
                    loginas = loginas,
                    LoginMail = username,
                    AmMail = model.AmMail,
                    TmMail = model.TmMail,
                    DealerMail = model.DealerMail,
                    AgingStartDay = model.AgingStartDay,
                    AgingEndDay = model.AgingEndDay,
                    StockStatus = model.StockStatus
                };
                var vals = await _db.QueryAsync("usp_GetStockMasterByFilterv2Web", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetInventoryDatav1(StockFilterModel model, string loginas, string username)
        {
            try
            {
                //var parameter = new { Dealercode = DealerCode };
                var parameter = new { StateHeadMail = model.ShMail, startdate = model.StartDate, enddate = model.EndDate, loginas = loginas, LoginMail = username, AmMail = model.AmMail, TmMail = model.TmMail, DealerMail = model.DealerMail, AgingStartDay = model.AgingStartDay, AgingEndDay = model.AgingEndDay, StockStatus = model.StockStatus };
                var vals = await _db.QueryAsync("usp_GetStockMasterByFilterv2", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<string> LoginUserv1(string MobileNo, string p)
        {

            {
                try
                {
                    var parameters = new { MobileNumber = MobileNo, Password = p, Action = 0 };
                    var vals = await _db.QueryFirstOrDefaultAsync<int>(
                    "loginuserv1",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
                    if (vals == 1)
                        return "Successfully";
                    else if (vals == 2)
                        return "User Already Login";
                    else
                        return "User Not Exists";


                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<IEnumerable<dynamic>> GetAvailableStockDatadb(StockFilterModel model, string loginas, string username)
        {
            try
            {
                //var parameter = new { Dealercode = DealerCode };
                var parameter = new
                {
                    RowStart = string.IsNullOrWhiteSpace(model.RowStart) ? 0 : int.Parse(model.RowStart),
                    PageSize = string.IsNullOrWhiteSpace(model.PageSize) ? 0 : int.Parse(model.PageSize),
                    StateHeadMail = model.ShMail,
                    startdate = model.StartDate,
                    enddate = model.EndDate,
                    loginas = loginas,
                    LoginMail = username,
                    AmMail = model.AmMail,
                    TmMail = model.TmMail,
                    DealerMail = model.DealerMail,
                    //AgingStartDay = model.AgingStartDay,
                    //AgingEndDay = model.AgingEndDay
                };
                var vals = await _db.QueryAsync("usp_GetStockMasterAvilableStock", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> ExistsUserdb(string MobileNo)
        {
            try
            {
                var Parameter = new { MobileNo = MobileNo };
                var vals = await _db.QuerySingleOrDefaultAsync<int>("usp_UserExists", param: Parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> UpdatePassworddb(string MobileNo, string P)
        {
            try
            {
                var Parameter = new { MobileNo = MobileNo, Password = P };
                var vals = await _db.QuerySingleOrDefaultAsync<int>("usp_UpdateUserPassword", param: Parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        
        public async Task<IEnumerable<dynamic>> GetSalesEnquiryMasterv1(SalesEnquiryMasterFiltered model, string LoginPosition, string Username)
        {
            try
            {
                //var parameter = new { Dealercode = DealerCode };
                var parameter = new
                {
                    PageSize = string.IsNullOrWhiteSpace(model.PageSize) ? 0 : int.Parse(model.PageSize),
                    RowStart = string.IsNullOrWhiteSpace(model.RowStart) ? 0 : int.Parse(model.RowStart),
                    StateHeadMail = model.ShMail,
                    AmMail = model.AmMail,
                    TmMail = model.TmMail,
                    DealerMail = model.DealerMail,
                    FollowenquiryStatus = model.FollowenquiryStatus,
                    Startdate = model.Startdate,
                    Enddate = model.Enddate,
                    LoginMail = Username,
                    LoginPosition = LoginPosition,
                    Leads = model.Leads,
                    Source = model.Source,
                    model.subSource,
                    model.Location,
                    model.StateCode
                };
                var vals = await _db.QueryAsync("usp_WebGetSalesEnquiryMasterSpPortal", param: parameter, commandType: CommandType.StoredProcedure);
                //var vals = await _db.QueryAsync("usp_WebGetSalesEnquiryMasterSpv1", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<BussinessPerformanceResult> GetBussinessPerformanceWebv1(BussinessPerformance model, string loginas, string username)
        {
            var parameter = new { PageSize = model.PageSize, RowStart = model.RowStart, BoxFilter = model.BoxFilter, Startdate = model.Startdate, Enddate = model.Enddate, AmMail = model.AmMail, ShMail = model.ShMail, TmMail = model.TmMail, DealerMail = model.DealerMail, LoginPosition = loginas, LoginMail = username, Source = model.Source, model.Location, model.subSource, model.StateCode };

            using var multi = await _db.QueryMultipleAsync(
                "usp_BussinessPerformanceWebv2",
                param: parameter,
                commandType: CommandType.StoredProcedure
            );

            List<dynamic> CleanNullRows(IEnumerable<dynamic> data) =>
                (data ?? Enumerable.Empty<dynamic>())
                    .Where(row => row is IDictionary<string, object> dict && dict.Values.Any(v => v != null))
                    .ToList();

            return new BussinessPerformanceResult
            {
                GetBoxes = CleanNullRows(await multi.ReadAsync<dynamic>()),
                GetList = CleanNullRows(await multi.ReadAsync<dynamic>())
                //TerritoryManagers = CleanNullRows(await multi.ReadAsync<dynamic>()),
                //Dealers = CleanNullRows(await multi.ReadAsync<dynamic>())
            };
        }

        public async Task<int> InsertBillingdb(DataTable file)
        {
            try
            {
                var parameters = new { StockTypeTable = file };
                var vals = await _db.ExecuteAsync(
                "usp_InsertBilling",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> getDealerByTehsildb(DealerListByLocationModel model)
        {
            try
            {
                var res = await _db.QueryAsync<dynamic>("usp_getDealerByTehsil", param: model, commandType: CommandType.StoredProcedure);
                return res;
            }
            catch
            {
                throw new Exception("Database Error");
            }
        }
        
        public async Task<IEnumerable<dynamic>> notAssignDealerList(NotAssign model, string username)
        {
            try
            {
                var parameter = new
                {
                    PageSize = model.PageSize,
                    RowStart = model.RowStart,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    username = username,
                    LoginPosition = _user.GetPositionName()
                };
                var res = await _db.QueryAsync<dynamic>("usp_notAssignDealerEnquiryList",
                    param: parameter,
                    commandType: CommandType.StoredProcedure);
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception("Database Error");
            }
        }

        public async Task<bool> assignDealerToEnquiry(AssignDealerModel model)
        {
            try
            {
                var parameter = new
                {
                    Ids = string.Join(",", model.Ids),
                    DealerCode = model.DealerCode
                };
                await _db.ExecuteAsync("usp_assignDealerToEnquiry",
                    param: parameter,
                    commandType: CommandType.StoredProcedure);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Database Error");
            }
        }
    }
}
