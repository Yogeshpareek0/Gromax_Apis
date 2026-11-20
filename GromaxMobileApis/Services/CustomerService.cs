using Azure.Storage.Blobs.Models;
using Dapper;
using GromaxMobileApis.Interfaces;
using GromaxMobileApis.Models;
using GromaxMobileApis.Models.DealerMaster;
using GromaxMobileApis.Models.Services;
using GromaxMobileApis.Models.VerifyWebhook;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using YamlDotNet.Core;
using static GromaxMobileApis.Models.Services.FieldTechnicalReportModel;
using static GromaxMobileApis.Models.Services.SparePartsModel;
using static GromaxMobileApis.Models.Services.WarrantyClaimModel;
using static GromaxMobileApis.Utilities.customerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GromaxMobileApis.Services
{
    public class CustomerService : ICustomerService
    {
        private IDbConnection _db;
        private string _conString;
        private IUser _user;
        public CustomerService(IDbConnection db, IConfiguration configuration, IUser user)
        {
            _db = db;
            _conString = configuration.GetConnectionString("DefaultConnection");
            _user = user;
        }


        public async Task<int> addJobCard(AddJobCardRequest m)
        {
            try
            {
                var param = new
                {
                    m.SalesMasterId,
                    m.ChassiNumber,
                    m.JobCardDate,
                    m.JobCardType,
                    m.Fuel,
                    DealerCode = _user.GetDealerCode(),
                    m.DealerName,
                    m.CustomerName,
                    m.CustomerAddress,
                    m.TractorSlNo,
                    m.RegnNo,
                    m.DateOfSale,
                    m.WorkDoneBy,
                    m.MobileNo,
                    m.AlternateMobileNo,
                    m.FrontTyrePressureLeft,
                    m.FrontTyrePressureRight,
                    m.RearTyrePressureLeft,
                    m.RearTyrePressureRight,
                    m.Hours,
                    m.TimeEstimate,
                    m.TimeActual,
                    m.CostEstimate,
                    m.CostActual,
                    m.SpareTotal,
                    m.LocalTotal,
                    m.SubletTotal,
                    m.GrandTotal,
                    m.TotalAmountPaid,
                    m.SubletDescription,
                    m.LabourTotal,
                    CreatedBy = _user.GetUserName(),
                    PlatformType = _user.GetPlatform(),

                    Complaints = m.Tables.Complaints.AsTableValuedParameter("JobCardComplaintType"),
                    MissingParts = m.Tables.MissingParts.AsTableValuedParameter("JobCardMissingPartType"),
                    SpareParts = m.Tables.SpareParts.AsTableValuedParameter("JobCardSparePartType"),
                    LocalParts = m.Tables.LocalParts.AsTableValuedParameter("JobCardLocalPartType")
                };
                var vals = await _db.ExecuteScalarAsync<int>(
                "usp_AddJobCard",
                param: param,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<int> addOnlineEnqdb(VerifyWebhook model)
        {
            try
            {
                var param = new
                {
                    Name = model.Name,
                    Mobile = model.Mobile,
                    City = model.City,
                    State = model.StateProvince,
                    Model = model.Model,
                    Hp_Category = model.HpCategory,
                    TermsOfcondition = model.TermsOfService
                };
                var vals = await _db.ExecuteAsync(
                "usp_addOnlineEnquiries",
                param: param,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<int> addPDIdb(PdiRequest m, DataTable dt)
        {
            try
            {
                var param = new { m.stockMasterId, m.runningHrs, m.other, createdBy = _user.GetUserName(), items = dt };
                var vals = await _db.ExecuteAsync(
                "usp_insertPdiInspection",
                param: param,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<dynamic>> getEligibleChassisList(EligibleChassisListRequest r)
        {
            try
            {
                var parameters = new
                {
                    username = _user.GetUserName(),
                    loginas = _user.GetPositionName(),
                    r.page,
                    r.searchQuery,
                    r.searchBy
                };
                var vals = await _db.QueryAsync(
                "usp_GetJobCardEligibleTractors",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<dynamic>> getPDIPendingList(PDIListResponse r)
        {
            try
            {
                var parameters = new { username = _user.GetUserName(), loginPosition = _user.GetPositionName(), r.page, r.searchQuery };
                var vals = await _db.QueryAsync(
                "usp_getPDIPendingList",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<dynamic>> GetServiceTimelinedb(Guid salesMasterId)
        {
            try
            {
                var param = new { salesMasterId };
                var vals = await _db.QueryAsync(
                "usp_GetServiceTimeline",
                param,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<List<SparesPartMaster>> getSparePartsdb()
        {
            try
            {
                var vals = await _db.QueryAsync<SparesPartMaster>(
                "usp_getSpareParts",
                commandType: CommandType.StoredProcedure);
                return vals.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<List<WorkNatureMasterResponse>> getWorkNatureListAsync()
        {
            try
            {
                var vals = await _db.QueryAsync<WorkNatureMasterResponse>(
                "usp_getWorkOfNatureMaster",
                commandType: CommandType.StoredProcedure);
                return vals.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<JObCardReportResponse> jobCardReportdb(JobCardMasterReportRequest m)
        {
            try
            {
                var param = new
                {
                    loginPosition = _user.GetPositionName(),
                    m.DealerMail,
                    loginMail = _user.GetUserName(),
                    m.StateName,
                    m.Duration,
                    m.StDate,
                    m.EnDate,
                    m.JobType
                };
                using var multi = await _db.QueryMultipleAsync(
                "usp_jobCardMasterReport",
                param,
                commandType: CommandType.StoredProcedure
                );

                var report = await multi.ReadAsync<dynamic>();

                var count = await multi.ReadAsync<dynamic>();

                return new JObCardReportResponse
                {
                    report = report,
                    count = count
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }



        public async Task<ResponseJobCardMaster> getJobCardByIddb(string jobCardMasterId)
        {
            try
            {
                var param = new { id = Guid.Parse(jobCardMasterId) };
                using var multi = await _db.QueryMultipleAsync(
                  "usp_getJobCardById",
                  param,
                  commandType: CommandType.StoredProcedure);

                var response = new ResponseJobCardMaster
                {
                    resJobCardMaster = (await multi.ReadAsync<JobCardMaster>()).ToList(),
                    resJobCardComplaint = (await multi.ReadAsync<JobCardComplaint>()).ToList(),
                    resJobCardMissingPart = (await multi.ReadAsync<JobCardMissingPart>()).ToList(),
                    resJobCardSparePart = (await multi.ReadAsync<JobCardSparePart>()).ToList(),
                    resJobCardLocalPart = (await multi.ReadAsync<JobCardLocalPart>()).ToList()
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<int> updateJobCard(UpdateJobCardRequest m)
        {
            try
            {
                var param = new
                {
                    m.JobCardMasterId,
                    m.SalesMasterId,
                    m.ChassiNumber,
                    m.JobCardDate,
                    m.JobCardType,
                    m.Fuel,
                    DealerCode = _user.GetDealerCode(),
                    m.DealerName,
                    m.CustomerName,
                    m.CustomerAddress,
                    m.TractorSlNo,
                    m.RegnNo,
                    m.DateOfSale,
                    m.WorkDoneBy,
                    m.MobileNo,
                    m.AlternateMobileNo,
                    m.FrontTyrePressureLeft,
                    m.FrontTyrePressureRight,
                    m.RearTyrePressureLeft,
                    m.RearTyrePressureRight,
                    m.Hours,
                    m.TimeEstimate,
                    m.TimeActual,
                    m.CostEstimate,
                    m.CostActual,
                    m.SpareTotal,
                    m.LocalTotal,
                    m.SubletTotal,
                    m.GrandTotal,
                    m.TotalAmountPaid,
                    m.SubletDescription,
                    m.LabourTotal,
                    CreatedBy = _user.GetUserName(),
                    PlatformType = _user.GetPlatform(),

                    Complaints = m.Tables.Complaints.AsTableValuedParameter("JobCardComplaintTypeUpd"),
                    MissingParts = m.Tables.MissingParts.AsTableValuedParameter("JobCardMissingPartTypeUpd"),
                    SpareParts = m.Tables.SpareParts.AsTableValuedParameter("JobCardSparePartTypeUpd"),
                    LocalParts = m.Tables.LocalParts.AsTableValuedParameter("JobCardLocalPartTypeUpd")
                };
                var vals = await _db.ExecuteScalarAsync<int>(
                "usp_UpdateJobCard",
                param: param,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<dynamic>> getNTIRPendingList(NTIRListResponse r)
        {
            try
            {
                var parameters = new { username = _user.GetUserName(), loginPosition = _user.GetPositionName(), r.page, r.searchQuery };
                var vals = await _db.QueryAsync(
                "usp_getNtirPendingList",
                param: parameters,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<int> addNTIRdb(NTIRRequest m, DataTable dt)
        {
            try
            {
                var param = new { m.stockMasterId, m.runningHrs, m.other, createdBy = _user.GetUserName(), items = dt };
                var vals = await _db.ExecuteScalarAsync<int>(
                "usp_insertNtirInspection",
                param: param,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<NTIRMasterReponse> getNTIRByIDdb(Guid Id)
        {
            try
            {
                var parameters = new { NTIRMasterId = Id };
                using var vals = await _db.QueryMultipleAsync(
                "usp_getNTIRById",
                param: parameters,
                commandType: CommandType.StoredProcedure);

                var response = new NTIRMasterReponse
                {
                    NtirMaster = (await vals.ReadAsync<NTIRResponse>()).FirstOrDefault(),
                    NtirFieldsMaster = (await vals.ReadAsync<NTIRFieldResponse>()).ToList(),

                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<NTIRMasterReponse> getPDIByIDdb(Guid Id)
        {
            try
            {
                var parameters = new { PDIMasterId = Id };
                using var vals = await _db.QueryMultipleAsync(
                "usp_getPDIById",
                param: parameters,
                commandType: CommandType.StoredProcedure);

                var response = new NTIRMasterReponse
                {
                    NtirMaster = (await vals.ReadAsync<NTIRResponse>()).FirstOrDefault(),
                    NtirFieldsMaster = (await vals.ReadAsync<NTIRFieldResponse>()).ToList(),

                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }
        public async Task<int> updateNTIRdb(NTIRRequest m, DataTable dt)
        {
            try
            {
                var param = new { ntirMasterId = m.id, m.runningHrs, m.other, createdBy = _user.GetUserName(), items = dt, m.stockMasterId };
                var vals = await _db.ExecuteScalarAsync<int>(
                "usp_updateNtirInspection",
                param: param,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<int> addPDIdbv1(NTIRRequest m, DataTable dt)
        {
            try
            {
                var param = new { m.stockMasterId, m.runningHrs, m.other, createdBy = _user.GetUserName(), items = dt, doneBy = m.doneBy, m.engineNo };
                var vals = await _db.ExecuteScalarAsync<int>(
                "usp_insertPdiInspectionv1",
                param: param,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<int> updatePDIdb(NTIRRequest m, DataTable dt)
        {
            try
            {
                var param = new { pdiMasterId = m.id, m.runningHrs, m.other, createdBy = _user.GetUserName(), items = dt, m.stockMasterId, doneBy = m.doneBy, m.engineNo };
                var vals = await _db.ExecuteScalarAsync<int>(
                "usp_updatePdiInspection",
                param: param,
                commandType: CommandType.StoredProcedure);
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }


        public async Task<int> AddMetaLead(MetaLeadRequest input)
        {
            try
            {


                var parameters = new DynamicParameters();

                parameters.Add("@LeadId", input.LeadId);
                parameters.Add("@LeadCreatedTime", input.LeadCreatedTime);
                parameters.Add("@FullName", input.FullName);
                parameters.Add("@PhoneNumber", input.PhoneNumber);
                parameters.Add("@City", input.City);
                parameters.Add("@State", input.State);
                parameters.Add("@CampaignName", input.CampaignName);
                parameters.Add(
                    "@Location",
                    input.Location);
                parameters.Add("@CreateBy", "Meta");

                var result = await _db.QueryFirstOrDefaultAsync<int>(
                    "usp_InsertMetaLead",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result;
            }
            catch
            {
                throw;
            }
        }


        public async Task<int> addDealerMasterdb(DealerRequestModel input)
        {
            try
            {
                string tehsilsCsv = input.tehsils != null && input.tehsils.Any()
                    ? string.Join(",", input.tehsils)
                    : null;

                string tehsilCodesCsv = input.tehsilCodes != null && input.tehsilCodes.Any()
                    ? string.Join(",", input.tehsilCodes)
                    : null;

                var parameters = new DynamicParameters();
                parameters.Add("@dealerCode", input.dealerCode);
                parameters.Add("@dealerName", input.dealerName);
                parameters.Add("@gstNo", input.gstNo);
                parameters.Add("@panNo", input.panNo);
                parameters.Add("@dealerMobile", input.dealerMobile);
                parameters.Add("@dealerAlternateMobile", input.dealerAlternateMobile);
                parameters.Add("@dealerEmail", input.dealerEmail);
                parameters.Add("@address", input.address);

                parameters.Add("@stateCode", input.stateCode);
                parameters.Add("@stateName", input.stateName);
                parameters.Add("@district", input.district);
                parameters.Add("@city", input.city);
                parameters.Add("@tehsils", tehsilsCsv);

                parameters.Add("@stateHeadMail", input.stateHeadMail);
                parameters.Add("@stateHeadName", input.stateHeadName);
                parameters.Add("@stateHeadMobile", input.stateHeadMobile);

                parameters.Add("@amMail", input.amMail);
                parameters.Add("@amName", input.amName);
                parameters.Add("@amMobile", input.amMobile);

                parameters.Add("@tmMail", input.tmMail);
                parameters.Add("@tmName", input.tmName);
                parameters.Add("@tmMobile", input.tmMobile);

                parameters.Add("@serviceCcmName", input.serviceCcmName);
                parameters.Add("@serviceCcmEmail", input.serviceCcmEmail);
                parameters.Add("@serviceCcmMobile", input.serviceCcmMobile);

                parameters.Add("@dateOfAppointment", input.dateOfAppointment);
                parameters.Add("@activeStatus", input.activeStatus);

                parameters.Add("@DistrictCode", input.districtCode);
                parameters.Add("@TehsilCodes", tehsilCodesCsv);

                var result = await _db.QueryFirstOrDefaultAsync<int>(
                    "usp_AddDealerMaster",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<dynamic>> getOpenJobCarddb(int pageNo)
        {
            try
            {
                var param = new { pageNo, loginas = _user.GetPositionName(), username = _user.GetUserName() };
                var res = await _db.QueryAsync(
                  "usp_GetOpenJobCard",
                  param,
                  commandType: CommandType.StoredProcedure);

                return res;

            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<ServiceInvoiceResponse> getServiceInvByIddb(Guid Id, DataTable Ids)
        {
            try
            {
                var param = new { Id = Id, loginas = _user.GetPositionName(), username = _user.GetUserName(), Ids = Ids };
                using var multi = await _db.QueryMultipleAsync(
                  "get_ServiceInvoiceData",
                  param,
                  commandType: CommandType.StoredProcedure);

                var response = new ServiceInvoiceResponse
                {
                    Master = (await multi.ReadAsync<ServiceInvoiceModel>()).FirstOrDefault(),
                    Items = (await multi.ReadAsync<ServiceInvoiceItemModel>()).ToList(),
                    Hours = (await multi.ReadAsync<ChassisServiceHoursModel>()).ToList()
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }


        public async Task<string> generateServiceInvoicedb(DataTable dt)
        {
            try
            {
                var param = new
                {
                    Id = dt,
                    loginas = _user.GetPositionName(),
                    username = _user.GetUserName(),
                    serviceType = "Free Service Coupon",
                    platFormType = _user.GetPlatform()
                };
                var res = await _db.ExecuteScalarAsync<string>(
                  "usp_CreateServiceInvoice",
                  param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<dynamic>> getFreeServiceClosedJobCarddb()
        {
            try
            {
                var param = new
                {

                    loginas = _user.GetPositionName(),
                    username = _user.GetUserName(),
                };
                var res = await _db.QueryAsync(
                  "usp_getFreeServiceClosedJob",
                  param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<int> updateServiceInvUrldb(Guid Id, string url)
        {
            try
            {
                var param = new
                {
                    Id,
                    url
                };
                var res = await _db.ExecuteScalarAsync<int>(
                  "usp_updateServiceInvUrl",
                  param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<dynamic>> GetPendingServiceInvoiceInstallationdb()
        {
            try
            {
                var param = new
                {

                    loginas = _user.GetPositionName(),
                    username = _user.GetUserName(),
                };
                var res = await _db.QueryAsync(
                  "usp_GetPendingServiceInvoiceInstallation",
                  param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }



        public async Task<string> generateInstallationInvoicedb(DataTable dt)
        {
            try
            {
                var param = new
                {
                    Id = dt,
                    loginas = _user.GetPositionName(),
                    username = _user.GetUserName(),
                    serviceType = "Installation Service",
                    platFormType = _user.GetPlatform()
                };
                var res = await _db.ExecuteScalarAsync<string>(
                  "usp_CreateInstallReimbInvoice",
                  param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<dynamic>> getInvoiceListdb(ServiceInvoiceListResp m)
        {
            try
            {
                var param = new
                {

                    loginas = _user.GetPositionName(),
                    username = _user.GetUserName(),
                    m.stDate,
                    m.enDate,
                    m.duration
                };
                var res = await _db.QueryAsync(
                  "usp_getServiceInvoices",
                  param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }



        public async Task<int> UpdateReimbursementPaymentStatusDb(UpdateReimbursementStatusRequest m)
        {
            try
            {
                var param = new
                {

                    loginas = _user.GetPositionName(),
                    username = _user.GetUserName(),
                    Id = m.InvoiceId,
                };
                var res = await _db.ExecuteScalarAsync<int>(
                  "usp_UpdateReimbursementPaymentStatus",
                  param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }


        public async Task<IEnumerable<ReimbursementScoreMasterModel>> getReimbursementListdb(string dealerCode)
        {
            try
            {
                var param = new { dealerCode };
                var res = await _db.QueryAsync<ReimbursementScoreMasterModel>(
                  "usp_ReimbursementScoreMaster_GetList",
                  param: param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<int> InsertMechanicDb(MechanicCreateRequest m)
        {
            try
            {
                m.DealerCode = string.Equals(_user.GetPositionName(), "Dealer", StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrWhiteSpace(_user.GetDealerCode())
                    ? _user.GetDealerCode() : m.DealerCode;
                m.CreatedBy = _user.GetUserName();
                var res = await _db.ExecuteScalarAsync<int>(
                    "USP_InsertMechanic",
                    m,
                    commandType: CommandType.StoredProcedure);

                return res;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<MechanicMasterDataModel>> getMechanicListdb(MechanicApprovalFilterRequest m)
        {
            try
            {
                var Param = new { username = _user.GetUserName(), loginAs = _user.GetPositionName(), m.Status, m.StDate, m.EnDate };
                var res = await _db.QueryAsync<MechanicMasterDataModel>(
                  "usp_getMechanicMasterList",
                  param: Param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<int> MechanicApprovalDb(MechanicApprovalRequest m)
        {
            try
            {
                m.UserName = _user.GetUserName();
                m.LoginAs = _user.GetPositionName();

                var res = await _db.ExecuteScalarAsync<int>(
                "USP_MechanicApproval",
                m,
                commandType: CommandType.StoredProcedure);

                return res;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<MechanicMaster>> getMechanicsPendingListDb()
        {
            try
            {
                var Param = new { username = _user.GetUserName(), loginAs = _user.GetPositionName() };
                var res = await _db.QueryAsync<MechanicMaster>(
                  "usp_GetPendingMechanicList",
                  param: Param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<int> UpdateMechanicDb(MechanicUpdateRequest m)
        {
            try
            {
                m.UserName = _user.GetUserName();
                m.CreatedBy = _user.GetPositionName();
                var res = await _db.ExecuteScalarAsync<int>(
                    "USP_UpdateMechanic",
                    m,
                    commandType: CommandType.StoredProcedure);

                return res;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<dynamic>> getAllDealerDb(int status)
        {
            try
            {
                var Param = new { username = _user.GetUserName(), loginAs = _user.GetPositionName(), status = status };
                var res = await _db.QueryAsync(
                  "usp_getDealerByLogin",
                  param: Param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<int> insertReimbursementScoreDb(ReimbursementRequest m, ReimbursementDataTables dt)
        {
            try
            {
                var param = new
                {
                    PointImages = dt.PointImages,
                    Points = dt.Points,
                    m.Date,
                    m.DealerCode,
                    CreatedBy = _user.GetUserName(),
                    LoginAs = _user.GetPositionName(),
                    m.CateType
                };
                var res = await _db.ExecuteScalarAsync<int>(
                    "usp_InsertReimbursementScore",
                    param,
                    commandType: CommandType.StoredProcedure);

                return res;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<PdiReportResponse> getPdiReportDb(PdiReportReqModel m)
        {
            try
            {
                var param = new
                {

                    loginas = _user.GetPositionName(),
                    username = _user.GetUserName(),
                    m.state,
                    m.dealerCode,
                    m.modelName,
                    m.chasisNo,
                    m.rowStart,
                    m.pageSize,
                    m.duration,
                    m.stDate,
                    m.enDate,
                    m.isDownload,
                    m.Type
                };
                using (var multi = await _db.QueryMultipleAsync("usp_pdiReport", param, commandType: CommandType.StoredProcedure))
                {
                    var pdiReport = await multi.ReadAsync();
                    var countResponse = await multi.ReadSingleAsync<PdiCountResponse>();

                    return new PdiReportResponse
                    {
                        pdiReport = pdiReport,
                        countResponse = countResponse
                    };
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<NtirReportResponse> getNtirReportDb(NtirReportReqModel m)
        {
            try
            {
                var param = new
                {

                    loginas = _user.GetPositionName(),
                    username = _user.GetUserName(),
                    m.state,
                    m.dealerCode,
                    m.modelName,
                    m.chasisNo,
                    m.rowStart,
                    m.pageSize,
                    m.duration,
                    m.stDate,
                    m.enDate,
                    m.isDownload,
                    m.Type
                };
                using (var multi = await _db.QueryMultipleAsync("usp_ntirReport", param, commandType: CommandType.StoredProcedure))
                {
                    var pdiReport = await multi.ReadAsync();
                    var countResponse = await multi.ReadSingleAsync<NtirCountResponse>();

                    return new NtirReportResponse
                    {
                        ntirReport = pdiReport,
                        countResponse = countResponse
                    };
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<int> removeJobCardDraftdb(string jobCardMasterId)
        {
            try
            {
                var param = new { username = _user.GetUserName(), loginas = _user.GetPositionName(), jobCardMasterId = Guid.Parse(jobCardMasterId) };
                var res = await _db.ExecuteScalarAsync<int>(
                    "USP_removeJobCardDraft",
                    param,
                    commandType: CommandType.StoredProcedure);

                return res;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }


        public async Task<MechanicDropdownResponse> getMechanicDropdownListdb()
        {
            try
            {
                using (var multi = await _db.QueryMultipleAsync("USP_getMechanicDropdownList", commandType: CommandType.StoredProcedure))
                {
                    var ServiceManagerList = await multi.ReadAsync();
                    var SkillLevelList = await multi.ReadAsync();

                    return new MechanicDropdownResponse
                    {
                        ServiceManagerList = ServiceManagerList,
                        SkillLevelList = SkillLevelList
                    };
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<int> updateJobCardPdfURLdb(string jobCardMasterId, string p)
        {
            try
            {
                var param = new { jobCardMasterId = Guid.Parse(jobCardMasterId), pdfURL = p };
                var res = await _db.ExecuteScalarAsync<int>(
                    "USP_addJobCardPdfURL",
                    param,
                    commandType: CommandType.StoredProcedure);

                return res;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<int> updateInstallationApprovaldb(InstallationApprovalRequest m)
        {
            try
            {
                var param = new { m.ApprovalStatus, m.Remark, InstallationId = Guid.Parse(m.InstallationId), Username = _user.GetUserName() };
                var res = await _db.ExecuteScalarAsync<int>(
                    "USP_updateInstallationApproval",
                    param,
                    commandType: CommandType.StoredProcedure);

                return res;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<GetChassisForFTRResponseModel>> GetChassisDetailsForFTRdb(GetFTRAllDataRequestModel m)
        {
            try
            {
                var Param = new
                {
                    username = _user.GetUserName(),
                    positionName = _user.GetPositionName(),
                    stockStatus = m.stockStatus,
                    pageNo = m.pageNo,
                    m.stDate,
                    m.enDate,
                    m.chassisNo
                };
                var res = await _db.QueryAsync<GetChassisForFTRResponseModel>(
                  "usp_GetChassisDetailsForFTR",
                  param: Param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> addFTRdb(AddFieldTechRequestModel m)
        {
            try
            {
                m.DealerCode = _user.GetDealerCode();
                var res = await _db.ExecuteScalarAsync<int>(
                    "usp_InsertFieldTech",
                    m,
                    commandType: CommandType.StoredProcedure);

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> updateFTRdb(UpdateFTRRequestModel m)
        {
            try
            {
                m.ModifiedBy = _user.GetUserName();
                var res = await _db.ExecuteScalarAsync<int>(
                    "usp_UpdateFieldTech",
                    m,
                    commandType: CommandType.StoredProcedure);

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<dynamic>> getFtrById(Guid? id, Guid? JobCardId)
        {
            try
            {
                var Param = new { username = _user.GetUserName(), positionName = _user.GetPositionName(), ftrMasterId = id, jobCardMasterId = JobCardId };
                var res = await _db.QueryAsync(
                  "usp_getFieldTechDataById",
                  param: Param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<dynamic>> getFtrAlldataDb(GetFTRAllDataRequestModel m)
        {
            try
            {
                var Param = new
                {
                    username = _user.GetUserName(),
                    positionName = _user.GetPositionName(),
                    m.pageNo,
                    m.stDate,
                    m.enDate,
                    m.chassisNo,
                    m.stateId,
                    m.isDownload,
                    m.duration
                };
                var res = await _db.QueryAsync(
                  "usp_getFieldTechAllData",
                  param: Param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<dynamic>> getReimbursementAppListdb()
        {
            try
            {
                var res = await _db.QueryAsync<dynamic>(
                  "usp_ReimbursementScoreMasterApp_GetList",
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<dynamic>> getMechanicCountOnDealerDb(string dealerCode)
        {
            try
            {
                var param = new { dealerCode, username = _user.GetUserName(), positionName = _user.GetPositionName() };
                var res = await _db.QueryAsync<dynamic>(
                  "USP_getMechanicCountOnDealer",
                  param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }


        public async Task<IEnumerable<dynamic>> getItemScoreReportDb(string dealerCode, int halfYearType, int year)
        {
            try
            {
                var param = new { dealerCode, username = _user.GetUserName(), positionName = _user.GetPositionName(), halfYearType, year };
                var res = await _db.QueryAsync<dynamic>(
                  "USP_itemScoreReport",
                  param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<SparePartsOrderResponse> SaveSparePartsOrder(SparePartsOrderRequest model)
        {
            try
            {
                var dt = new DataTable();

                dt.Columns.Add("SparePartId", typeof(int));
                dt.Columns.Add("Quantity", typeof(decimal));

                foreach (var item in model.SparePartsOrderDetails)
                {
                    dt.Rows.Add(
                        item.SparePartId,
                        item.Quantity
                    );
                }

                var param = new DynamicParameters();

                param.Add("DealerId", _user.GetDealerCode());
                param.Add("CreatedBy", _user.GetUserName());
                param.Add("PaymentStatus", model.PaymentStatus);
                param.Add("Remarks", model.Remarks);
                param.Add("PaymentAmount", model.PaymentAmount);
                param.Add("PaymentMode", model.PaymentMode);
                param.Add("TransactionId", model.TransactionId);
                param.Add("ReferenceNo", model.ReferenceNo);
                param.Add("PaymentRemarks", model.PaymentRemarks);

                param.Add(
                    "SparePartsOrderDetails",
                    dt.AsTableValuedParameter("dbo.SparePartsOrderDetailsType")
                );

                var res = await _db.QueryFirstOrDefaultAsync<SparePartsOrderResponse>(
                "usp_SaveSparePartsOrder",
                param,
                commandType: CommandType.StoredProcedure
                );

                return res;

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<UpdateSparePartsPaymentResponse> UpdateSparePartsOrderPayment(UpdateSparePartsPaymentRequest model)
        {
            try
            {
                var param = new
                {
                    OrderId = Guid.Parse(model.OrderId),
                    model.PaymentStatus,
                    model.PaymentAmount,
                    model.PaymentMode,
                    model.TransactionId,
                    model.ReferenceNo,
                    model.PaymentRemarks,
                    UpdatedBy = _user.GetUserName(),
                    PositionName = _user.GetPositionName()
                };

                var res = await _db.QueryFirstOrDefaultAsync<UpdateSparePartsPaymentResponse>(
                    "usp_UpdateSparePartsOrderPaymentStatus",
                    param,
                    commandType: CommandType.StoredProcedure
                );

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<SparePartsOrderDropdownResponse>> GetSparePartsOrderDropdown()
        {
            try
            {
                var res = await _db.QueryAsync<SparePartsOrderDropdownResponse>(
                    "usp_dropDownValueForSparePartsOrder",
                    commandType: CommandType.StoredProcedure
                );

                return res.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }
        public async Task<SparePartsPaginationResponse> getSparePartsPaginationdb(int pageNumber, int pageSize, string partCodeOrName, string category)
        {
            try
            {
                var param = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    PartCodeOrName = partCodeOrName,
                    Category = category
                };

                using var multi = await _db.QueryMultipleAsync(
                    "usp_getSparePartsPagination",
                    param,
                    commandType: CommandType.StoredProcedure);

                var data = (await multi.ReadAsync<SparesPartMaster>()).ToList();

                var totalRecords = await multi.ReadFirstAsync<int>();

                return new SparePartsPaginationResponse
                {
                    Data = data,
                    TotalRecords = totalRecords,
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<SparePartsOrderListResponse> GetSparePartsOrdersdb(
            int pageNumber,
            int pageSize,
            string DealerCode,
            string StateName,
            string Duration,
            DateTime? StDate,
            DateTime? EnDate,
            string OrderNo,
            int IsDownload)
        {
            try
            {
                var param = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Username = _user.GetUserName(),
                    PositionName = _user.GetPositionName(),
                    DealerCode = string.IsNullOrWhiteSpace(DealerCode) ? "All" : DealerCode,
                    StateName = StateName,
                    Duration = Duration,
                    StDate = StDate,
                    EnDate = EnDate,
                    OrderNo = string.IsNullOrWhiteSpace(OrderNo) ? null : OrderNo.Trim(),
                    IsDownload
                };

                using var multi = await _db.QueryMultipleAsync(
                    "usp_GetSparePartsOrders",
                    param,
                    commandType: CommandType.StoredProcedure);

                var data = (await multi.ReadAsync<GetSparePartsOrderResponse>()).ToList();

                var totalRecords = await multi.ReadFirstAsync();

                return new SparePartsOrderListResponse
                {
                    Data = data,
                    TotalRecords = totalRecords.TotalRecords,
                    TotalRecordsValue = totalRecords.TotalRecordsValue,
                    TotalPendingInvoice = totalRecords.TotalPendingInvoice,
                    TotalPendingInvoiceValue = totalRecords.TotalPendingInvoiceValue,
                    TotalPendingPayment = totalRecords.TotalPendingPayment,
                    TotalPendingPaymentValue = totalRecords.TotalPendingPaymentValue,
                    TotalPendingDelivered = totalRecords.TotalPendingDelivered,
                    TotalPendingDeliveredValue = totalRecords.TotalPendingDeliveredValue
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<GetSparePartsOrderDetailResponse>> GetSparePartsOrderDetailsdb(
    string orderId)
        {
            try
            {
                var param = new
                {
                    OrderId = Guid.Parse(orderId),
                    Username = _user.GetUserName(),
                    PositionName = _user.GetPositionName()
                };

                var data = await _db.QueryAsync<GetSparePartsOrderDetailResponse>(
                    "usp_GetSparePartsOrderDetails",
                    param,
                    commandType: CommandType.StoredProcedure);

                return data.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<UpdateSparePartsOrderItemResponse> UpdateSparePartsOrderItem(UpdateSparePartsOrderItemRequest model)
        {
            try
            {
                var param = new
                {
                    OrderDetailId = string.IsNullOrWhiteSpace(model.OrderDetailId) ? (Guid?)null : Guid.Parse(model.OrderDetailId),
                    model.Action,
                    model.Quantity,
                    OrderId = string.IsNullOrWhiteSpace(model.OrderId) ? (Guid?)null : Guid.Parse(model.OrderId),
                    CreatedBy = _user.GetUserName(),
                    PositionName = _user.GetPositionName()
                };

                var res = await _db.QueryFirstOrDefaultAsync<UpdateSparePartsOrderItemResponse>(
                    "usp_UpdateSparePartsOrderItem",
                    param,
                    commandType: CommandType.StoredProcedure);

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CreateSpareOrderInvoiceResponse> CreateSpareOrderInvoice(CreateSpareOrderInvoiceRequest model)
        {
            try
            {
                var param = new
                {
                    OrderId = Guid.Parse(model.OrderId),
                    model.InvoiceNumber,
                    model.InvoiceDate,
                    model.DocketNumber,
                    model.TaxableAmount,
                    model.TaxAmount,
                    model.TotalAmount,
                    model.PDFUrl,
                    CreatedBy = _user.GetUserName(),
                    PositionName = _user.GetPositionName()
                };

                var res = await _db.QueryFirstOrDefaultAsync<CreateSpareOrderInvoiceResponse>(
                    "usp_CreateSpareOrderInvoice",
                    param,
                    commandType: CommandType.StoredProcedure);

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<SpareOrderInvoiceResponse> GetSpareOrderInvoice(string orderId)
        {
            try
            {
                var param = new
                {
                    OrderId = Guid.Parse(orderId),
                    CreatedBy = _user.GetUserName(),
                    PositionName = _user.GetPositionName()
                };

                var res = await _db.QueryFirstOrDefaultAsync<SpareOrderInvoiceResponse>(
                    "usp_GetSpareOrderInvoiceByOrderId",
                    param,
                    commandType: CommandType.StoredProcedure);

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<dynamic>> GetClosedJobCardListdb(ClosedJobCardListRequest request)
        {
            try
            {
                request.PositionName = _user.GetPositionName();
                request.Username = _user.GetUserName();
                var multi = await _db.QueryAsync(
                    "usp_GetClosedJobCardList",
                    request,
                    commandType: CommandType.StoredProcedure);

                return multi;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<dynamic>> getSparePartListByJobCardIddb(Guid Id)
        {
            try
            {
                var param = new { JobCardMasterId = Id };
                var multi = await _db.QueryAsync(
                    "USP_getSparePartByJobCardId",
                    param,
                    commandType: CommandType.StoredProcedure);

                return multi;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> SaveWarrantyClaimdb(WarrantyClaimRequest request)
        {
            try
            {
                var partsTable = new DataTable();
                partsTable.Columns.Add("SparePartId", typeof(Guid));
                partsTable.Columns.Add("PartCode", typeof(string));
                partsTable.Columns.Add("ClaimQty", typeof(decimal));
                partsTable.Columns.Add("Rate", typeof(decimal));
                partsTable.Columns.Add("GSTPercentage", typeof(decimal));

                foreach (var part in request.Parts)
                {
                    partsTable.Rows.Add(
                        part.SparePartId,
                        string.IsNullOrWhiteSpace(part.PartCode) ? part.PartCode : DBNull.Value,
                        part.ClaimQty,
                        part.Rate,
                        part.GSTPercentage
                    );
                }

                var param = new
                {
                    JobCardId = request.JobCardId,
                    ClaimRemarks = request.ClaimRemarks,
                    CreatedBy = _user.GetUserName(),
                    PositionName = _user.GetPositionName(),
                    Parts = partsTable.AsTableValuedParameter("dbo.WarrantyClaimPartsType"),
                    LabourCharge = request.LabourCharge,
                    DealerCode = _user.GetDealerCode()
                };

                var result = await _db.ExecuteScalarAsync<int>(
                    "usp_SaveWarrantyClaim",
                    param,
                    commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<dynamic>> getPendingWarrantyClaimListdb(string ClaimStatus)
        {
            try
            {
                var param = new { ClaimStatus, UserName = _user.GetUserName(), PositionName = _user.GetPositionName() };
                var multi = await _db.QueryAsync(
                    "USP_GetPendingWarrantyClaimMasterList",
                    param: param,
                    commandType: CommandType.StoredProcedure);

                return multi;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<dynamic>> getWarrantySparesByMasterIddb(Guid Id)
        {
            try
            {
                var param = new { Id };
                var multi = await _db.QueryAsync(
                    "USP_WarrantySparesListByMasterId",
                    param,
                    commandType: CommandType.StoredProcedure);

                return multi;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> UpdateWarrantyClaimApprovaldb(WarrantyClaimApprovalRequest request)
        {
            var parameters = new
            {
                WarrantyClaimId = request.WarrantyClaimId,
                PositionName = request.PositionName,
                Action = request.Action,
                Remarks = request.Remarks,
                ModifiedBy = _user.GetUserName()
            };

            var result = await _db.ExecuteScalarAsync<int>(
                "dbo.usp_UpdateWarrantyClaimApproval",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<int> UpdateWarrantyClaimdb(UpdateWarrantyClaimRequest request)
        {
            try
            {
                var partsTable = new DataTable();
                partsTable.Columns.Add("SparePartId", typeof(Guid));
                partsTable.Columns.Add("PartCode", typeof(string));
                partsTable.Columns.Add("ClaimQty", typeof(decimal));
                partsTable.Columns.Add("Rate", typeof(decimal));
                partsTable.Columns.Add("GSTPercentage", typeof(decimal));

                foreach (var part in request.Parts)
                {
                    partsTable.Rows.Add(
                        part.SparePartId,
                        string.IsNullOrWhiteSpace(part.PartCode) ? part.PartCode : DBNull.Value,
                        part.ClaimQty,
                        part.Rate,
                        part.GSTPercentage
                    );
                }

                var param = new
                {
                    WarrantyClaimId = request.WarrantyClaimId,
                    ClaimRemarks = request.ClaimRemarks,
                    CreatedBy = _user.GetUserName(),
                    PositionName = _user.GetPositionName(),
                    Parts = partsTable.AsTableValuedParameter("dbo.WarrantyClaimPartsType"),
                    LabourCharge = request.LabourCharge
                };

                var result = await _db.ExecuteScalarAsync<int>(
                    "usp_UpdateWarrantyClaim",
                    param,
                    commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<getReimbursementScoreListByIdModel> getReimbursementScoreListById(string Id)
        {
            try
            {
                var param = new
                {
                    id = Guid.Parse(Id)
                };

                using (var multi = await _db.QueryMultipleAsync("USP_getReimbursementScoreListById", param, commandType: CommandType.StoredProcedure))
                {
                    var report = (await multi.ReadAsync<ReimbursementScoreDetailModel>()).ToList();
                    var images = (await multi.ReadAsync<ReimbursementScoreImageModel>()).ToList();

                    // Same ReimbursementScoreDetailId wali sabhi images ek list me
                    var imgLookup = images
                        .Where(x => !string.IsNullOrWhiteSpace(x.ImagesUrl))
                        .ToLookup(x => x.ReimbursementScoreDetailId);

                    foreach (var r in report)
                    {
                        r.images = imgLookup[r.Id]
                            .Select(x => x.ImagesUrl.Trim())
                            .Distinct()
                            .ToList();
                    }

                    return new getReimbursementScoreListByIdModel
                    {
                        report = report
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }


        public async Task<IEnumerable<dynamic>> getItemScoreApprovalPendingReportDb()
        {
            try
            {
                var param = new { username = _user.GetUserName(), positionName = _user.GetPositionName() };
                var res = await _db.QueryAsync<dynamic>(
                  "USP_itemScorePendingApprovalReportList",
                  param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> UpdateReimbursementScoreDb(UpdateReimbursementRequest m, ReimbursementDataTables dt)
        {
            try
            {
                var param = new
                {
                    ApprovalId = m.itemScoreId,
                    PointImages = dt.PointImages,
                    Points = dt.Points,
                    CreatedBy = _user.GetUserName(),
                    LoginAs = _user.GetPositionName(),
                };
                var res = await _db.ExecuteScalarAsync<int>(
                    "usp_UpdateReimbursementScore",
                    param,
                    commandType: CommandType.StoredProcedure);

                return res;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<int> ApproveReimbursementScoreDb(ApproveReimbursementRequest m)
        {
            var parameters = new
            {
                username = _user.GetUserName(),
                position = _user.GetPositionName(),
                remark = m.Remark,
                Status = m.Status,
                ApprovalId = m.ApprovalId
            };

            var result = await _db.ExecuteScalarAsync<int>(
                "dbo.usp_approveItemScore",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<ItemScoreEmailDataModel> getItemScoreEmailRequiredDataDb(Guid approvalId)
        {
            try
            {
                var param = new
                {
                    ApprovalId = approvalId
                };

                using (var multi = await _db.QueryMultipleAsync("USP_GetItemScoreEmailRequiredData", param, commandType: CommandType.StoredProcedure))
                {
                    var result = new ItemScoreEmailDataModel
                    {
                        ItemScoreEmail = await multi.ReadFirstOrDefaultAsync<ItemScoreEmailModel>(),
                        _To = (await multi.ReadAsync<string>()).ToList(),
                        _Cc = (await multi.ReadAsync<string>()).ToList(),
                        _Bcc = (await multi.ReadAsync<string>()).ToList()
                    };

                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpdateItemScoreMailStatusdb(bool Status, string FailedMessage, Guid ApprovalId)
        {
            try
            {
                var param = new
                {
                    Status,
                    FailedMessage,
                    ApprovalId
                };
                var res = await _db.ExecuteScalarAsync<int>(
                    "USP_UpdateItemScoreMailStatus",
                    param,
                    commandType: CommandType.StoredProcedure);

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<dynamic> GetWarrantyClaimsCountdb()
        {
            try
            {
                var param = new { UserName = _user.GetUserName(), PositionName = _user.GetPositionName() };
                var res = await _db.QuerySingleOrDefaultAsync<dynamic>(
                  "USP_getWarrantyClaimsCount",
                  param: param,
                  commandType: CommandType.StoredProcedure);
                return res;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
