using GromaxMobileApis.Models;
using GromaxMobileApis.Models.DealerMaster;
using GromaxMobileApis.Models.Services;
using GromaxMobileApis.Models.VerifyWebhook;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Threading.Tasks;
using static GromaxMobileApis.Models.Services.FieldTechnicalReportModel;
using static GromaxMobileApis.Models.Services.SparePartsModel;
using static GromaxMobileApis.Models.Services.WarrantyClaimModel;

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

        Task<IEnumerable<ReimbursementScoreMasterModel>> getReimbursementListdb(string dealerCode);
        Task<IEnumerable<dynamic>> getReimbursementAppListdb();

        Task<int> InsertMechanicDb(MechanicCreateRequest m);
        Task<IEnumerable<MechanicMasterDataModel>> getMechanicListdb(MechanicApprovalFilterRequest m);

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

        Task<IEnumerable<GetChassisForFTRResponseModel>> GetChassisDetailsForFTRdb(GetFTRAllDataRequestModel m);
        Task<int> addFTRdb(AddFieldTechRequestModel m);
        Task<int> updateFTRdb(UpdateFTRRequestModel m);

        Task<IEnumerable<dynamic>> getFtrById(Guid id);
        Task<IEnumerable<dynamic>> getFtrAlldataDb(GetFTRAllDataRequestModel m);

        Task<IEnumerable<dynamic>> getMechanicCountOnDealerDb(string dealerCode);
        Task<IEnumerable<dynamic>> getItemScoreReportDb(string dealerCode, int halfYearType, int year);

        Task<SparePartsOrderResponse> SaveSparePartsOrder(SparePartsOrderRequest model);
        Task<UpdateSparePartsPaymentResponse> UpdateSparePartsOrderPayment(UpdateSparePartsPaymentRequest model);
        Task<List<SparePartsOrderDropdownResponse>> GetSparePartsOrderDropdown();
        Task<SparePartsPaginationResponse> getSparePartsPaginationdb(int pageNumber, int pageSize, string partCodeOrName, string category);


        Task<SparePartsOrderListResponse> GetSparePartsOrdersdb(
        int pageNumber,
        int pageSize,
        string DealerCode,
        string StateName,
        string Duration,
        DateTime? StDate,
        DateTime? EnDate,
        string OrderNo,
        int IsDownload);

        Task<List<GetSparePartsOrderDetailResponse>> GetSparePartsOrderDetailsdb(string orderId);

        Task<UpdateSparePartsOrderItemResponse> UpdateSparePartsOrderItem(UpdateSparePartsOrderItemRequest model);
        Task<CreateSpareOrderInvoiceResponse> CreateSpareOrderInvoice(CreateSpareOrderInvoiceRequest model);
        Task<SpareOrderInvoiceResponse> GetSpareOrderInvoice(string orderId);


        Task<IEnumerable<dynamic>> GetClosedJobCardListdb(ClosedJobCardListRequest request);
        Task<IEnumerable<dynamic>> getSparePartListByJobCardIddb(Guid Id);

        Task<int> SaveWarrantyClaimdb(WarrantyClaimRequest request);

        Task<IEnumerable<dynamic>> getPendingWarrantyClaimListdb();
        Task<IEnumerable<dynamic>> getWarrantySparesByMasterIddb(Guid Id);
        Task<int> UpdateWarrantyClaimApprovaldb(WarrantyClaimApprovalRequest request);
        Task<int> UpdateWarrantyClaimdb(UpdateWarrantyClaimRequest request);

        Task<getReimbursementScoreListByIdModel> getReimbursementScoreListById(string Id);
        Task<IEnumerable<dynamic>> getItemScoreApprovalPendingReportDb();

        Task<int> UpdateReimbursementScoreDb(UpdateReimbursementRequest m, ReimbursementDataTables dt);
        Task<int> ApproveReimbursementScoreDb(ApproveReimbursementRequest m);
        Task<ItemScoreEmailDataModel> getItemScoreEmailRequiredDataDb(Guid approvalId);

        Task<int> UpdateItemScoreMailStatusdb(bool Status, string FailedMessage,Guid ApprovalId);



















    }
}