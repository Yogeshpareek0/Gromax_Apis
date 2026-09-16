using GromaxMobileApis.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using OfficeOpenXml.Drawing.Chart;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GromaxMobileApis.Interfaces
{
    public interface IDatabaseServicesweb
    {
        Task<string> LoginUser(string MobileNo);
        Task<string> LoginUserv1(string MobileNo, string password);
        Task<GetLoginmasterweb> GetUserData(Loginmasterdto model);
        Task<IEnumerable<dynamic>> GetSalesEnquiryMaster(SalesEnquiryMasterFiltered model, string LoginPosition, string Username);
        Task<IEnumerable<dynamic>> GetSalesEnquiryMasterv1(SalesEnquiryMasterFiltered model, string LoginPosition, string Username);
        Task<PositionFilterResult> GetPossitionFilter(PositionFilter model, string loginas, string username);
        Task<IEnumerable<dynamic>> GetInventoryData(StockFilterModel model, string loginas, string username);
        Task<IEnumerable<dynamic>> GetInventoryDatav1(StockFilterModel model, string loginas, string username);
        //Task<IEnumerable<dynamic>> GetLeads(FilterModel model,string loginas,string username);
        Task<dynamic> GetLeadsFollowUpFilterWeb(string loginas, string username);
        Task<IEnumerable<dynamic>> GetLeads(SalesEnquiryMasterFiltered model, string loginas, string username);
        Task<BussinessPerformanceResult> GetBussinessPerformance(BussinessPerformance model, string loginas, string username);
        Task<BussinessPerformanceResult> GetBussinessPerformanceWeb(BussinessPerformance model, string loginas, string username);
        Task<BussinessPerformanceResult> GetBussinessPerformanceWebv1(BussinessPerformance model, string loginas, string username);
        Task<int> InsertStockdb(DataTable file);
        Task<IEnumerable<dynamic>> DownloadSalesEnquiryMasterdb(SalesEnquiryMasterFiltered model, string LoginPosition, string Username);
        Task<IEnumerable<dynamic>> GetInventoryDataWeb(StockFilterModel model, string loginas, string username);
        Task<int> UploadCampaignSalesEnquiry(DataTable dt);
        Task<IEnumerable<dynamic>> GetPendingSalesFollowupList(SalesEnquiryMaster model, string Mobile, string Dealercode, string LoginPosition, string username);
        Task<IEnumerable<dynamic>> GetPendingSalesFollowupListv1(EnquiryFilterRequest model, string Mobile, string Dealercode, string LoginPosition, string username);
        Task<IEnumerable<dynamic>> GetLeadFollowUp(string DealerCode, string loginas, string username);
        Task<IEnumerable<dynamic>> GetInventoryDataWebv1(StockFilterModel model, string loginas, string username);
        Task<IEnumerable<dynamic>> GetAvailableStockDatadb(StockFilterModel model, string loginas, string username);
        Task<int> ExistsUserdb(string MobileNo);
        Task<int> UpdatePassworddb(string MobileNo, string Password);
        Task<int> InsertBillingdb(DataTable file);
        Task<IEnumerable<dynamic>> getDealerByTehsildb(DealerListByLocationModel model);
        Task<IEnumerable<dynamic>> notAssignDealerList(NotAssign model, string username);
        Task<bool> assignDealerToEnquiry(AssignDealerModel model);
    }
}
