using GromaxMobileApis.Models;
using GromaxMobileApis.Models.DealerMaster;
using GromaxMobileApis.Models.Services;
using GromaxMobileApis.Models.VerifyWebhook;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Threading.Tasks;
using static GromaxMobileApis.Models.Services.FieldTechnicalReportModel;

namespace GromaxMobileApis.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<dynamic>> getPDIPendingList(PDIListResponse r);
        Task<int> addPDIdb(PdiRequest m, DataTable dt);
        Task<List<WorkNatureMasterResponse>> getWorkNatureListAsync();

        Task<IEnumerable<dynamic>> getEligibleChassisList(EligibleChassisListRequest r);

        Task<List<SparesPartMaster>> getSparePartsdb();
        Task<int> addJobCard(AddJobCardRequest m);

        Task<IEnumerable<dynamic>> GetServiceTimelinedb(Guid salesMasterId);
        Task<JObCardReportResponse> jobCardReportdb(JobCardMasterReportRequest m);
        Task<int> addOnlineEnqdb(VerifyWebhook m);

        Task<ResponseJobCardMaster> getJobCardByIddb(string jobCardMasterId);

        Task<int> updateJobCard(UpdateJobCardRequest m);

        Task<IEnumerable<dynamic>> getNTIRPendingList(NTIRListResponse r);

        Task<int> addNTIRdb(NTIRRequest m, DataTable dt);

        Task<NTIRMasterReponse> getNTIRByIDdb(Guid Id);
        Task<NTIRMasterReponse> getPDIByIDdb(Guid Id);


        Task<int> updateNTIRdb(NTIRRequest m, DataTable dt);
        Task<int> updatePDIdb(NTIRRequest m, DataTable dt);

        Task<int> addPDIdbv1(NTIRRequest m, DataTable dt);


        Task<int> AddMetaLead(MetaLeadRequest input);
        Task<int> addDealerMasterdb(DealerRequestModel input);

        Task<IEnumerable<dynamic>> getOpenJobCarddb(int pageNo);

        Task<ServiceInvoiceResponse> getServiceInvByIddb(Guid Id, DataTable Ids);

        Task<string> generateServiceInvoicedb(DataTable dt);



        Task<IEnumerable<dynamic>> getFreeServiceClosedJobCarddb();
        Task<IEnumerable<dynamic>> GetPendingServiceInvoiceInstallationdb();

        Task<int> updateServiceInvUrldb(Guid Id, string url);



        Task<string> generateInstallationInvoicedb(DataTable dt);


        Task<IEnumerable<dynamic>> getInvoiceListdb(ServiceInvoiceListResp m);

        Task<int> UpdateReimbursementPaymentStatusDb(UpdateReimbursementStatusRequest m);

        Task<IEnumerable<ReimbursementScoreMasterModel>> getReimbursementListdb();

        Task<int> InsertMechanicDb(MechanicCreateRequest m);
        Task<IEnumerable<MechanicMaster>> getMechanicListdb(MechanicApprovalFilterRequest m);

        Task<int> MechanicApprovalDb(MechanicApprovalRequest m);

        Task<IEnumerable<MechanicMaster>> getMechanicsPendingListDb();

        Task<int> UpdateMechanicDb(MechanicUpdateRequest m);

        Task<IEnumerable<dynamic>> getAllDealerDb(int status);

        Task<int> insertReimbursementScoreDb(ReimbursementRequest m, ReimbursementDataTables dt);


        Task<PdiReportResponse> getPdiReportDb(PdiReportReqModel m);
        Task<NtirReportResponse> getNtirReportDb(NtirReportReqModel m);

        public Task<int> removeJobCardDraftdb(string jobCardMasterId);


        Task<MechanicDropdownResponse> getMechanicDropdownListdb();

        Task<int> updateJobCardPdfURLdb(string jobCardMasterId, string path);

        public Task<int> updateInstallationApprovaldb(InstallationApprovalRequest m);

        Task<IEnumerable<GetChassisForFTRResponseModel>> GetChassisDetailsForFTRdb(string m);
        Task<int> addFTRdb(AddFieldTechRequestModel m);














    }
}