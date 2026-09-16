using Dapper;
using FirebaseAdmin.Messaging;
using Google.Api.Gax;
using GromaxMobileApis.Interfaces;
using GromaxMobileApis.Models;
using GromaxMobileApis.Models.DealerMaster;
using GromaxMobileApis.Models.EmployeeMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
//using static GromaxMobileApis.Utilities.ApiRoutes;

namespace GromaxMobileApis.Services
{


    public class DatabaseService : IDatabaseService
    {
        private IDbConnection _db;
        private string _conString;
        private IUser _user;
        public DatabaseService(IDbConnection db, IConfiguration configuration, IUser user)
        {
            _db = db;
            _conString = configuration.GetConnectionString("DefaultConnection");
            _user = user;
        }

        public async Task<IEnumerable<string>> BannerUrl()
        {
            try
            {
                var vals = await _db.QueryAsync<string>(
                "BannerUrlsp",
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GenerateSalesEnquiry(SalesEnquiryMaster s, string dealerCode, string username, string mobileNumber)
        {
            try
            {
                var parameters = new
                {
                    StateCode = s.StateCode,
                    DistrictCode = s.DistrictCode,
                    TehsilCode = s.TehsilCode,
                    VillageCode = s.VillageCode,

                    mobileNo = mobileNumber,
                    username = username,
                    DealerCode = s.DealerCode,
                    ProspectName = s.ProspectName,
                    ProspectMobile = s.ProspectMobile,
                    EnquirySource = s.EnquirySource,
                    EnquirySubSource = s.EnquirySubSource,
                    NextFollowUpDate = s.NextFollowUpDate,
                    ProspectDistrict = s.ProspectDistrict,
                    ProspectPinCode = s.ProspectPinCode,
                    ProspectTehsil = s.ProspectTehsil,
                    ProspectVillage = s.ProspectVillage,
                    fatherName = s.FatherName,
                    ProspectType = s.ProspectType,
                    SalesmanName = s.SalesmanName,
                    SalesmanNumber = s.SalesmanNumber,
                    ExpectedPurchaseDate = s.ExpectedPurchaseDate,
                    EnquiryStatus = s.EnquiryStatus,
                    InterestedModel = s.InterestedModel,
                    Varient_BOMCode = s.VarientOrBOMCode,
                    ProductUse = s.ProductUse,
                    ActionPlanned = s.ActionPlanned,
                    HPCategory = s.HPCategory,
                    DriveType = s.DriveType,
                    SubSubSource = s.SubSubSource,
                    customAction = s.customAction
                };

                var vals = _db.QueryFirstOrDefault<int>(
                "GenerateSalesEnquiry",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> GetFinanceMaster(FinanceMastersalesenqiddto model)
        {
            try
            {
                var parameters = new { SalesEnquiryMasterId = model.SalesEnquiryMasterId };
                var vals = await _db.QueryAsync(
                "usp_getfinancemaster",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> GetHistoryEnquiry(HistoryEnquiry model)
        {
            try
            {
                var parameters = new { SalesMasterId = model.SalesEnquiryMasterId };
                var vals = await _db.QueryAsync(
                "GetHistoryEnquiryonSalesMasterid",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> GetInstallationMaster()
        {
            try
            {
                var vals = await _db.QueryAsync(
                "GetInstallationMasterSp",
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
                "GetLeadsFollowUpSpv1",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> Getmodelmasterlist()
        {
            try
            {

                var vals = await _db.QueryAsync(
                "Getmodelmastersp",
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> GetPendingSalesFollowupList(SalesEnquiryMaster model, string Mobile, string Dealercode, string LoginPosition, string username)
        {
            try
            {
                var parameters = new { mobile = Mobile, dealercode = Dealercode, source = model.EnquirySource, Model = model.InterestedModel, enquirytype = model.EnquiryStatus, followuptype = model.EnquiryType, loginas = LoginPosition, username = username };
                var vals = await _db.QueryAsync(
                "PendingSalesFollowupListsp",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> GetSalesEnquiry(SalesEnquiryMasterdto m, string Dealercode, string username, string LoginPosition)
        {
            try
            {
                var parameters = new { ProspectNumber = m.ProspectMobile, Dealercode = Dealercode, username = username, LoginPosition = LoginPosition };
                var vals = await _db.QueryAsync(
                "GetSalesEnquiryMasterSpv1",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> GetSalesTarget(string dealercode, string DateFilter, string LoginPosition, string username)
        {
            try
            {

                var parameters = new { dealercode = dealercode, DateFilter = DateFilter, loginas = LoginPosition, username = username };
                var vals = await _db.QueryAsync(
                //"GetSalesTargetSp",
                "GetSalesTargetSpv1",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Loginmaster>> GetUserData(Loginmaster model)
        {
            try
            {
                var parameters = new { MobileNumber = model.MobileNo, DeviceId = model.DeviceId, DeviceModel = model.ModelDevice, DeviceBrand = model.DeviceBrand, Platform = model.PlatformType, Action = 1 };
                var vals = (await _db.QueryAsync<Loginmaster>(
                "loginuser",
                param: parameters,
                commandType: CommandType.StoredProcedure)).ToList();
                return vals;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> InsertFinanceMaster(FinanceMaster model)
        {
            try
            {
                var parameters = new { SalesEnquiryMasterId = model.SalesEnquiryMasterId, HistoryId = model.HistoryId, ExchangeRequired = model.ExchangeRequired, FinalOn_RoadPrice = model.FinalOn_RoadPrice, OtherExpenses = model.OtherExpenses, ProductSupport = model.ProductSupport, NdpOfVarient = model.NdpOfVarient, NetMargin = model.NetMargin, CustomerExpecValueForExchangemodel = model.CustomerExpecValueForExchangemodel, DealerEstmtdCostOfExchangetractor = model.DealerEstmtdCostOfExchangetractor, FinalValOfferedForExchangeModel = model.FinalValOfferedForExchangeModel, PaymentMode = model.PaymentMode, DpAmount = model.DpAmount, LoanAmount = model.LoanAmount, FinancerName = model.FinancerName, FinanceStatus = model.FinanceStatus, KycCollected = model.KycCollected, CibilChecked = model.CibilChecked, CibilScore = model.CibilScore, FileStatus = model.FileStatus, LoanType = model.LoanType, TotalAmount = model.TotalAmount, ExchangeBrand = model.ExchangeBrand, ExchangeModel = model.ExchangeModel, ExchangeHPCategory = model.ExchangeHPCategory, ExchangeModelName = model.ExchangeModelName };

                var vals = _db.QueryFirstOrDefault<int>(
                "usp_InsertFinanceMasterv1",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> InsertHistoryEnquiry(HistoryEnquiry model, string P)
        {
            try
            {
                var parameters = new { salesEnqmasterid = model.SalesEnquiryMasterId, callstatus = model.CallStatus, Remarks = model.Remark, EnquiryType = model.CallLeads, NextFollowUpDate = model.NextFollowUpDate, expectedDeliveryDate = model.ExpectedDeliveryDate, closureReason = model.closureReason, subClosureReason = model.subClosureReason, followenquiryStatus = model.followenquiryStatus, PlatformType = P };
                var vals = _db.QueryFirstOrDefault<int>(
                "InsertHistoryEnquiry",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task InsertToken(string MobileNo, string id)
        {
            try
            {
                var parameters = new { MobileNumber = MobileNo, id = id };
                var vals = await _db.QueryFirstOrDefaultAsync<int>(
                "insertToken",
                param: parameters,
                commandType: CommandType.StoredProcedure);



            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<string> LoginUser(string MobileNo)
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

        public async Task<bool> Logout(string MobileNumber)
        {
            try
            {
                var parameters = new { MobileNumber = MobileNumber, Action = 2 };
                var vals = await _db.QueryFirstOrDefaultAsync<int>(
                "loginuser",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                if (vals > 0)
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> updtSalesCustomerProfile(SalesEnquiryMaster model, string p)
        {
            try
            {
                var parameters = new { Id = model.Id, ProspectDistrict = model.ProspectDistrict, ProspectTehsil = model.ProspectTehsil, ProspectVillage = model.ProspectVillage, ProspectPinCode = model.ProspectPinCode, EnquiryStatus = model.EnquiryStatus, ActionPlanned = model.ActionPlanned, ProductUse = model.ProductUse, LandHolding = model.LandHolding, customAction = model.customAction, StateCode = model.StateCode, DistrictCode = model.DistrictCode, TehsilCode = model.TehsilCode, VillageCode = model.VillageCode, SalesmanName = model.SalesmanName, SalesmanNumber = model.SalesmanNumber, PlatformType = p };

                var vals = await _db.ExecuteAsync(
                "usp_updtSalesCustomerProfile",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> updtSalesCustomerEnquiry(SalesEnquiryMaster model)
        {
            try
            {
                var parameters = new
                {
                    Id = model.Id,
                    ProspectName = model.ProspectName,
                    NextFollowUpDate = model.NextFollowUpDate,
                    FatherName = model.FatherName,
                    EnquirySource = model.EnquirySource,
                    EnquirySubSource = model.EnquirySubSource,
                    ExpectedDeliveryDate = model.ExpectedDeliveryDate,
                    InterestedModel = model.InterestedModel,
                    VarientOrBOMCode = model.VarientOrBOMCode,
                    HPCategory = model.HPCategory,
                    DriveType = model.DriveType,
                    SubSubSource = model.SubSubSource,
                };
                var vals = await _db.ExecuteAsync(
                "usp_updtSalesCustomerEnquiry",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ValidateToken(string MobileNo, string id)
        {
            try
            {
                var parameters = new { MobileNumber = MobileNo, id = id };
                var vals = await _db.QueryFirstOrDefaultAsync<int>(
                "ValidateTokensp",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                if (vals > 0)
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<VersionMasterDto>> VersionMaster(string platform)
        {
            try
            {
                var parameters = new { platform = platform };
                List<VersionMasterDto> list = (await _db.QueryAsync<VersionMasterDto>(
                "GetVersion",
                param: parameters,
                commandType: CommandType.StoredProcedure)).ToList();
                return list;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> InsertInstallationImg(DataTable dt)
        {
            try
            {
                var parameters = new { dt = dt };
                var vals = await _db.ExecuteAsync(
                "usp_insertInstallationImg",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> InsertInstallationImgv1(DataTable dt, string workingHrs)
        {
            try
            {
                var parameters = new { dt = dt, workingHrs };
                var vals = await _db.ExecuteAsync(
                "usp_insertInstallationImgv1",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> InsertReturnRequestMaster(ReturnRequestMaster model)
        {
            try
            {
                var parameters = new { SalesEnquiryMasterId = model.SalesEnquiryMasterId, ReturnDate = model.ReturnDate, Reason = model.Reason, Remarks = model.Remarks };

                var vals = await _db.ExecuteAsync("usp_insertReturnRequestMaster", param: parameters, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<ReturnRequestMaster>> GetReturnRequestMaster(ReturnRequestMaster model)
        {
            try
            {
                var parameters = new { SalesEnquiryMasterId = model.SalesEnquiryMasterId };
                var vals = await _db.QueryAsync<ReturnRequestMaster>(
                "usp_getReturnRequestMaster",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> getSevenDaySalesEnquiryDelivery(string Mobile, string Dealercode, string username, string loginposition)
        {
            try
            {
                var parameters = new { mobile = Mobile, dealercode = Dealercode, username = username, loginposition = loginposition };
                var vals = await _db.QueryAsync(
                //"usp_getSevenDaySalesEnquiryDelivery",
                "usp_getSevenDaySalesEnquiryDeliveryv1",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> getSevenDaySalesEnquiryDeliveryV1(SuperHotEnquiry model, string Mobile, string Dealercode, string username, string loginposition)
        {
            try
            {
                var parameters = new
                {
                    mobile = Mobile,
                    dealercode = Dealercode,
                    username = username,
                    loginposition = loginposition,
                    model
                .PageSize,
                    model.RowStart,
                    model.Download
                };
                var vals = await _db.QueryAsync(
                //"usp_getSevenDaySalesEnquiryDelivery",
                "usp_getSevenDaySalesEnquiryDeliveryv2",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> GetCustomerAddressFilter(string Type, string Name, string Code)
        {
            try
            {
                var parameter = new { Type = Type, Name = Name, Code = Code };
                var vals = await _db.QueryAsync("usp_GetCustomerAddressFilter", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception("Database Error"); }
        }

        public async Task<IEnumerable<dynamic>> GetClosureMasterdb()
        {
            try
            {
                var vals = await _db.QueryAsync("usp_GetClosureMasterdb", commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception("Database Error"); }
        }

        public async Task<IEnumerable<dynamic>> GetInventoryData(StockMaster model)
        {
            try
            {
                var parameters = new { DealerCode = model.DealerCode, Filter = model.Filter };
                var vals = await _db.QueryAsync(
                "usp_getInventoryData",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> MarkStockSolddb(MarkStockSoldModel model)
        {
            try
            {
                var parameter = new
                {
                    SalesEnquiryMasterId = Guid.Parse(model.SalesEnquiryMasterId),
                    StockMasterId = Guid.Parse(model.StockMasterId),
                };
                var result = await _db.ExecuteScalarAsync<int>("usp_MarkStockSoldAndUpdateEnquiry", param: parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetStatedb(string dealercode)
        {
            try
            {
                var parameter = new { dealercode = dealercode };
                var result = await _db.QueryAsync("usp_Getstate", param: parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> GenerateSalesEnquiryv1(SalesEnquiryMaster s, string dealerCode, string username, string mobileNumber, string platformtype)
        {
            try
            {
                var parameters = new
                {
                    StateCode = s.StateCode,
                    DistrictCode = s.DistrictCode,
                    TehsilCode = s.TehsilCode,
                    VillageCode = s.VillageCode,
                    Remark = s.Remark,
                    Surname = s.Surname,
                    mobileNo = mobileNumber,
                    username = username,
                    DealerCode = s.DealerCode,
                    ProspectName = s.ProspectName,
                    ProspectMobile = s.ProspectMobile,
                    EnquirySource = s.EnquirySource,
                    EnquirySubSource = s.EnquirySubSource,
                    NextFollowUpDate = s.NextFollowUpDate,
                    ProspectDistrict = s.ProspectDistrict,
                    ProspectPinCode = s.ProspectPinCode,
                    ProspectTehsil = s.ProspectTehsil,
                    ProspectVillage = s.ProspectVillage,
                    fatherName = s.FatherName,
                    ProspectType = s.ProspectType,
                    SalesmanName = s.SalesmanName,
                    SalesmanNumber = s.SalesmanNumber,
                    ExpectedPurchaseDate = s.ExpectedPurchaseDate,
                    EnquiryStatus = s.EnquiryStatus,
                    InterestedModel = s.InterestedModel,
                    Varient_BOMCode = s.VarientOrBOMCode,
                    ProductUse = s.ProductUse,
                    ActionPlanned = s.ActionPlanned,
                    HPCategory = s.HPCategory,
                    DriveType = s.DriveType,
                    SubSubSource = s.SubSubSource,
                    customAction = s.customAction,
                    PlatformType = platformtype
                };

                var vals = _db.QueryFirstOrDefault<int>(
                "GenerateSalesEnquiryv1",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> GetSalesEnquiryMaster(SalesEnquiryMasterFiltered model, string LoginPosition, string Username)
        {
            try
            {
                //var parameter = new { Dealercode = DealerCode };
                var parameter = new
                {
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
                var vals = await _db.QueryAsync("usp_MobileGetSalesEnquiryMasterSp", param: parameter, commandType: CommandType.StoredProcedure);

                //var vals = await _db.QueryAsync("usp_WebGetSalesEnquiryMasterSp", param: parameter, commandType: CommandType.StoredProcedure);
                //var vals = await _db.QueryAsync("usp_WebGetSalesEnquiryMasterSpv1", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> InsertFinanceMasterv1(FinanceMaster model)
        {
            try
            {
                var parameters = new { SalesEnquiryMasterId = model.SalesEnquiryMasterId, HistoryId = model.HistoryId, ExchangeRequired = model.ExchangeRequired, FinalOn_RoadPrice = model.FinalOn_RoadPrice, OtherExpenses = model.OtherExpenses, ProductSupport = model.ProductSupport, NdpOfVarient = model.NdpOfVarient, NetMargin = model.NetMargin, CustomerExpecValueForExchangemodel = model.CustomerExpecValueForExchangemodel, DealerEstmtdCostOfExchangetractor = model.DealerEstmtdCostOfExchangetractor, FinalValOfferedForExchangeModel = model.FinalValOfferedForExchangeModel, PaymentMode = model.PaymentMode, DpAmount = model.DpAmount, LoanAmount = model.LoanAmount, FinancerName = model.FinancerName, FinanceStatus = model.FinanceStatus, KycCollected = model.KycCollected, CibilChecked = model.CibilChecked, CibilScore = model.CibilScore, FileStatus = model.FileStatus, LoanType = model.LoanType, TotalAmount = model.TotalAmount, ExchangeBrand = model.ExchangeBrand, ExchangeModel = model.ExchangeModel, ExchangeHPCategory = model.ExchangeHPCategory, ExchangeModelName = model.ExchangeModelName, FinalLiquidationPrice = model.FinalLiquidationPrice, FinanceFI = model.FinanceFI, SanctionDone = model.SanctionDone, StockAvailable = model.StockAvailable };

                var vals = _db.QueryFirstOrDefault<int>(
                "usp_InsertFinanceMasterv2",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> updtSalesCustomerEnquiryv1(SalesEnquiryMaster model)
        {
            try
            {
                var parameters = new
                {
                    Id = model.Id,
                    ProspectName = model.ProspectName,
                    NextFollowUpDate = model.NextFollowUpDate,
                    FatherName = model.FatherName,
                    EnquirySource = model.EnquirySource,
                    EnquirySubSource = model.EnquirySubSource,
                    ExpectedDeliveryDate = model.ExpectedDeliveryDate,
                    InterestedModel = model.InterestedModel,
                    VarientOrBOMCode = model.VarientOrBOMCode,
                    HPCategory = model.HPCategory,
                    DriveType = model.DriveType,
                    SubSubSource = model.SubSubSource,
                    Remark = model.Remark,
                    Surname = model.Surname,
                    DistrictCode = model.DistrictCode,
                    TehsilCode = model.TehsilCode,
                    VillageCode = model.VillageCode,
                    StateCode = model.StateCode,
                    ProspectDistrict = model.ProspectDistrict,
                    ProspectTehsil = model.ProspectTehsil,
                    ProspectVillage = model.ProspectVillage,
                    ProspectPinCode = model.ProspectPinCode

                };
                var vals = await _db.ExecuteAsync(
                "usp_updtSalesCustomerEnquiryv1",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> GetSalesEnquiryOnPendingFollowUpdb(SalesEnquiryMasterdto model)
        {
            try
            {
                var parameter = new
                {
                    id = Guid.Parse(model.id)
                };
                var vals = await _db.QueryAsync("usp_GetSalesEnquiryOnPendingFollowUp", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetSalesEnquiryMasterPagination(SalesEnquiryMasterFiltered model, string LoginPosition, string Username)
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
                var vals = await _db.QueryAsync("usp_MobileGetSalesEnquiryMasterPaginationSp", param: parameter, commandType: CommandType.StoredProcedure);

                //var vals = await _db.QueryAsync("usp_WebGetSalesEnquiryMasterSp", param: parameter, commandType: CommandType.StoredProcedure);
                //var vals = await _db.QueryAsync("usp_WebGetSalesEnquiryMasterSpv1", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetReturnRequestMasterv1(GetReturnRequestMasterv1 model, string loginas, string username)
        {
            try
            {
                var parameter = new
                {
                    DealerCode = model.DealerCode,
                    SearchValue = model.SearchValue,
                    SearchType = model.SearchType,
                    ReturnType = model.ReturnType,
                    loginas = loginas,
                    username = username
                };
                var vals = await _db.QueryAsync("usp_GetReturnRequestMasterv1", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> GenerateReturnRequest(GenerateReturnRequestv1 model, string p)
        {
            try
            {
                string fileNamesJson = model.FileNamesurl != null && model.FileNamesurl.Any()
           ? JsonSerializer.Serialize(model.FileNamesurl)
           : null;
                var parameter = new
                {
                    CustomerName = model.CustomerName,
                    ChasisNumber = model.ChasisNumber,
                    Mobile = model.Mobile,
                    SaleDate = model.SaleDate,
                    SoldBy = model.SoldBy,
                    Reason = model.Reason,
                    Remarks = model.Remarks,
                    SalesEnquiryMasterId = model.SalesEnquiryMasterId,
                    DealerCode = model.DealerCode,
                    Model = model.Modeln,
                    fileNamesJson = fileNamesJson,
                    PlatformType = p,
                    LoginAs = _user.GetPositionName()

                };
                var vals = await _db.ExecuteAsync("usp_GenerateReturnRequest", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetSalesDetailsForReturnRequest(GetReturnRequestMasterv1 model)
        {
            try
            {
                var parameter = new
                {
                    SearchValue = model.SearchValue,
                    SearchType = model.SearchType,
                    DealerCode = model.DealerCode,
                    LoginAs = _user.GetPositionName()
                };
                var vals = await _db.QueryAsync("usp_GetSalesDetailsForReturnRequest", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> ReturnRequestApprovaldb(ReturnRequestApproval model, string p)
        {
            try
            {
                var parameter = new
                {
                    Position = model.Position,
                    Status = model.Status,
                    Remarks = model.Remarks,
                    ReturnRequestId = Guid.Parse(model.ReturnRequestId),
                    Platformtype = p,
                    username = _user.GetUserName(),
                    LoginName = _user.GetName()
                };
                var vals = await _db.ExecuteAsync("usp_UpdateReturnRequestStatus", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> ReturnRequestApprovaldbv1(ReturnRequestApproval model)
        {
            try
            {
                var parameter = new
                {
                    Position = model.Position,
                    Status = model.Status,
                    Remarks = model.Remarks,
                    ReturnRequestId = Guid.Parse(model.ReturnRequestId),
                    Platformtype = _user.GetPlatform(),
                    username = _user.GetUserName(),
                    LoginName = _user.GetName()
                };
                var vals = await _db.ExecuteAsync("usp_UpdateReturnRequestStatusv1", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetStatusHistorydb(ReturnRequestApproval model)
        {
            try
            {
                var parameter = new
                {
                    ReturnRequestId = Guid.Parse(model.ReturnRequestId)
                };
                var vals = await _db.QueryAsync("usp_GetReturnRequestStatusHistory", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> InsertFinanceMasterv2(FinanceMaster model, string P)
        {
            try
            {
                var parameters = new
                {
                    SalesEnquiryMasterId = model.SalesEnquiryMasterId,
                    HistoryId = model.HistoryId,
                    ExchangeRequired = model.ExchangeRequired,
                    FinalOn_RoadPrice = model.FinalOn_RoadPrice,
                    OtherExpenses = model.OtherExpenses,
                    ProductSupport = model.ProductSupport,
                    NdpOfVarient = model.NdpOfVarient,
                    NetMargin = model.NetMargin,
                    CustomerExpecValueForExchangemodel = model.CustomerExpecValueForExchangemodel,
                    DealerEstmtdCostOfExchangetractor = model.DealerEstmtdCostOfExchangetractor,
                    FinalValOfferedForExchangeModel = model.FinalValOfferedForExchangeModel,
                    PaymentMode = model.PaymentMode,
                    DpAmount = model.DpAmount,
                    LoanAmount = model.LoanAmount,
                    FinancerName = model.FinancerName,
                    FinanceStatus = model.FinanceStatus,
                    KycCollected = model.KycCollected,
                    CibilChecked = model.CibilChecked,
                    CibilScore = model.CibilScore,
                    FileStatus = model.FileStatus,
                    LoanType = model.LoanType,
                    TotalAmount = model.TotalAmount,
                    ExchangeBrand = model.ExchangeBrand,
                    ExchangeModel = model.ExchangeModel,
                    ExchangeHPCategory = model.ExchangeHPCategory,
                    ExchangeModelName = model.ExchangeModelName,
                    FinalLiquidationPrice = model.FinalLiquidationPrice,
                    FinanceFI = model.FinanceFI,
                    SanctionDone = model.SanctionDone,
                    StockAvailable = model.StockAvailable,
                    TabType = model.TabType,
                    PlatformType = P
                };

                var vals = _db.QueryFirstOrDefault<int>(
                "usp_InsertFinanceMasterv3",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> GetSalesEnquirySearchByMobileNumberdb(SalesEnquirySearchdto model, string Loginas, string UserName)
        {
            try
            {
                var parameter = new
                {
                    CustomerMobileNumber = model.ProspectMobileNumber,
                    LoginAs = Loginas,
                    UserMail = UserName
                };
                var vals = await _db.QueryAsync("usp_GetSalesEnquiry_MobNo", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> DonwloadInventoryReportdb(StockFilterModel model, string Loginas, string UserName)
        {
            try
            {
                var parameter = new
                {
                    startdate = model.StartDate,
                    enddate = model.EndDate,
                    AmMail = model.AmMail,
                    StateHeadMail = model.ShMail,
                    TmMail = model.TmMail,
                    DealerMail = model.DealerMail,
                    AgingStartDay = model.AgingStartDay,
                    AgingEndDay = model.AgingEndDay,
                    loginas = Loginas,
                    LoginMail = UserName,
                    model.StockStatus,
                    model.location,
                    model.StateCode
                };
                var vals = await _db.QueryAsync("usp_DownloadInventoryData", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> SearchInventoryReportdb(SearchInventoryReportDto model, string Loginas, string UserName)
        {
            try
            {
                var parameter = new
                {
                    ChassisNo = model.ChassisNo,
                    loginas = Loginas,
                    LoginMail = UserName
                };
                var vals = await _db.QueryAsync("usp_SearchInventoryReport", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> SearchAvailableInventoryReportdb(SearchInventoryReportDto model, string Loginas, string UserName)
        {
            try
            {
                var parameter = new
                {
                    ChassisNo = model.ChassisNo,
                    loginas = Loginas,
                    LoginMail = UserName
                };
                var vals = await _db.QueryAsync("usp_SearchAvailableInventoryReport", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> DonwloadAvailableInventoryReportdb(StockFilterModel model, string Loginas, string UserName)
        {
            try
            {
                var parameter = new
                {
                    startdate = model.StartDate,
                    enddate = model.EndDate,
                    AmMail = model.AmMail,
                    StateHeadMail = model.ShMail,
                    TmMail = model.TmMail,
                    DealerMail = model.DealerMail,
                    AgingStartDay = model.AgingStartDay,
                    AgingEndDay = model.AgingEndDay,
                    loginas = Loginas,
                    LoginMail = UserName
                };
                var vals = await _db.QueryAsync("usp_DownloadAvailableInventoryData", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<DataSet> GetInstallationMasterv1(InstallationMasterdto model, string loginas, string loginmail)
        {
            try
            {
                using (var conn = new SqlConnection(_conString))
                using (var cmd = new SqlCommand("GetInstallationMasterSpv1", conn))
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LoginPosition", loginas);
                    cmd.Parameters.AddWithValue("@StateName", model.StateName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Dealership", model.Dealership ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsDownload", model.IsDownload ?? (object)DBNull.Value);
                    cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = model.StartDate ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = model.EndDate ?? (object)DBNull.Value;
                    //cmd.Parameters.AddWithValue("@ShMail", model.ShMail ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@AmMail", model.AmMail ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@TmMail", model.TmMail ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@DealerMail", model.DealerMail ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LoginMail", loginmail ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", model.Status ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RowStart", !string.IsNullOrEmpty(model.RowStart) ? int.Parse(model.RowStart) : 0);
                    cmd.Parameters.AddWithValue("@PageSize", !string.IsNullOrEmpty(model.PageSize) ? int.Parse(model.PageSize) : 0);

                    var ds = new DataSet();

                    await conn.OpenAsync();
                    adapter.Fill(ds); // fills all result sets into tables in dataset

                    return ds;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database Error", ex);
            }
            //try
            //{
            //    var Parameter = new
            //    {
            //        LoginPosition = loginas,
            //        ShMail = model.ShMail,
            //        AmMail = model.AmMail,
            //        TmMail = model.TmMail,
            //        DealerMail = model.DealerMail,
            //        LoginMail = loginmail,
            //        RowStart = model.RowStart != "" && model.RowStart is not null ? int.Parse(model.RowStart) : 0,
            //        PageSize = model.PageSize != "" && model.PageSize is not null ? int.Parse(model.PageSize) : 0,
            //    };
            //    var vals = await _db.QueryAsync(
            //    "GetInstallationMasterSpv1",
            //    param: Parameter,
            //    commandType: CommandType.StoredProcedure);
            //    return vals;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
        }

        public async Task<int> MarkStockSolddbv1(MarkStockSoldModel model, string p, string username)
        {
            try
            {
                var parameter = new
                {
                    SalesEnquiryMasterId = Guid.Parse(model.SalesEnquiryMasterId),
                    StockMasterId = Guid.Parse(model.StockMasterId),
                    DeliveryDate = model.DeliveryDate,
                    PlatformType = p,
                    username = username
                    //IsRetailed = model.IsRetailedSales
                };
                var result = await _db.ExecuteScalarAsync<int>("usp_MarkStockSoldAndUpdateEnquiryv1", param: parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> CheckFillFinanceDatadb(SalesEnquiryMasterdto model)
        {
            try
            {
                var parameter = new
                {
                    SalesEnquiryMasterId = Guid.Parse(model.SalesEnquiryId),
                    //StockMasterId = Guid.Parse(model.StockMasterId),
                    //DeliveryDate = model.DeliveryDate
                };
                //var result = await _db.ExecuteScalarAsync<int>("", param: parameter, commandType: CommandType.StoredProcedure);
                var result = await _db.QueryAsync("usp_CheckFillFinanceStatus", param: parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetSalemanListdb(SalesmanGetdto model, string loginas, string loginmail)
        {
            try
            {
                var parameter = new
                {
                    LoginPosition = loginas,
                    LoginMail = loginmail,
                    ShMail = model.ShMail,
                    AmMail = model.AmMail,
                    TmMail = model.TmMail,
                    DealerMail = model.DealerMail,
                    //Status = model.Status
                    //SalesEnquiryMasterId = Guid.Parse(model.SalesEnquiryId),
                    //StockMasterId = Guid.Parse(model.StockMasterId),
                    //DeliveryDate = model.DeliveryDate
                };
                //var result = await _db.ExecuteScalarAsync<int>("", param: parameter, commandType: CommandType.StoredProcedure);
                var result = await _db.QueryAsync("usp_GetSalesmanList", param: parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> InsertSalesmandb(SalesmanMaster model, string p)
        {
            try
            {
                var parameters = new
                {
                    SalesmanName = model.SalesmanName,
                    MobileNo = model.MobileNo,
                    DealerCode = model.DealerCode,
                    DealerName = model.DealerName,
                    PlatformType = p,
                    model.Status
                };

                var vals = _db.QueryFirstOrDefault<int>(
                "usp_InsertSalesman",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> InsertSalesmandbv1(SalesmanMasterv1 model, string p, string file1, string file2, string file3)
        {
            try
            {
                var parameters = new
                {
                    SalesmanName = model.SalesmanName,
                    MobileNo = model.MobileNo,
                    DealerCode = model.DealerCode,
                    DealerName = model.DealerName,
                    PlatformType = p,
                    model.Status,
                    model.kycstatus,
                    file1 = file1,
                    file2 = file2,
                    file3 = file3,
                    username = _user.GetUserName(),
                    loginas = _user.GetPositionName(),
                    loginname = _user.GetName(),
                    model.insertORupdt,
                    model.dateofseperation,
                    id = model.Id,
                    dateofjoin = Convert.ToDateTime(model.dateofjoin)
                };

                var vals = _db.QueryFirstOrDefault<int>(
                "usp_InsertSalesmanv1",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> GetInstallationImagesdb(InstallationImage model)
        {
            try
            {
                var parameter = new
                {
                    Id = Guid.Parse(model.Id),
                };
                //var result = await _db.ExecuteScalarAsync<int>("", param: parameter, commandType: CommandType.StoredProcedure);
                var result = await _db.QueryAsync("usp_GetInstallationImages", param: parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetSalesmanByDealerCodedb(SalesmanMaster model)
        {
            try
            {
                var parameter = new
                {
                    DealerCode = model.DealerCode,
                };
                //var result = await _db.ExecuteScalarAsync<int>("", param: parameter, commandType: CommandType.StoredProcedure);
                var result = await _db.QueryAsync("usp_GetSaleMan_Dealercode", param: parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<BussinessPerformanceResult> DownloadBussinessPerformancedb(BussinessPerformance model, string loginas, string username)
        {
            try
            {
                var parameter = new
                {
                    BoxFilter = model.BoxFilter,
                    Startdate = model.Startdate,
                    Enddate = model.Enddate,
                    AmMail = model.AmMail,
                    ShMail = model.ShMail,
                    TmMail = model.TmMail,
                    DealerMail = model.DealerMail,
                    LoginPosition = loginas,
                    LoginMail = username,
                    Source = model.Source,
                    model.subSource,
                    model.Location,
                    model.StateCode
                };

                using var multi = await _db.QueryMultipleAsync(
                    "usp_DownloadBussinessPerformance",
                    param: parameter,
                    commandType: CommandType.StoredProcedure
                );

                List<dynamic> CleanNullRows(IEnumerable<dynamic> data) =>
                    (data ?? Enumerable.Empty<dynamic>())
                        .Where(row => row is IDictionary<string, object> dict && dict.Values.Any(v => v != null))
                        .ToList();

                return new BussinessPerformanceResult
                {
                    //GetBoxes = CleanNullRows(await multi.ReadAsync<dynamic>()),
                    GetList = CleanNullRows(await multi.ReadAsync<dynamic>())
                    //TerritoryManagers = CleanNullRows(await multi.ReadAsync<dynamic>()),
                    //Dealers = CleanNullRows(await multi.ReadAsync<dynamic>())
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<dynamic>> GetSalesEnquiryForFollowSearchByMobileNumberdb(SalesEnquirySearchdto model, string Loginas, string UserName)
        {
            try
            {
                var parameter = new
                {
                    CustomerMobileNumber = model.ProspectMobileNumber,
                    LoginAs = Loginas,
                    UserMail = UserName
                };
                var vals = await _db.QueryAsync("usp_GetSalesEnquiryForFollowup_MobNo", param: parameter, commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> DownloadGetPendingSalesFollowupListdb(EnquiryFilterRequest model, string Mobile, string Dealercode, string LoginPosition, string username)
        {
            try
            {
                try
                {
                    var parameters = new
                    {
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
                    "DownloadPendingSalesFollowupListsp",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
                    return vals;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public Task<IEnumerable<dynamic>> GetUserDetaildb(string MobileNo)
        {
            try
            {
                var parameter = new { MobileNo = MobileNo };
                var values = _db.QueryAsync("usp_GetUserDetails", param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public Task<int> CustomerExistsdb(SalesEnquiryMasterdto model)
        {
            try
            {
                var parameter = new { ProspectMobile = model.ProspectMobile, id = Guid.Parse(model.id) };
                var values = _db.QuerySingleOrDefaultAsync<int>("usp_CustomerExists",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> updtSalesCustomerEnquiryv2(SalesEnquiryMaster model, string p)
        {
            try
            {
                var parameters = new
                {
                    Id = model.Id,
                    ProspectName = model.ProspectName,
                    NextFollowUpDate = model.NextFollowUpDate,
                    FatherName = model.FatherName,
                    EnquirySource = model.EnquirySource,
                    EnquirySubSource = model.EnquirySubSource,
                    ExpectedDeliveryDate = model.ExpectedDeliveryDate,
                    InterestedModel = model.InterestedModel,
                    VarientOrBOMCode = model.VarientOrBOMCode,
                    HPCategory = model.HPCategory,
                    DriveType = model.DriveType,
                    SubSubSource = model.SubSubSource,
                    Remark = model.Remark,
                    Surname = model.Surname,
                    DistrictCode = model.DistrictCode,
                    TehsilCode = model.TehsilCode,
                    VillageCode = model.VillageCode,
                    StateCode = model.StateCode,
                    ProspectDistrict = model.ProspectDistrict,
                    ProspectTehsil = model.ProspectTehsil,
                    ProspectVillage = model.ProspectVillage,
                    ProspectPinCode = model.ProspectPinCode,
                    ProspectMobile = model.ProspectMobile,
                    PlatformType = p

                };
                var vals = await _db.ExecuteAsync(
                "usp_updtSalesCustomerEnquiryv2",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Loginmaster>> GetUserDatav1(Loginmaster model)
        {
            try
            {
                var parameters = new { MobileNumber = model.MobileNo, DeviceId = model.DeviceId, DeviceModel = model.ModelDevice, DeviceBrand = model.DeviceBrand, Platform = model.PlatformType, Action = 1, FCMToken = model.FCMToken };
                var vals = (await _db.QueryAsync<Loginmaster>(
                "loginuserappv1",
                param: parameters,
                commandType: CommandType.StoredProcedure)).ToList();
                return vals;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> BlankFCMToken(string MobileNo)
        {
            try
            {
                var parameter = new { MobileNo = MobileNo };
                var values = await _db.ExecuteAsync("usp_BlankFCMToken",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> UpdateFcmTokendb(string MobileNo, string FCMToken)
        {
            try
            {
                var parameter = new { MobileNo = MobileNo, FCMToken = FCMToken };
                var values = await _db.ExecuteAsync("usp_UpdateFcmToken",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public Task<IEnumerable<dynamic>> DealerlistStatewisedb(string StateCode)
        {
            try
            {
                var parameter = new
                {
                    StateCode = StateCode,
                    LoginAs = _user.GetPositionName()
                };
                var values = _db.QueryAsync("usp_DealerListStatewise", param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> DlrToDlrStockTrfdb(StockMasterdto model)
        {
            try
            {
                var parameter = new
                {
                    DlrFrom = model.DealerCodeFrom,
                    DlrTo = model.DealerCodeTo,
                    ChassisNo = model.ChassisNo,
                    StockId = Guid.Parse(model.StockId),
                    username = _user.GetUserName(),
                    loginas = _user.GetPositionName(),
                    platform = _user.GetPlatform()
                };
                var values = await _db.ExecuteAsync("usp_DlrToDlrStrkTrf",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> StockTrfApprovaldb(StockTrfAprovalDto model)
        {
            try
            {
                var parameter = new
                {
                    Position = model.Position,
                    Status = model.Status,
                    Reason = model.Reason,
                    Remarks = model.Remarks,
                    DlrToDlrStkTrfMasterId = Guid.Parse(model.DlrToDlrStkTrfMasterId),
                    username = _user.GetUserName(),
                    loginas = _user.GetPositionName(),
                    platform = _user.GetPlatform()
                };
                var values = await _db.ExecuteAsync("usp_DlrToDlrStrkTrfApproval",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetStockTrfApprovaldb(string Loginas, string Username)
        {
            try
            {
                var parameter = new { Loginas = Loginas, Username = Username };
                var values = await _db.QueryAsync("usp_GetStktrfApprovalDetail", param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetFcmTokendb()
        {
            try
            {

                var vals = await _db.QueryAsync(
                "usp_GetFcmToken",
                commandType: CommandType.StoredProcedure);
                return vals;

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetStockForStockTransferdb(GetStockForStkTrfdto model, string Loginas, string Username)
        {
            try
            {
                var parameter = new { loginas = Loginas, LoginMail = Username, SearchBy = model.SearchBy };
                var values = await _db.QueryAsync("usp_SearchAvailableInventoryReportForStkTrf", param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> UpdateSalesmanStatusdb(SalesmanStatusdto model)
        {
            try
            {
                var parameter = new { Id = Guid.Parse(model.Id), ActiveStatus = model.ActiveStatus };
                var values = await _db.ExecuteAsync("usp_UpdateSalesmanStatus",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetNotRetailedSales(RetailedSalesFilter model, string LoginPosition, string username)
        {
            try
            {
                var parameter = new
                {
                    LoginPosition = LoginPosition,
                    ShMail = model.ShMail,
                    AmMail = model.AmMail,
                    TmMail = model.TmMail,
                    DealerMail = model.DealerMail,
                    LoginMail = username,
                    model.PageSize,
                    model.RowStart,
                    model.SearchText,
                    model.selectedCategory,
                    model.location,
                    model.StateCode
                };
                var values = await _db.QueryAsync("usp_Get_NotRetailedSalesList",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> UpdateRetailSaledb(RetailSaleRequestv1 model, string username, string PlatformType)
        {
            try
            {
                var parameter = new
                {
                    SalesId = Guid.Parse(model.SalesId),
                    RetailedDate = model.RetailedDate,

                    Username = username,
                    PlatformType = PlatformType,

                    ProspectType = model.ProspectType,
                    ExchangeMake = model.ExchangeMake,
                    ExchangeHpCategory = model.ExchangeHpCategory,
                    ExchangeModel = model.ExchangeModel,
                    MfgYear = model.MfgYear,

                    CustomerAskExchange = model.CustomerAskExchange,
                    MktValueExchange = model.MktValueExchange,
                    FinalPriceExchange = model.FinalPriceExchange,

                    ExchangeStockEntry = model.ExchangeStockEntry,
                    ExchangeStockEntryValue = model.ExchangeStockEntryValue,

                    PaymentType = model.PaymentType,

                    DueAmount = model.DueAmount,

                    LoanRequired = model.LoanRequired,
                    FinancerName = model.FinancerName,
                    ManualFinancerName = model.ManualFinancerName,

                    LoanType = model.LoanType,

                    FinanceStatus = model.FinanceStatus,
                    FinanceStatusDetail = model.FinanceStatusDetail,

                    DisbursedAmount = model.DisbursedAmount,
                    AdditionalCash = model.AdditionalCash,

                    SalesEnquiryId = Guid.Parse(model.SalesEnquiryId),
                    FinanceMasterId = Guid.Parse(model.FinanceMasterId),
                    expectedRetailDate = model.expectedRetailDate
                };
                var values = await _db.ExecuteAsync("usp_ConvertToRetailSales",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> GenerateSalesEnquiryv2(EnquiryMainModel s, string dealerCode, string username, string mobileNumber, string platformtype, string f)
        {
            try
            {
                var param = new
                {
                    // ===== Common / Dealer =====
                    //DealerCode = dealerCode,
                    Username = username,
                    MobileNumber = mobileNumber,
                    PlatformType = platformtype,

                    s.stateHead,
                    s.nameStateHead,
                    s.areaManager,
                    s.nameAm,
                    s.territoryManager,
                    s.nameTm,

                    s.dealer,
                    s.dealerName,
                    s.dealerMail,
                    s.dealershipCode,
                    s.dealershipName,
                    s.dealershipLocation,

                    s.salesmenName,
                    s.salesmenNumber,

                    // ===== Enquiry =====
                    enquiryDate = s.Enquiry?.enquiryDate,
                    enquirySource = s.Enquiry?.enquirySource,
                    enquirySubSource = s.Enquiry?.enquirySubSource,

                    prospectName = s.Enquiry?.prospectName,
                    prospectMobile = s.Enquiry?.prospectMobile,
                    prospectDistrict = s.Enquiry?.prospectDistrict,
                    prospectDistrictCode = s.Enquiry?.prospectDistrictCode,
                    prospectTehsil = s.Enquiry?.prospectTehsil,
                    prospectTehsilCode = s.Enquiry?.prospectTehsilCode,
                    prospectVillage = s.Enquiry?.prospectVillage,
                    prospectVillageCode = s.Enquiry?.prospectVillageCode,
                    otherVillageName = s.Enquiry?.otherVillageName,
                    prospectPINCode = s.Enquiry?.prospectPINCode,
                    stateCode = s.Enquiry?.stateCode,

                    hpCategory = s.Enquiry?.hpCategory,
                    driveType = s.Enquiry?.driveType,
                    interestedModel = s.Enquiry?.interestedModel,
                    variant = s.Enquiry?.variant,

                    enquiryType = s.Enquiry?.enquiryType,
                    enquiryCurrentStatus = s.Enquiry?.enquiryCurrentStatus,
                    nextFollowupDate = s.Enquiry?.nextFollowupDate,

                    // ===== Sale =====
                    bookingDate = s.Enquiry?.bookingDate,
                    bookingAmount = s.Enquiry?.bookingAmount,
                    expDeliveryDate = s.Enquiry?.expDeliveryDate,

                    prospectType = s.Sale?.prospectType,
                    exchangeMake = s.Sale?.exchangeMake,
                    exchangeHpCategory = s.Sale?.exchangeHpCategory,
                    exchangeModel = s.Sale?.exchangeModel,
                    mfgYear = s.Sale?.mfgYear,
                    customerAskExchange = s.Sale?.customerAskExchange,
                    mktValueExchange = s.Sale?.mktValueExchange,
                    finalPriceExchange = s.Sale?.finalPriceExchange,
                    exchangeStockEntry = s.Sale?.exchangeStockEntry,
                    expectedRetailDate = s.Sale?.expectedRetailDate,

                    // ===== Finance =====
                    paymentType = s.Finance?.paymentType,
                    finalSalePrice = s.Finance?.finalSalePrice,
                    dpAmount = s.Finance?.dpAmount,
                    dueAmount = s.Finance?.dueAmount,

                    loanRequired = s.Finance?.loanRequired,
                    financerName = s.Finance?.financerName,
                    manualFinancerName = s.Finance?.manualFinancerName,
                    loanType = s.Finance?.loanType,

                    financeStatus = s.Finance?.financeStatus,
                    financeStatusDetail = s.Finance?.financeStatusDetail,
                    disbursedAmount = s.Finance?.disbursedAmount,
                    customerDues = s.Finance?.customerDues,

                    // ===== Close =====
                    enquiryStatus = s.Enquiry?.enquiryStatus,
                    deliveryDate = s.Sale?.deliveryDate,
                    chassisNumber = s.Sale?.chassisNumber,
                    closedDroppedStatus = s.Close?.closedDroppedStatus,
                    saleLostReason = s.Close?.saleLostReason,
                    exchangeStockEntryValue = s.Sale?.exchangeStockEntryValue,

                    // ===== Extra =====
                    s.conversionChallenge,
                    s.actionPlanned,
                    additionalPayment = s.Finance?.AdditionalCash,
                    excFile = f
                };

                var vals = await _db.ExecuteAsync(
                "GenerateSalesEnquiryV2",
                param: param,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> GetSalesEnq2db(string Loginas, string Username, SalesEnquirySearchdto model)
        {
            try
            {
                var param = new
                {
                    loginas = Loginas,
                    username = Username,
                    mobileNumber = model.ProspectMobileNumber
                };
                var vals = await _db.QueryAsync(
                "GetSalesEnquiryMasterSpv3",
                param: param,
                commandType: CommandType.StoredProcedure);
                return vals;

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> getRcrecords(Rc model, string loginas, string username)
        {
            try
            {
                var param = new
                {
                    model.RcStatus,
                    model.ChassisNo,
                    loginas,
                    username

                };
                var result = await _db.QueryAsync<dynamic>("usp_getRcStatusRecords", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> updRcrecords(Rc model, string loginas, string username)
        {
            try
            {
                var param = new
                {
                    RcStatus = model.RcStatus,
                    loginas = loginas,
                    username = username,
                    RegistrationNumber = model.RegistrationNumber,
                    Id = Guid.Parse(model.Id)
                };
                var result = await _db.ExecuteAsync("usp_updateRcStatus", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<int> updtGenerateSalesEnquiryv2(UpdateEnquiryMainModel s, string dealerCode, string username, string mobileNumber, string platformtype, string f)
        {
            try
            {
                var param = new
                {
                    // ===== Common / Dealer =====
                    //DealerCode = dealerCode,
                    Username = username,
                    MobileNumber = mobileNumber,
                    PlatformType = platformtype,
                    SalesEnquiryId = Guid.Parse(s.SalesEnquiryId),
                    FinanceMasterId = Guid.Parse(s.FinanceMasterId),
                    s.callstatus,
                    s.Remarks,
                    s.enquiryMainModel.stateHead,
                    s.enquiryMainModel.nameStateHead,
                    s.enquiryMainModel.areaManager,
                    s.enquiryMainModel.nameAm,
                    s.enquiryMainModel.territoryManager,
                    s.enquiryMainModel.nameTm,

                    s.enquiryMainModel.dealer,
                    s.enquiryMainModel.dealerName,
                    s.enquiryMainModel.dealerMail,
                    s.enquiryMainModel.dealershipCode,
                    s.enquiryMainModel.dealershipName,
                    s.enquiryMainModel.dealershipLocation,

                    s.enquiryMainModel.salesmenName,
                    s.enquiryMainModel.salesmenNumber,

                    // ===== Enquiry =====
                    enquiryDate = s.enquiryMainModel.Enquiry?.enquiryDate,
                    enquirySource = s.enquiryMainModel.Enquiry?.enquirySource,
                    enquirySubSource = s.enquiryMainModel.Enquiry?.enquirySubSource,

                    prospectName = s.enquiryMainModel.Enquiry?.prospectName,
                    prospectMobile = s.enquiryMainModel.Enquiry?.prospectMobile,
                    prospectDistrict = s.enquiryMainModel.Enquiry?.prospectDistrict,
                    prospectDistrictCode = s.enquiryMainModel.Enquiry?.prospectDistrictCode,
                    prospectTehsil = s.enquiryMainModel.Enquiry?.prospectTehsil,
                    prospectTehsilCode = s.enquiryMainModel.Enquiry?.prospectTehsilCode,
                    prospectVillage = s.enquiryMainModel.Enquiry?.prospectVillage,
                    prospectVillageCode = s.enquiryMainModel.Enquiry?.prospectVillageCode,
                    otherVillageName = s.enquiryMainModel.Enquiry?.otherVillageName,
                    prospectPINCode = s.enquiryMainModel.Enquiry?.prospectPINCode,
                    stateCode = s.enquiryMainModel.Enquiry?.stateCode,

                    hpCategory = s.enquiryMainModel.Enquiry?.hpCategory,
                    driveType = s.enquiryMainModel.Enquiry?.driveType,
                    interestedModel = s.enquiryMainModel.Enquiry?.interestedModel,
                    variant = s.enquiryMainModel.Enquiry?.variant,

                    enquiryType = s.enquiryMainModel.Enquiry?.enquiryType,
                    enquiryCurrentStatus = s.enquiryMainModel.Enquiry?.enquiryCurrentStatus,
                    nextFollowupDate = s.enquiryMainModel.Enquiry?.nextFollowupDate,

                    // ===== Sale =====
                    bookingDate = s.enquiryMainModel.Enquiry?.bookingDate,
                    bookingAmount = s.enquiryMainModel.Enquiry?.bookingAmount,
                    expDeliveryDate = s.enquiryMainModel.Enquiry?.expDeliveryDate,

                    prospectType = s.enquiryMainModel.Sale?.prospectType,
                    exchangeMake = s.enquiryMainModel.Sale?.exchangeMake,
                    exchangeHpCategory = s.enquiryMainModel.Sale?.exchangeHpCategory,
                    exchangeModel = s.enquiryMainModel.Sale?.exchangeModel,
                    mfgYear = s.enquiryMainModel.Sale?.mfgYear,
                    customerAskExchange = s.enquiryMainModel.Sale?.customerAskExchange,
                    mktValueExchange = s.enquiryMainModel.Sale?.mktValueExchange,
                    finalPriceExchange = s.enquiryMainModel.Sale?.finalPriceExchange,
                    exchangeStockEntry = s.enquiryMainModel.Sale?.exchangeStockEntry,
                    expectedRetailDate = s.enquiryMainModel.Sale?.expectedRetailDate,

                    // ===== Finance =====
                    paymentType = s.enquiryMainModel.Finance?.paymentType,
                    finalSalePrice = s.enquiryMainModel.Finance?.finalSalePrice,
                    dpAmount = s.enquiryMainModel.Finance?.dpAmount,
                    dueAmount = s.enquiryMainModel.Finance?.dueAmount,

                    loanRequired = s.enquiryMainModel.Finance?.loanRequired,
                    financerName = s.enquiryMainModel.Finance?.financerName,
                    manualFinancerName = s.enquiryMainModel.Finance?.manualFinancerName,
                    loanType = s.enquiryMainModel.Finance?.loanType,

                    financeStatus = s.enquiryMainModel.Finance?.financeStatus,
                    financeStatusDetail = s.enquiryMainModel.Finance?.financeStatusDetail,
                    disbursedAmount = s.enquiryMainModel.Finance?.disbursedAmount,
                    customerDues = s.enquiryMainModel.Finance?.customerDues,

                    // ===== Close =====
                    enquiryStatus = s.enquiryMainModel.Enquiry?.enquiryStatus,
                    deliveryDate = s.enquiryMainModel.Sale?.deliveryDate,
                    chassisNumber = s.enquiryMainModel.Sale?.chassisNumber,
                    closedDroppedStatus = s.enquiryMainModel.Close?.closedDroppedStatus,
                    saleLostReason = s.enquiryMainModel.Close?.saleLostReason,
                    exchangeStockEntryValue = s.enquiryMainModel.Sale?.exchangeStockEntryValue,

                    // ===== Extra =====
                    s.enquiryMainModel.conversionChallenge,
                    s.enquiryMainModel.actionPlanned,
                    AdditionalCash = s.enquiryMainModel.Finance?.AdditionalCash,
                    excFile = f
                };

                var vals = await _db.ExecuteAsync(
                "usp_updSalesEnquiry",
                param: param,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<dynamic> GetSalesEnqbyId2db(string Loginas, string Username, salesEnquiryIdDto model)
        {
            try
            {
                var param = new
                {
                    loginas = Loginas,
                    username = Username,
                    SalesEnquiryId = Guid.Parse(model.Id)
                };
                var vals = await _db.QueryFirstOrDefaultAsync(
                "GetSalesEnquiryByIDv3",
                param: param,
                commandType: CommandType.StoredProcedure);
                return vals;

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetChassisNumber(string DealerCode)
        {
            try
            {
                var param = new { DealerCode };
                var result = await _db.QueryAsync("usp_GetChassisNumberOnDealer", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<dynamic> GetPopUpCountDb(string loginas, string username)
        {
            try
            {
                var param = new { loginas = loginas, username = username };
                var result = await _db.QueryFirstAsync("usp_GetPopUpCount", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<dynamic> ReportLastDateHeading()
        {
            try
            {
                var result = await _db.QueryFirstAsync("usp_getRepoFilledLastDateHeadingCount", commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> AddtionalPaymentRecdb(salesEnquiryIdDto id)
        {
            try
            {
                var param = new { salesId = Guid.Parse(id.Id) };
                var result = await _db.QueryAsync("usp_GetAdditiPaymentReco", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> getExchangeStockdb(getExchangeStock model, string loginas, string username)
        {
            try
            {
                var param = new
                {
                    model.Status,
                    model.RowStart,
                    model.PageSize,
                    model.AmMail,
                    model.ShMail,
                    model.TmMail,
                    model.DealerMail,
                    loginas,
                    username,
                    model.SearchText,
                    model.location,
                    model.StateCode
                };
                var result = await _db.QueryAsync("usp_getExchangeStock", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> SoldExchdb(SoldExchStock model, string platfmtype, string username)
        {
            try
            {
                var param = new
                {
                    model.Tehsil,
                    model.Village,
                    model.SellingPrice,
                    model.LiquidationDate,
                    model.District,
                    model.CustomerMobile,
                    model.CustomerName,
                    SalesId = Guid.Parse(model.SalesId),
                    platfmtype,
                    username
                };
                var result = await _db.ExecuteAsync("usp_ExchStockSoldMark", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetOldEnquirydb(getExchangeStock model, string loginas, string username)
        {
            try
            {
                var param = new
                {
                    //model.Status,
                    model.RowStart,
                    model.PageSize,
                    model.AmMail,
                    model.ShMail,
                    model.TmMail,
                    model.DealerMail,
                    loginas,
                    username,
                    model.SearchText,
                    model.selectedCategory,
                    model.location,
                    model.StateCode
                };
                var result = await _db.QueryAsync("usp_getOldEnquiry", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> UpdateOldEnquirydb(oldEnquiryUpdate model, string platfmtype, string username)
        {
            try
            {
                var param = new
                {
                    model.FinalSellingPrice,
                    model.DpAmount,
                    SalesId = Guid.Parse(model.SalesId),
                    platfmtype,
                    username
                };
                var result = await _db.ExecuteAsync("usp_UpdatOldEnquiry", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<string> InsertNDAFormdb(NDAFormModel model)
        {
            try
            {
                var param = new
                {
                    Name = _user.GetName(),
                    UserName = _user.GetUserName(),
                    platformtype = _user.GetPlatform(),
                    model.TmName,
                    model.AmName,
                    model.SHName,
                    model.EnquirySource,
                    model.EnquirySubSource,
                    model.NDAProspectName,
                    model.MobileNo,
                    model.EnquiryCurrentStatus,
                    model.DistrictName,
                    model.TehsilName,
                    model.CityName,
                    model.CurrentBussiness,
                    model.IndustrySize,
                    model.InvestPlan,
                    model.ActionPlan,
                    model.NextFollowDate,
                    model.FollowupRemarks,
                    model.CloseEnquiryRemark,
                    model.DistrictCode,
                    model.expectedConversionDate,
                    model.Remarks

                };
                var result = await _db.ExecuteScalarAsync<string>("usp_AddNDAEnquirytblv1", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetDistrict(GetDistrict model)
        {
            try
            {
                var param = new
                {
                    username = model.userName ?? _user.GetUserName(),
                    loginas = model.Position ?? _user.GetPositionName()

                };
                var result = await _db.QueryAsync("usp_GetDistrictsByUser", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetTehsil(List<GetTehsil> model)
        {
            try
            {
                var list = new List<dynamic>();

                foreach (var code in model)
                {
                    var param = new
                    {
                        DistrictCode = code.DistrictCode,
                    };

                    var result = await _db.QueryAsync<dynamic>(
                        "usp_GetTehsil",
                        param: param,
                        commandType: CommandType.StoredProcedure
                    );

                    list.AddRange(result);
                }

                return list;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetCity(List<GetCity> model)
        {
            try
            {
                var list = new List<dynamic>();

                foreach (var code in model)
                {
                    var param = new
                    {
                        TehsilCode = code.TehsilCode,
                    };

                    var result = await _db.QueryAsync<dynamic>(
                        "usp_GetCity",
                        param: param,
                        commandType: CommandType.StoredProcedure
                    );

                    list.AddRange(result);
                }

                return list;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetNdadb(RequestGetNDA m)
        {
            try
            {
                var param = new
                {
                    username = _user.GetUserName(),
                    loginas = _user.GetPositionName(),
                    downlaodStatus = m.Status,
                    pageNo = m.PageNo,
                    m.StateCode,
                    m.Duration,
                    m.StDate,
                    m.EnDate,
                    m.selectedEnquirySource,
                    m.selectedEnquirySubSource,
                    m.SelectedMobileNumber,
                    m.selectedOverdueFilter


                };
                var result = await _db.QueryAsync("usp_getNDAtbl", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> updtNDAdb(NDAEnquiryModel model)
        {
            try
            {
                var param = new
                {
                    createby = _user.GetUserName(),
                    platformtype = _user.GetPlatform(),
                    model.NextFollowUpDate,

                    model.SHInterviewStatus,
                    model.SHInterviewDate,
                    model.SHRemarks,
                    model.SHRejectionRemark,

                    model.HOInterviewStatus,
                    model.HOInterviewDate,
                    model.HORemarks,
                    model.HORejectionRemark,

                    model.SDReceivedStatus,
                    model.SDReceivingDate,

                    model.GSTStatus,
                    model.GSTRegistrationDate,

                    model.FundsReceivedStatus,
                    model.FundTransferDate,

                    model.BGReceivedStatus,
                    model.BGSubmissionDate,

                    model.CodeOpenedStatus,
                    model.LOIDate,

                    Id = Guid.Parse(model.Id),
                    model.FollowupRemarks,
                    model.CloseEnquiryRemark,
                    model.EnquiryCurrentStatus,
                    model.ActionPlan,
                    model.expectedConversionDate,
                    model.Remarks


                };
                var result = await _db.ExecuteAsync("usp_updtNDAtbl", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetNDAByIddb(GetNDAByIDModel model)
        {
            try
            {
                var param = new
                {
                    createby = _user.GetUserName(),
                    loginas = _user.GetPositionName(),
                    Id = Guid.Parse(model.Id)


                };
                var result = await _db.QueryAsync("usp_getNDAbyId", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<PositionFilterResult> GetStateHeadByStatedb(StateCodeModel model)
        {
            try
            {
                var param = new
                {
                    username = _user.GetUserName(),
                    loginas = _user.GetPositionName(),
                    model.StateName


                };
                using var multi = await _db.QueryMultipleAsync(
                    "usp_GetstateHeadByState",
                    param: param,
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
                };
            }
            catch
            {
                throw new Exception("Database Error");
            }
        }

        public async Task<IEnumerable<dynamic>> GetOldRetailedEnquirydb(getExchangeStock model, string loginas, string username)
        {
            try
            {
                var param = new
                {
                    //model.Status,
                    model.RowStart,
                    model.PageSize,
                    model.AmMail,
                    model.ShMail,
                    model.TmMail,
                    model.DealerMail,
                    loginas,
                    username,
                    //model.SearchText
                };
                var result = await _db.QueryAsync("usp_getOldRetailedEnq", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> DownloadNotRetailedList(RetailedSalesFilter model, string LoginPosition, string username)
        {
            try
            {
                var parameter = new
                {
                    LoginPosition = LoginPosition,
                    ShMail = model.ShMail,
                    AmMail = model.AmMail,
                    TmMail = model.TmMail,
                    DealerMail = model.DealerMail,
                    LoginMail = username,
                    model.SearchText,
                    model.selectedCategory,
                    model.location,
                    model.StateCode

                };
                var values = await _db.QueryAsync("usp_DownloadNotRetailedSalesList",
                    param: parameter, commandType: CommandType.StoredProcedure);
                return values;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> downloadExchangeStockdb(getExchangeStock model, string loginas, string username)
        {
            try
            {
                var param = new
                {

                    model.AmMail,
                    model.ShMail,
                    model.TmMail,
                    model.DealerMail,
                    loginas,
                    username,
                    model.SearchText,
                    model.Status,
                    model.location,
                    model.StateCode

                };
                var result = await _db.QueryAsync("usp_DownloadExchangeStock", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetNDAHistoryById(GetNDAByIDModel model)
        {
            try
            {
                var param = new
                {
                    createby = _user.GetUserName(),
                    loginas = _user.GetPositionName(),
                    Id = Guid.Parse(model.Id)


                };
                var result = await _db.QueryAsync("usp_getNdaHistory", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> ApprovalSalesmanKyc(ApprovalSalesmanKyc model)
        {
            try
            {
                var param = new
                {
                    username = _user.GetUserName(),
                    loginas = _user.GetPositionName(),
                    loginname = _user.GetName(),
                    Id = Guid.Parse(model.Id),
                    model.Remark,
                    model.Dealercode,
                    model.Status,
                    model.BatchId,


                };
                var result = await _db.ExecuteAsync("usp_ApproveSalesmanKyc", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetKycPendingListdb(SalesmanGetdto model)
        {
            try
            {
                var param = new
                {
                    LoginMail = _user.GetUserName(),
                    LoginPosition = _user.GetPositionName(),
                    model.ShMail,
                    model.AmMail,
                    model.TmMail,
                    model.DealerMail,
                    model.Status,
                    model.Download

                };
                var result = await _db.QueryAsync("usp_GetKycPendingList", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> getLocationListdb(PositionFilter model)
        {
            try
            {
                var param = new
                {
                    username = _user.GetUserName(),
                    loginas = _user.GetPositionName(),
                    model.ShMail,
                    model.AmMail,
                    model.TmMail,
                    //model.DealerMail

                };
                var result = await _db.QueryAsync("usp_getLocationList", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> updatedealerdb(DealerUpdate model)
        {
            try
            {
                var param = new
                {
                    //LoginMail = _user.GetUserName(),
                    //LoginPosition = _user.GetPositionName(),
                    model.DealerCode,
                    model.DealerStatus,

                };
                var result = await _db.ExecuteAsync("usp_updtdealer", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetChassisForReturnBillingdb(Pagignation model)
        {
            try
            {
                var param = new
                {
                    username = _user.GetUserName(),
                    loginas = _user.GetPositionName(),
                    Skip = model.Skip,
                    model.GlobalFilter

                };
                var result = await _db.QueryAsync("usp_getStockForReturnBilling", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<int> ReturnBillingdb(ReturnBillingModel model)
        {
            try
            {
                var param = new
                {
                    //LoginMail = _user.GetUserName(),
                    //LoginPosition = _user.GetPositionName(),
                    id = Guid.Parse(model.Id),
                    model.DealerCode,
                    model.ChassisNo,
                    model.ModelCode,
                    billdate = model.BillDate,
                    returndate = Convert.ToDateTime(model.ReturnDate),
                    username = _user.GetUserName(),
                    loginas = _user.GetPositionName(),


                };
                var result = await _db.ExecuteAsync("usp_returnBilling", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> GetExchangeModelListdb()
        {
            try
            {
                //var param = new
                //{
                //    username = _user.GetUserName(),
                //    loginas = _user.GetPositionName()

                //};
                var result = await _db.QueryAsync("usp_getExchangeModelMaster", commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<sourceSubSourceMaster>> getSourceSubSourcedb()
        {
            try
            {
                var param = new { loginas = _user.GetPositionName() };
                var result = await _db.QueryAsync<sourceSubSourceMaster>("usp_getSourceSubSource", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<sourceSubSourceMaster>> getSourceSubSourceForRepFilter()
        {
            try
            {
                var param = new { loginas = _user.GetPositionName() };
                var result = await _db.QueryAsync<sourceSubSourceMaster>("usp_getSourceSubsourceForRepFilter", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> getbillingReqData(BillingReqModel model)
        {
            try
            {
                var param = new
                {
                    loginas = _user.GetPositionName(),
                    username = _user.GetUserName(),
                    DealerCode = model.dealerCode
                };
                var result = await _db.QueryAsync("usp_getBillingReqData", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<dynamic> getUnassignedCountdb()
        {
            try
            {
                var param = new
                {
                    loginas = _user.GetPositionName(),
                    username = _user.GetUserName()

                };
                var result = await _db.QueryFirstOrDefaultAsync("get_unassignedCount", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> getThreeDaysOdEnquiriesdb(SuperHotEnquiry model)
        {
            try
            {
                var parameters = new
                {
                    mobile = _user.GetMobile(),
                    username = _user.GetUserName(),
                    loginposition = _user.GetPositionName(),
                    model.PageSize,
                    model.RowStart,
                    model.Download
                };
                var vals = await _db.QueryAsync(
                //"usp_getSevenDaySalesEnquiryDelivery",
                "usp_getThereeDaysOdEnq",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> getPendingConversionByListdb(RequestGetNDA m)
        {
            try
            {
                var parameters = new
                {
                    username = _user.GetUserName(),
                    loginas = _user.GetPositionName(),
                    pageNo = m.PageNo,
                    m.Duration,
                    m.StDate,
                    m.EnDate,
                    m.StateCode,
                    m.selectedEnquirySource,
                    m.selectedEnquirySubSource,
                    m.SelectedMobileNumber
                };

                var vals = await _db.QueryAsync(
                "usp_PendingConversionByList",
                param: parameters,
                commandType: CommandType.StoredProcedure);


                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> getIndustryByTalukadb(int talukaCode)
        {
            try
            {
                var parameters = new
                {
                    username = _user.GetUserName(),
                    loginas = _user.GetPositionName(),
                    talukaCode
                };

                var vals = await _db.QueryFirstOrDefaultAsync<int>(
                "usp_getIndustryByTaluka",
                param: parameters,
                commandType: CommandType.StoredProcedure);

                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<whatsappUserDetail> getStateHeadByStateName(string StateName)
        {
            try
            {
                var parameters = new
                {
                    username = _user.GetUserName(),
                    loginas = _user.GetPositionName(),
                    StateName
                };

                return await _db.QueryFirstOrDefaultAsync<whatsappUserDetail>(
                "usp_getStateHeadBtStateName",
                param: parameters,
                commandType: CommandType.StoredProcedure);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> updateNDAWhatsappCount(string mid, string message, string mobile, string status, string messageType)
        {
            try
            {
                var param = new
                {
                    mid,
                    message,
                    mobile,
                    status,
                    messageType

                };
                var result = await _db.ExecuteAsync("usp_WhatsappNDAMessageHistory", param: param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> getMenuListdb()
        {
            try
            {
                var param = new { userName = _user.GetUserName() };
                var vals = await _db.QueryAsync(
                "usp_getMenuList",
                param: param,
                commandType: CommandType.StoredProcedure);


                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<ResponseEmployeeMaster>> getEmployeeMasterList(RequestEmployeeMaster m)
        {
            try
            {
                var parameters = new
                {
                    m.stateCode,
                    m.position
                };

                var vals = await _db.QueryAsync<ResponseEmployeeMaster>(
                "getAllEmployee",
                param: parameters,
                commandType: CommandType.StoredProcedure);

                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> updtConversionBydb(reqForUpdateConversion m)
        {
            try
            {

                var result = await _db.ExecuteAsync("usp_updateNdaConversionBy", param: m, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<dynamic>> DealerDetailByDealerCodedb(string dealerCode)
        {
            try
            {
                var parameters = new
                {
                    dealerCode
                };

                var vals = await _db.QueryAsync(
                "usp_getDealerDetailByDealerCode",
                param: parameters,
                commandType: CommandType.StoredProcedure);

                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<int> updateDealerAssignmentsdb(UpdateDealerFormRequest m)
        {
            try
            {

                var result = await _db.ExecuteScalarAsync<int>("usp_updateDealerMapping",
                    param: m, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }



        public async Task<IEnumerable<ResponseEmployeeByPosition>> getEmployeeMasterListByPositionDb(string position)
        {
            try
            {
                var parameters = new
                {
                    position
                };

                var vals = await _db.QueryAsync<ResponseEmployeeByPosition>(
                "usp_getEmployeeByPosition",
                param: parameters,
                commandType: CommandType.StoredProcedure);

                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
