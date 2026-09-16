using System.CodeDom;

namespace GromaxMobileApis.Utilities
{
    public static class ApiRoutes
    {
        private const string LoginBase = "Api/Login/";
        private const string HomeBase = "Api/Home/";

        public static class LogIn
        {
            public const string Login = LoginBase + "Login";
            public const string Loginv1 = LoginBase + "Loginv1";
            public const string Logout = LoginBase + "Logout";
        }
        public static class PossitionMaster
        {
            public const string PossitionFilter = HomeBase + "PossitionFilter";
            public const string GetStateHeadByState = HomeBase + "GetStateHeadByState";

        }
        public static class FinanceMaster
        {
            public const string Insert = HomeBase + "InsertFinanceMaster";
            public const string Insertv1 = HomeBase + "InsertFinanceMasterv1";
            public const string Insertv2 = HomeBase + "InsertFinanceMasterv2";
            public const string Get = HomeBase + "GetFinanceMaster";
            public const string CheckFillFinanceData = HomeBase + "CheckFillFinanceData";
            public const string AddtionalPaymentRec = HomeBase + "AddtionalPaymentRec";
        }

        public static class ReturnRequestMaster
        {
            public const string Insert = HomeBase + "InsertReturnRequestMaster";
            public const string Get = HomeBase + "GetReturnRequestMaster";
            public const string Getv1 = HomeBase + "GetReturnRequestMasterv1";
            public const string Generate = HomeBase + "GenerateReturnRequest";
            public const string GetSalesDetailsForReturnRequest = HomeBase + "GetSalesDetailsForReturnRequest";
            public const string ReturnRequestApproval = HomeBase + "ReturnRequestApproval";
            public const string ReturnRequestApprovalv1 = HomeBase + "ReturnRequestApprovalv1";
            public const string GetStatusHistory = HomeBase + "GetStatusHistory";


        }
        public static class SalesEnquiry
        {
            public const string GetPendingFollowUp = HomeBase + "PendingSalesFollowupList";
            public const string GetPendingFollowUpv1 = HomeBase + "PendingSalesFollowupListv1";
            public const string Get = HomeBase + "GetSalesEnquiry";
            public const string GetSalesEnquiryOnPendingFollowUp = HomeBase + "GetSalesEnquiryOnPendingFollowUp";
            public const string Insert = HomeBase + "GenerateEnquiry";
            public const string Insertv1 = HomeBase + "GenerateEnquiryv1";
            public const string GetLeadsFollowup = HomeBase + "GetLeadsFollowUp";
            public const string updCustomerProfile = HomeBase + "UpdateSalesCustomerProfile";
            public const string updCustomerEnquiry = HomeBase + "UpdateSalesCustomerEnquiry";
            public const string updCustomerEnquiryv1 = HomeBase + "UpdateSalesCustomerEnquiryv1";
            public const string updCustomerEnquiryv2 = HomeBase + "UpdateSalesCustomerEnquiryv2";
            public const string getSevenDaySalesEnquiryDelivery = HomeBase + "GetSevenDaySalesEnquiryDelivery";
            public const string getSevenDaySalesEnquiryDeliveryv1 = HomeBase + "GetSevenDaySalesEnquiryDeliveryv1";
            public const string GetSalesEnquiryMasterweb = HomeBase + "GetSalesEnquiryMasterweb";
            public const string GetSalesEnquiryMasterPagination = HomeBase + "GetSalesEnquiryMasterPagination";
            public const string GetSalesEnquirySearchByMobileNumber = HomeBase + "GetSalesEnqByMobileNo";
            public const string GetSalesEnqForFollowByMobileNo = HomeBase + "GetSalesEnqForFollowByMobileNo";
            public const string DownloadGetPendingFillowupList = HomeBase + "DownloadGetPendingFillowupList";
            public const string CustomerExists = HomeBase + "CustomerExists";
            public const string GenerateEnquiryV2 = HomeBase + "GenerateEnquiryv2";
            public const string GenerateEnquiryV3 = HomeBase + "GenerateEnquiryv3";
            public const string GetsalesEnq2 = HomeBase + "GetSalesEnquiry2";
            public const string UpdtGenerateEnquiryv2 = HomeBase + "UpdtGenerateEnquiryv2";
            public const string UpdtGenerateEnquiryv3 = HomeBase + "UpdtGenerateEnquiryv3";
            public const string GetSalesEnquiryById2 = HomeBase + "GetSalesEnquiryById2";
            public const string getSourceSubSource = HomeBase + "getSourceSubSource";
            public const string getSourceSubSourceForRepFilter = HomeBase + "getSourceSubSourceForRepFilter";
            public const string getUnassignedCount = HomeBase + "getUnassignedCount";
            public const string getThreeDaysOdEnquiries = HomeBase + "getThreeDaysOdEnquiries";






        }

        public static class HistoryEnquiry
        {
            public const string Get = HomeBase + "GetHistoryEnquiry";
            public const string Insert = HomeBase + "AddHistoryEnquiry";
        }

        public static class ModelMaster
        {
            public const string Get = HomeBase + "Getmodelmaster";
        }

        public static class InstallationMaster
        {
            public const string Get = HomeBase + "GetInstallation";
            public const string Getv1 = HomeBase + "GetInstallationv1";
            public const string InsertImg = HomeBase + "InsertInstallationImg";
            public const string InsertImgv1 = HomeBase + "InsertInstallationImgv1";
            //public const string InsertImg = HomeBase + "InsertInstallationImg";
            public const string GetImages = HomeBase + "GetImagesOnId";
        }

        public static class Other
        {
            public const string GetBanner = HomeBase + "GetBanner";
            public const string GetVersion = HomeBase + "Version";
            public const string GetSalesTarget = HomeBase + "GetsalesTarget";
            public const string GetUserDetail = HomeBase + "GetUserDetail";
            public const string UpdateFcmToken = HomeBase + "UpdateFcmToken";
            public const string getLocationList = HomeBase + "getLocationList";
            public const string updatedealer = HomeBase + "updatedealer";
            public const string getbillingReqData = HomeBase + "getbillingReqData";
            public const string getDealerByTehsil = HomeBase + "getDealerByTehsil";

            //public const string GetStateListByLogin = HomeBase + "GetStateListByLogin";
        }

        public static class NotAssign
        {
            public const string notAssignDealerList = HomeBase + "notAssignDealerList";
            public const string assignDealerToEnquiry = HomeBase + "assignDealerToEnquiry";

        }

        public static class StateDistrictTehsilMaster
        {
            public const string Filter = HomeBase + "GetCustomerAddressFilter";
            public const string GetState = HomeBase + "GetState";

        }
        public static class MasterClosureReason
        {
            public const string GetClosureMaster = HomeBase + "GetClosureMaster";
        }

        public static class StockMaster
        {
            public const string GetInventoryData = HomeBase + "GetInventoryData";
            public const string GetInventoryDataweb = HomeBase + "GetInventoryDataweb";
            public const string GetInventoryDatawebv1 = HomeBase + "GetInventoryDatawebv1";
            public const string GetInventoryDataPagination = HomeBase + "GetInventoryDataPagination";
            public const string GetInventoryDataPaginationv1 = HomeBase + "GetInventoryDataPaginationv1";
            public const string MarkStockSold = HomeBase + "MarkStockSold";
            public const string MarkStockSoldv1 = HomeBase + "MarkStockSoldv1";
            public const string DownloadInventoryReport = HomeBase + "DownloadInventoryReport";
            public const string DownloadAvailableInventoryReport = HomeBase + "DownloadAvailableInventoryReport";
            public const string SearchInventoryReport = HomeBase + "SearchInventoryReport";
            public const string SearchAvailableInventoryReport = HomeBase + "SearchAvailableInventoryReport";
            public const string DlrToDlrStockTransfer = HomeBase + "DlrToDlrStockTransfer";
            public const string StockTrfApproval = HomeBase + "StockTrfApproval";
            public const string GetStockTrfApproval = HomeBase + "GetStockTrfApproval";
            public const string GetStockForStockTransfer = HomeBase + "GetStockForStockTransfer";
            public const string GetAvailableChassisNo = HomeBase + "GetAvailableChassisNo";
            public const string GetChassisForReturnBilling = HomeBase + "GetChassisForReturnBilling";
            public const string ReturnBilling = HomeBase + "ReturnBilling";
        }
        public static class BussinessPerformance
        {
            public const string Get = HomeBase + "GetBussinessPerformace";
            public const string Getv1 = HomeBase + "GetBussinessPerformacev1";
            public const string DownloadBussinessPerformace = HomeBase + "GetBussinessPerformaceDownloadWeb";

        }

        public static class SalesmanMaster
        {
            public const string Get = HomeBase + "GetSalesmanList";
            public const string Insert = HomeBase + "InsertSalesman";
            public const string Insertv1 = HomeBase + "InsertSalesmanv1";
            public const string GetByDealerCode = HomeBase + "GetSalesmanByDealerCode";
            public const string UpdateSalesManStatus = HomeBase + "UpdateSalesManStatus";
            public const string ApproveSalesmanKyc = HomeBase + "ApproveSalesmanKyc";
            public const string GetKycPendingList = HomeBase + "GetKycPendingList";
            //public const string GetImages = HomeBase + "GetImagesOnId";

        }

        public static class FirebaseNotification
        {
            public const string FirebaseNotificationApi = HomeBase + "FirebaseNotification";


        }
        public static class DealerMaster
        {
            public const string DealerListStatewise = HomeBase + "DealerListStateWise";
            public const string DealerDetailByDealerCode = HomeBase + "DealerDetailByDealerCode";
            public const string updateDealerAssignments = HomeBase + "updateDealerAssignments";

        }
        public static class Retiledsales
        {
            public const string NotRetailedList = HomeBase + "NotRetailedList";
            public const string DownloadNotRetailedList = HomeBase + "DownloadNotRetailedList";
            public const string UpdateRetailSale = HomeBase + "UpdateRetailSale";

        }

        public static class Rc
        {
            public const string GetRcStatusList = HomeBase + "GetRcStatusList";
            public const string UpdateRcStatus = HomeBase + "UpdateRcStatus";

        }

        public static class PopUp
        {

            public const string GetPopUpCount = HomeBase + "GetPopUpCount";
            public const string ReportLastDateHeading = HomeBase + "ReportLastDateHeading";

        }

        public static class GetExchangeStock
        {
            public const string GetExch = HomeBase + "GetExch";
            public const string DownloadExch = HomeBase + "DownloadExch";
            public const string SoldMarkExch = HomeBase + "SoldMarkExch";
            public const string GetExchangeModelList = HomeBase + "GetExchangeModelList";

        }
        public static class OldEnquiryTemp
        {
            public const string GetOldEnquiry = HomeBase + "GetOldEnquiry";
            public const string GetOldRetailedEnquiry = HomeBase + "GetOldRetailedEnquiry";
            public const string UpdateOldEnquiry = HomeBase + "UpdateOldEnquiry";

        }

        public static class NDAForm
        {
            public const string InsertNDAForm = HomeBase + "InsertNDAForm";
            public const string GetDistrict = HomeBase + "GetDistrict";
            public const string GetTehsil = HomeBase + "GetTehsil";
            public const string GetCity = HomeBase + "GetCity";
            public const string GetNda = HomeBase + "getNdaEnquiryList";
            public const string updtNDA = HomeBase + "updtNDA";
            public const string GetNDAById = HomeBase + "GetNDAById";
            public const string GetNDAHistoryById = HomeBase + "GetNDAHistoryById";

            public const string getPendingConversionByList = HomeBase + "getPendingConversionByList";
            public const string getIndustryByTaluka = HomeBase + "getIndustryByTaluka";
            public const string updtConversionBy = HomeBase + "updtConversionBy";

        }

        public static class Menu
        {
            public const string getMenuList = HomeBase + "getMenuList";

        }
        public static class Employee
        {
            public const string getEmployeeList = HomeBase + "getEmployeeList";
            public const string getEmployeeListByPosition = HomeBase + "getEmployeeListByPosition";

        }
    }


    public static class WebApiRoutes
    {
        private const string LoginBase = "Webapi/Login/";
        private const string HomeBase = "Webapi/Home/";

        public static class LogIn
        {
            public const string Login = LoginBase + "Login";
            public const string Loginv1 = LoginBase + "Loginv1";
            public const string Logout = LoginBase + "Logout";
            public const string UserExists = LoginBase + "UserExists";
            public const string UpdatePassword = LoginBase + "UpdatePassword";
        }
        public static class SalesEnquiryMaster
        {
            public const string GetSalesEnquiryMaster = HomeBase + "WebGetSalesEnquiryMaster";
            public const string GetSalesEnquiryMasterv1 = HomeBase + "WebGetSalesEnquiryMasterv1";
            public const string GetLeadsByFilter = HomeBase + "GetLeads";
            public const string GetLeadsFollowup = HomeBase + "GetLeadsFollowup";
            public const string GetSalesTarget = HomeBase + "GetSalesTarget";
            public const string Insert = HomeBase + "GenerateEnquiry";
            public const string Get = HomeBase + "GetSalesEnquiry";
            public const string DownloadSalesEnquiryMaster = HomeBase + "DownloadSalesEnquiryMaster";
            public const string GetPendingFillowupList = HomeBase + "GetPendingFillowupList";

        }
        public static class PossitionMaster
        {
            public const string PossitionFilter = HomeBase + "PossitionFilter";

        }
        public static class StockMaster
        {
            public const string GetInventoryData = HomeBase + "GetInventoryData";
            public const string GetAvailableStockData = HomeBase + "GetAvailableStockData";
            public const string InsertStock = HomeBase + "InsertStock";
            public const string InsertBilling = HomeBase + "InsertBilling";
        }
        public static class BussinessPerformance
        {
            public const string Get = HomeBase + "GetBussinessPerformace";
            public const string Getv1 = HomeBase + "GetBussinessPerformacev1";
        }
        public static class ModelMaster
        {
            public const string Get = HomeBase + "Getmodelmaster";
        }
        public static class StateDistrictTehsilMaster
        {
            public const string Filter = HomeBase + "GetCustomerAddressFilter";
            public const string GetState = HomeBase + "GetState";

        }
        public static class CampaignsalesEnquiry
        {
            public const string UploadCampaignsalesEnquiry = HomeBase + "UploadCampaignsalesEnquiry";

        }
    }


    public class GromaxReports
    {
        private const string Base = "Api/Report/";
        public const string Preview = Base + "Preview";
        public const string Commit = Base + "Commit";
        public const string Commitv1 = Base + "Commitv1";
        public const string GetAllDashboards = Base + "GetAllDashboards";
        public const string GetDashboardsConfig = Base + "GetDashboardsConfig";
        public const string GetDashboardsConfigv1 = Base + "GetDashboardsConfigv1";
        public const string GetSavedReport = Base + "GetSavedReport";
        public const string GetUserDefineTable = Base + "GetUserDefineTable";
        public const string GetUserDefineColumns = Base + "GetUserDefineColumns";
        public const string GetUserDefineProcedures = Base + "GetUserDefineProcedures";
        public const string GetUserDefineParameter = Base + "GetUserDefineParameter";
        public const string InsertReportConfig = Base + "InsertReportConfig";
        public const string SaveDashboardConfig = Base + "SaveDashboardConfig";
        public const string GetUploadMasterList = Base + "GetUploadMasterList";
        public const string GetDropdownList = Base + "GetDropdownList";
        public const string GetStateListByLogin = Base + "GetStateListByLogin";
        public const string GetDealerByState = Base + "GetDealerByState";
        public const string GetDealerAccByState = Base + "GetDealerAccByState";
        public const string GetModelListByState = Base + "GetModelListByState";
        public const string getBrand_HpForIndustry = Base + "GetBrandHpForIndustry";
        public const string GetDistrictAndTalukaList = Base + "GetDistrictAndTalukaList";
        public const string GetActivityNameList = Base + "GetActivityNameList";
        public const string GetGroupColumnJson = Base + "GetGroupColumnJson";
        public const string GetReportTracking = Base + "GetReportTracking";
        public const string BDRCReportApproval = Base + "BDRCReportApproval";
        public const string ForecastReportApproval = Base + "ForecastReportApproval";
        public const string insertPricePosition = Base + "insertPricePosition";
        public const string PricePosiReportApproval = Base + "PricePosiReportApproval";
        public const string UpdatePricePosiReport = Base + "UpdatePricePosiReport";
        public const string UpdatePricePosiReportv1 = Base + "UpdatePricePosiReportv1";
        public const string UpdatePricePosiReportv2 = Base + "UpdatePricePosiReportv2";
        public const string ApprovalPricePosiReport = Base + "ApprovalPricePosiReport";
        public const string GetPricePositionReport = Base + "GetPricePositionReport";
        public const string GetStateListByLoginNew = Base + "GetStateListByLoginNew";
        public const string getOutlookFormatData = Base + "getOutlookFormatData";
        public const string getRevisedForExcel = Base + "getRevisedForExcel";
        public const string getPdd = Base + "getPdd";



        public const string send90DaysPendingMessage = Base + "send90DaysPendingMessage";
        public const string getTMByDistCode = Base + "getTMByDistCode";


        //public const string Loginv1 = LoginBase + "Loginv1";
        //public const string Logout = LoginBase + "Logout";
        //public const string UserExists = LoginBase + "UserExists";
        //public const string UpdatePassword = LoginBase + "UpdatePassword";


    }


    public static class customerServices
    {
        private const string Base = "Api/Services/";
        public const string pdiList = Base + "pdiList";
        public const string addPDI = Base + "addPDI";
        public const string addPDIv1 = Base + "addPDIv1";
        public const string uploadPDIImage = Base + "uploadPDIImage";
        public const string removeImage = Base + "removeImage";
        public const string getWorkNature = Base + "getWorkNature";
        public const string jobCardEligibleChassis = Base + "jobCardEligibleChassis";
        public const string GetServiceTimeline = Base + "GetServiceTimeline";
        public const string Webhook = Base + "Webhook";

        public const string ntirList = Base + "ntirList";
        public const string addNTIR = Base + "addNTIR";
        public const string updateNTIR = Base + "updateNTIR";
        public const string updatePDI = Base + "updatePDI";

        public const string getNTIRByID = Base + "getNTIRByID";
        public const string getPDIByID = Base + "getPDIByID";

        public const string uploadNTIRImage = Base + "uploadNTIRImage";
        public const string MetaLead = Base + "MetaLead";


        public const string GetPendingServiceInvoiceInstallation = Base + "GetPendingServiceInvoiceInstallation";
        public const string updateReimbursementPaymentStatus = Base + "updateReimbursementPaymentStatus";




        public static class spareParts
        {
            public const string getSpareParts = Base + "getSpareParts";
        }

        public static class jobCard
        {
            public const string addJobCard = Base + "addJobCard";
            public const string jobCardReport = Base + "jobCardReport";
            public const string getJobCardById = Base + "getJobCardById";
            public const string updateJobCard = Base + "updateJobCard";
            public const string getOpenJobCard = Base + "getOpenJobCard";
            public const string getFreeServiceClosedJobCard = Base + "getFreeServiceClosedJobCard";


        }

        public static class invoice
        {
            public const string service = Base + "serviceInvoice";
            public const string generateService = Base + "generateServiceInvoice";
            public const string generateInstallationInvoice = Base + "generateInstallationInvoice";
            public const string getInvoiceList = Base + "getInvoiceList";

        }

        public static class reimbursement
        {
            public const string getReimbursementList = Base + "getReimbursementList";

        }

        public static class DealerMaster
        {
            public const string addDealerMaster = Base + "addDealerMaster";

        }
        public static class mechanicServices
        {
            public const string insertMechanic = Base + "insertMechanic";
            public const string updateMechanic = Base + "updateMechanic";
            public const string getMechanicList = Base + "getMechanicList";
            public const string approvalstatus = Base + "approval-status";
            public const string getMechanicsPendingList = Base + "getMechanicsPendingList";


        }



    }




}
