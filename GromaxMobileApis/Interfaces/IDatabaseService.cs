using GromaxMobileApis.Models;
using GromaxMobileApis.Models.DealerMaster;
using GromaxMobileApis.Models.EmployeeMaster;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml.Drawing.Chart;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GromaxMobileApis.Interfaces
{
    public interface IDatabaseService
    {

        #region MobileApp
        Task InsertToken(string MobileNo, string id);
        Task<bool> ValidateToken(string MobileNo, string id);
        Task<List<VersionMasterDto>> VersionMaster(string platform);
        Task<string> LoginUser(string MobileNo);
        Task<List<Loginmaster>> GetUserData(Loginmaster model);
        Task<List<Loginmaster>> GetUserDatav1(Loginmaster model);
        Task<IEnumerable<dynamic>> GetSalesTarget(string dealercode, string DateFilter, string LoginPosition, string username);
        Task<IEnumerable<dynamic>> GetLeadFollowUp(string DealerCode, string loginas, string username);
        Task<IEnumerable<dynamic>> GetInstallationMaster();
        Task<DataSet> GetInstallationMasterv1(InstallationMasterdto model, string loginas, string loginmail);
        Task<bool> Logout(string MobileNumber);
        Task<IEnumerable<string>> BannerUrl();
        Task<int> GenerateSalesEnquiry(SalesEnquiryMaster SalesEnquery, string dealerCode, string username, string mobileNumber);
        Task<int> GenerateSalesEnquiryv1(SalesEnquiryMaster SalesEnquery, string dealerCode, string username, string mobileNumber, string platform);
        Task<IEnumerable<dynamic>> GetSalesEnquiry(SalesEnquiryMasterdto model, string Dealercode, string username, string LoginPosition);

        Task<IEnumerable<dynamic>> GetPendingSalesFollowupList(SalesEnquiryMaster model, string Mobile, string Dealercode, string LoginPosition, string username);
        Task<int> InsertHistoryEnquiry(HistoryEnquiry model, string P);
        Task<IEnumerable<dynamic>> Getmodelmasterlist();
        Task<IEnumerable<dynamic>> GetHistoryEnquiry(HistoryEnquiry model);
        Task<int> InsertFinanceMaster(FinanceMaster model);
        Task<int> InsertFinanceMasterv1(FinanceMaster model);
        Task<int> InsertFinanceMasterv2(FinanceMaster model, string PlatformType);
        Task<IEnumerable<dynamic>> GetFinanceMaster(FinanceMastersalesenqiddto model);
        Task<int> updtSalesCustomerEnquiry(SalesEnquiryMaster model);
        Task<int> updtSalesCustomerEnquiryv1(SalesEnquiryMaster model);
        Task<int> updtSalesCustomerEnquiryv2(SalesEnquiryMaster model, string p);
        Task<int> updtSalesCustomerProfile(SalesEnquiryMaster model, string p);
        Task<int> InsertInstallationImg(DataTable dt);
        Task<int> InsertInstallationImgv1(DataTable dt, string WorkingHrs);
        Task<int> InsertReturnRequestMaster(ReturnRequestMaster model);
        Task<IEnumerable<ReturnRequestMaster>> GetReturnRequestMaster(ReturnRequestMaster model);
        Task<IEnumerable<dynamic>> getSevenDaySalesEnquiryDelivery(string Mobile, string Dealercode, string username, string loginposition);
        Task<IEnumerable<dynamic>> GetCustomerAddressFilter(string Type, string Name, string Code);
        Task<IEnumerable<dynamic>> GetClosureMasterdb();

        Task<IEnumerable<dynamic>> GetInventoryData(StockMaster model);
        Task<int> MarkStockSolddb(MarkStockSoldModel model);
        Task<int> MarkStockSolddbv1(MarkStockSoldModel model, string P, string username);
        Task<IEnumerable<dynamic>> GetStatedb(string dealercode);
        public Task<IEnumerable<dynamic>> GetSalesEnquiryMaster(SalesEnquiryMasterFiltered model, string LoginPosition, string Username);
        public Task<IEnumerable<dynamic>> GetSalesEnquiryMasterPagination(SalesEnquiryMasterFiltered model, string LoginPosition, string Username);
        #endregion
        Task<IEnumerable<dynamic>> GetSalesEnquiryOnPendingFollowUpdb(SalesEnquiryMasterdto model);
        Task<IEnumerable<dynamic>> GetReturnRequestMasterv1(GetReturnRequestMasterv1 model, string loginas, string username);
        Task<int> GenerateReturnRequest(GenerateReturnRequestv1 model, string p);
        Task<IEnumerable<dynamic>> GetSalesDetailsForReturnRequest(GetReturnRequestMasterv1 model);
        Task<int> ReturnRequestApprovaldb(ReturnRequestApproval model, string p);
        Task<int> ReturnRequestApprovaldbv1(ReturnRequestApproval model);
        Task<IEnumerable<dynamic>> getSevenDaySalesEnquiryDeliveryV1(SuperHotEnquiry model, string Mobile, string Dealercode, string username, string loginposition);

        Task<IEnumerable<dynamic>> GetStatusHistorydb(ReturnRequestApproval model);
        Task<IEnumerable<dynamic>> GetSalesEnquirySearchByMobileNumberdb(SalesEnquirySearchdto model, string Loginas, string UserName);
        Task<IEnumerable<dynamic>> DonwloadInventoryReportdb(StockFilterModel model, string Loginas, string UserName);
        Task<IEnumerable<dynamic>> SearchInventoryReportdb(SearchInventoryReportDto model, string Loginas, string UserName);
        Task<IEnumerable<dynamic>> SearchAvailableInventoryReportdb(SearchInventoryReportDto model, string Loginas, string UserName);
        Task<IEnumerable<dynamic>> DonwloadAvailableInventoryReportdb(StockFilterModel model, string Loginas, string UserName);
        Task<IEnumerable<dynamic>> CheckFillFinanceDatadb(SalesEnquiryMasterdto model);

        Task<IEnumerable<dynamic>> GetSalemanListdb(SalesmanGetdto model, string loginas, string loginmail);
        Task<int> InsertSalesmandb(SalesmanMaster model, string p);
        Task<int> InsertSalesmandbv1(SalesmanMasterv1 model, string p, string file1, string file2, string file3);

        Task<IEnumerable<dynamic>> GetInstallationImagesdb(InstallationImage model);
        Task<IEnumerable<dynamic>> GetSalesmanByDealerCodedb(SalesmanMaster model);
        Task<BussinessPerformanceResult> DownloadBussinessPerformancedb(BussinessPerformance model, string loginas, string username);
        Task<IEnumerable<dynamic>> GetSalesEnquiryForFollowSearchByMobileNumberdb(SalesEnquirySearchdto model, string Loginas, string UserName);

        Task<IEnumerable<dynamic>> DownloadGetPendingSalesFollowupListdb(EnquiryFilterRequest model, string Mobile, string Dealercode, string LoginPosition, string username);
        Task<IEnumerable<dynamic>> GetUserDetaildb(string MobileNo);
        Task<int> CustomerExistsdb(SalesEnquiryMasterdto model);
        Task<int> BlankFCMToken(string MobileNo);
        Task<int> UpdateFcmTokendb(string MobileNo, string FCMToken);
        Task<IEnumerable<dynamic>> DealerlistStatewisedb(string StateCode);
        Task<int> DlrToDlrStockTrfdb(StockMasterdto model);
        Task<int> StockTrfApprovaldb(StockTrfAprovalDto model);
        Task<IEnumerable<dynamic>> GetStockTrfApprovaldb(string Loginas, string Username);
        Task<IEnumerable<dynamic>> GetFcmTokendb();
        Task<IEnumerable<dynamic>> GetStockForStockTransferdb(GetStockForStkTrfdto model, string loginas, string username);
        Task<int> UpdateSalesmanStatusdb(SalesmanStatusdto model);
        Task<IEnumerable<dynamic>> GetNotRetailedSales(RetailedSalesFilter model, string LoginPosition, string username);
        Task<int> UpdateRetailSaledb(RetailSaleRequestv1 model, string username, string PlatformType);

        Task<int> GenerateSalesEnquiryv2(EnquiryMainModel SalesEnquery, string dealerCode, string username, string mobileNumber, string platform, string f);

        Task<IEnumerable<dynamic>> GetSalesEnq2db(string Loginas, string Username, SalesEnquirySearchdto model);

        Task<IEnumerable<dynamic>> getRcrecords(Rc model, string loginas, string username);
        Task<int> updRcrecords(Rc model, string loginas, string username);

        Task<int> updtGenerateSalesEnquiryv2(UpdateEnquiryMainModel SalesEnquery, string dealerCode, string username, string mobileNumber, string platform, string file1);


        Task<dynamic> GetSalesEnqbyId2db(string Loginas, string Username, salesEnquiryIdDto model);
        Task<IEnumerable<dynamic>> GetChassisNumber(string Dealercode);
        Task<dynamic> GetPopUpCountDb(string loginas, string username);
        Task<dynamic> ReportLastDateHeading();
        Task<IEnumerable<dynamic>> AddtionalPaymentRecdb(salesEnquiryIdDto id);
        Task<IEnumerable<dynamic>> getExchangeStockdb(getExchangeStock model, string loginas, string username);
        Task<int> SoldExchdb(SoldExchStock model, string platfmtype, string username);
        Task<IEnumerable<dynamic>> GetOldEnquirydb(getExchangeStock model, string loginas, string username);
        Task<int> UpdateOldEnquirydb(oldEnquiryUpdate model, string platfmtype, string username);
        Task<string> InsertNDAFormdb(NDAFormModel model);
        Task<IEnumerable<dynamic>> GetDistrict(GetDistrict model);
        Task<IEnumerable<dynamic>> GetTehsil(List<GetTehsil> model);
        Task<IEnumerable<dynamic>> GetCity(List<GetCity> model);
        Task<IEnumerable<dynamic>> GetNdadb(RequestGetNDA m);
        Task<int> updtNDAdb(NDAEnquiryModel model);
        Task<IEnumerable<dynamic>> GetNDAByIddb(GetNDAByIDModel model);
        Task<PositionFilterResult> GetStateHeadByStatedb(StateCodeModel model);
        Task<IEnumerable<dynamic>> GetOldRetailedEnquirydb(getExchangeStock model, string loginas, string username);

        Task<IEnumerable<dynamic>> DownloadNotRetailedList(RetailedSalesFilter model, string LoginPosition, string username);

        Task<IEnumerable<dynamic>> downloadExchangeStockdb(getExchangeStock model, string loginas, string username);
        Task<IEnumerable<dynamic>> GetNDAHistoryById(GetNDAByIDModel model);
        Task<int> ApprovalSalesmanKyc(ApprovalSalesmanKyc model);
        Task<IEnumerable<dynamic>> GetKycPendingListdb(SalesmanGetdto model);
        Task<IEnumerable<dynamic>> getLocationListdb(PositionFilter model);
        Task<int> updatedealerdb(DealerUpdate model);
        Task<IEnumerable<dynamic>> GetChassisForReturnBillingdb(Pagignation model);
        Task<int> ReturnBillingdb(ReturnBillingModel model);

        Task<IEnumerable<dynamic>> GetExchangeModelListdb();
        Task<IEnumerable<sourceSubSourceMaster>> getSourceSubSourcedb();
        Task<IEnumerable<sourceSubSourceMaster>> getSourceSubSourceForRepFilter();
        Task<IEnumerable<dynamic>> getbillingReqData(BillingReqModel model);
        Task<dynamic> getUnassignedCountdb();

        Task<IEnumerable<dynamic>> getThreeDaysOdEnquiriesdb(SuperHotEnquiry model);

        Task<IEnumerable<dynamic>> getPendingConversionByListdb(RequestGetNDA m);
        Task<int> getIndustryByTalukadb(int talukaCode);

        Task<whatsappUserDetail> getStateHeadByStateName(string StateName);

        Task<int> updateNDAWhatsappCount(string mid, string message, string mobile, string status, string messageType);


        Task<IEnumerable<dynamic>> getMenuListdb();


        Task<IEnumerable<ResponseEmployeeMaster>> getEmployeeMasterList(RequestEmployeeMaster m);
        Task<int> updtConversionBydb(reqForUpdateConversion m);

        Task<IEnumerable<dynamic>> DealerDetailByDealerCodedb(string dealerCode);

        Task<int> updateDealerAssignmentsdb(UpdateDealerFormRequest m);

        Task<IEnumerable<ResponseEmployeeByPosition>> getEmployeeMasterListByPositionDb(string position);






        #region WebApplication

        #endregion





    }
}
