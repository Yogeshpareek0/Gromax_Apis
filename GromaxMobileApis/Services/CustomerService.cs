using Azure.Storage.Blobs.Models;
using Dapper;
using GromaxMobileApis.Interfaces;
using GromaxMobileApis.Models.DealerMaster;
using GromaxMobileApis.Models.Services;
using GromaxMobileApis.Models.VerifyWebhook;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<dynamic>> jobCardReportdb(JobCardMasterReportRequest m)
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
                var vals = await _db.QueryAsync(
                "usp_jobCardMasterReport",
                param,
                commandType: CommandType.StoredProcedure);
                return vals;
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
                throw new Exception($"Database Error: {ex.Message}");
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
                throw new Exception($"Database Error: {ex.Message}");
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


        public async Task<IEnumerable<ReimbursementScoreMasterModel>> getReimbursementListdb()
        {
            try
            {
                var res = await _db.QueryAsync<ReimbursementScoreMasterModel>(
                  "usp_ReimbursementScoreMaster_GetList",
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
                m.DealerCode = _user.GetDealerCode();
                m.CreatedBy = _user.GetPositionName();
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

        public async Task<IEnumerable<MechanicMaster>> getMechanicListdb(MechanicApprovalFilterRequest m)
        {
            try
            {
                var Param = new { username = _user.GetUserName(), loginAs = _user.GetPositionName(), m.Status, m.StDate, m.EnDate };
                var res = await _db.QueryAsync<MechanicMaster>(
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

    }
}
