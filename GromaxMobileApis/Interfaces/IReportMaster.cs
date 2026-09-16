using GromaxMobileApis.Models;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static GromaxMobileApis.Models.ReportMasterModel;

namespace GromaxMobileApis.Interfaces
{
    public interface IReportMaster
    {
        Task<UploadMaster> GetUploadMasterById(int uploadID);
        Task<List<UploadColumnMapping>> GetMappings(int uploadID);
        Task<int> MoveDatadb(int Month, int Year, string StagingTable, int UploadId, string createuser);
        Task<IEnumerable<dynamic>> MoveDatadbv1(int Month, int Year, string StagingTable, int UploadId, string createuser, string loginposition, string loginName);
        Task<IEnumerable<dynamic>> GetDashboardData(string LoginPosition, string username);
        Task<dynamic> GetDashboardConfigJson(int Id);

        Task<DashboardViewModel> GetDashboardConfigJsonv1(int Id);
        Task<dynamic> GetSavedReports(int Id);
        Task<IEnumerable<dynamic>> GetUserDefineTablesdb();
        Task<IEnumerable<dynamic>> GetUserDefineColumnsdb(int Id);
        Task<IEnumerable<dynamic>> GetUserDefineProceduresdb();
        Task<IEnumerable<dynamic>> GetUserDefineParameterdb(int Id);
        Task<int> InsertReportConfigdb(ReportsConfigModel model);
        Task<int> SaveDashboardConfigdb(DashboardViewModel model);
        //Task<IEnumerable<dynamic>> ExecuteStoredProcedure(string procedureName, Dictionary<string, object> paramValues);
        Task<IEnumerable<dynamic>> ExecuteSqlQuery(string Sql, List<ReportParameterV5> Param);
        Task<IEnumerable<dynamic>> ExecuteStoredProcedure(string procedureName, List<ReportParameterV5> parameter);

        Task<IEnumerable<dynamic>> GetUploadMasterListdb();
        Task<List<string>> Dropdownlistdb(string Query);
        Task<IEnumerable<string>> GetStateList(string username, string loginas);
        Task<IEnumerable<dynamic>> GetDealerByState(string username, string loginas, StateModel model);
        Task<IEnumerable<dynamic>> GetDealerAccByState(string username, string loginas, StateModel model);
        Task<IEnumerable<dynamic>> GetModelListByState(string username, string loginas, StateModel model);
        Task<IEnumerable<dynamic>> getBrand_HpForIndustry(string username, string loginas, StateModel model);
        Task<IEnumerable<dynamic>> GetDistrictAndTalukaList(string username, string loginas, StateModel model);
        Task<IEnumerable<dynamic>> GetActivityNameListdb(string username, string loginas);
        Task<string> GetGroupColumnJsondb(string username, string loginas, ReportParameterV5 model);
        Task<IEnumerable<DynamicGrouping>> GetDynamicGroupColumnJsondb(string username, string loginas, ReportParameterV5 model);
        Task<IEnumerable<dynamic>> ReportTrackingdb(ReportTrackingFilter model, string username, string loginas);
        Task<int> updtBDRCModel(DataTable dt, string username, string loginas, string status, string r, string loginname);
        Task<int> updtForecasttbl(DataTable dt, string username, string loginas, string status, string r, string loginname);
        Task<int> insertPricePositiondb(PricePositionModel model, string mopProofString, string rcCopyString);
        Task<int> UpdatePricePosiReportdb(DataTable dt);
        Task<int> UpdatePricePosiReportdbv1(DataTable dt);
        Task<int> UpdatePricePosiReportdbv2(DataTable dt);
        Task<int> ApprovalPricePosiReportdb(string jsonIds, string Remarks, int Iscommitted);

        Task<IEnumerable<dynamic>> GetPricePositionReportdb(string username, string loginas);

        Task<IEnumerable<dynamic>> GetStateListNew();
        Task<IEnumerable<dynamic>> getOutlookFormatDatadb(int month, int year);
        Task<IEnumerable<dynamic>> getRevisedForExceldb(RevisedBDRCRequest m);
        Task<List<AdvanceMoreThan90Whatsapp>> get90DaysAdvancedb();

        Task<IEnumerable<dynamic>> getTMByDistCodedb(int distcode);

        Task<IEnumerable<dynamic>> getPdddb(int month, int year);





    }
}
