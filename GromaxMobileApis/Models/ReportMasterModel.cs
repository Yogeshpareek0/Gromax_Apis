using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Data;

namespace GromaxMobileApis.Models
{
    public class ReportMasterModel
    {
        public class UploadColumnMapping
        {
            public int MappingID { get; set; }
            public int UploadID { get; set; }
            public string ExcelColumn { get; set; }
            public string TargetColumn { get; set; }
            public string DataType { get; set; }
            public bool IsMandatory { get; set; }
        }

        public class UploadPreviewResponse
        {
            public string StagingTable { get; set; }
            public List<UploadColumnMapping> Mappings { get; set; }
            public string TargetTable { get; set; }
            public string UploadName { get; set; }
            public int UploadId { get; set; }

            public int Total { get; set; }
            public int Valid { get; set; }
            public int Invalid { get; set; }
            public List<string> Unmatched { get; set; }

            public int Month { get; set; }
            public int Year { get; set; }
            //public DataTable Preview { get; set; }
            public List<Dictionary<string, object>> Preview { get; set; }
        }
        public class UploadMaster
        {
            public int UploadID { get; set; }
            public string UploadName { get; set; }
            public string TargetTable { get; set; }
        }

        public class Dashboard
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Theme { get; set; }
            public string ConfigJson { get; set; }
        }

        public class ReportsConfigModel
        {
            public int sectionId { get; set; }
            public string reportName { get; set; }
            public string reportConfigJson { get; set; }


        }

        #region Extract Report Config column
        public class ExtractReportConfigModel
        {
            public string SourceType { get; set; }
            public ViewConfig ViewConfig { get; set; }

            // Used when SourceType = "StoredProcedure"
            public string StoredProcedureName { get; set; }
            public List<Parameter> Parameters { get; set; }

            // Used when SourceType = "QueryBuilder"
            public string BaseTable { get; set; }
            public bool Distinct { get; set; }
            public int? Top { get; set; }
            public List<Columns> Columns { get; set; }
            public List<Join> Joins { get; set; }
            public List<WhereCondition> Where { get; set; }
            public List<GroupByColumn> GroupBy { get; set; }
            public List<HavingCondition> Having { get; set; }
            public List<OrderBy> OrderBy { get; set; }
        }

        public class GetSectionWise
        {
            public int Id { get; set; }
            public List<ReportParameterV5> Parameters { get; set; }
        }
        public class ViewConfig
        {
            public string ViewType { get; set; }
            public string ChartType { get; set; }
            public string Theme { get; set; }
            public string PrimaryColor { get; set; }
            public string SecondaryColor { get; set; }
            public string BackgroundColor { get; set; }
            public string TextColor { get; set; }
            public string XAxisColumn { get; set; }
            public string YAxisColumn { get; set; }
            public List<SummaryBox> SummaryBoxes { get; set; }
            public ExportOptions ExportOptions { get; set; }
        }

        public class SummaryBox
        {
            public string Title { get; set; }
            public string Column { get; set; }
            public string Aggregate { get; set; }
            public string Color { get; set; }
            public string Icon { get; set; }
        }

        public class ExportOptions
        {
            public bool AllowExcelExport { get; set; }
            public bool AllowPDFExport { get; set; }
            public bool AllowCSVExport { get; set; }
            public bool AllowCopyToClipboard { get; set; }
            public string DefaultExportFormat { get; set; }
            public string PDFOrientation { get; set; }
            public string ExcelSheetName { get; set; }
            public string CSVDelimiter { get; set; }
        }


        public class Parameter
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string DataType { get; set; }
            public string DefaultValue { get; set; }
            public bool IsRequired { get; set; }
            public string DropdownQuery { get; set; }
        }


        public class Columns
        {
            public string Table { get; set; }
            public string Column { get; set; }
            public string Alias { get; set; }
            public string Aggregate { get; set; }
            public bool IsExpression { get; set; }
            public string Expression { get; set; }
        }

        public class GroupByColumn
        {
            public string Table { get; set; }
            public string Column { get; set; }
        }

        public class HavingCondition
        {
            public string Column { get; set; }
            public string Operator { get; set; }
            public string Value { get; set; }
        }

        public class Join
        {
            public string FromTable { get; set; }
            public string ToTable { get; set; }
            public string Condition { get; set; }
            public string JoinType { get; set; }
        }

        public class WhereCondition
        {
            public string Column { get; set; }
            public string Operator { get; set; }
            public string Value { get; set; }
        }

        public class OrderBy
        {
            public string Column { get; set; }
            public string Direction { get; set; }
        }



        #endregion

        #region Dashboard config model Region
        public class DashboardViewModel
        {
            public int DashboardId { get; set; }
            public string Name { get; set; }
            public string Theme { get; set; }
            public List<DashboardSection> Sections { get; set; }
        }

        public class DashboardSection
        {
            public int Id { get; set; }
            public int ColumnWidth { get; set; }
            public string Title { get; set; }
            public List<string> Files { get; set; }
            public int? SavedReportId { get; set; }
            public string ReportConfigJson { get; set; }
        }

        #endregion



        #region KevalSir's model
        public class ReportBuilderViewModelV5
        {
            /// <summary>
            /// List of all tables available in the database
            /// </summary>
            public List<string> AllTables { get; set; } = new List<string>();

            /// <summary>
            /// List of all stored procedures available in the database
            /// </summary>
            public List<StoredProcedureInfo> AllStoredProcedures { get; set; } = new List<StoredProcedureInfo>();
        }

        /// <summary>
        /// Information about a stored procedure
        /// </summary>
        public class StoredProcedureInfo
        {
            public string Name { get; set; }
            public string Schema { get; set; }
            public List<StoredProcedureParameter> Parameters { get; set; } = new List<StoredProcedureParameter>();
        }

        /// <summary>
        /// Parameter information for stored procedures
        /// </summary>
        public class StoredProcedureParameter
        {
            public string Name { get; set; }
            public string DataType { get; set; }
            public int? MaxLength { get; set; }
            public bool IsOutput { get; set; }
            public bool HasDefaultValue { get; set; }
            public string DefaultValue { get; set; }
        }



        public class ColumnItemV5
        {
            public string Table { get; set; }
            public string Column { get; set; }
            public string Alias { get; set; }
            public string Aggregate { get; set; }
            public bool IsExpression { get; set; } = false;
            public string Expression { get; set; }
        }

        public class JoinConditionItemV5
        {
            public string MainColumn { get; set; }
            public string Operator { get; set; } = "=";
            public string JoinColumn { get; set; }
        }

        public class JoinItemV5
        {
            public string JoinType { get; set; } = "INNER";
            public string JoinTable { get; set; }
            public string MainColumn { get; set; }
            public string JoinColumn { get; set; }
            public List<JoinConditionItemV5> ExtraConditions { get; set; } = new List<JoinConditionItemV5>();
        }

        public class ReportParameterV5
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string DataType { get; set; }
            public string DefaultValue { get; set; }
            public bool IsRequired { get; set; }
            public string DropdownQuery { get; set; }
        }

        public class WhereItemV5
        {
            public string Table { get; set; }
            public string Column { get; set; }
            public string Operator { get; set; }
            public string ValueType { get; set; }
            public string Value { get; set; }
            public string ParameterName { get; set; }
            public string ParameterName2 { get; set; }
            public string Condition { get; set; } = "AND";
            public bool IsExpression { get; set; } = false;
        }

        public class GroupItemV5
        {
            public string Table { get; set; }
            public string Column { get; set; }
            public bool IsExpression { get; set; } = false;
            public string Expression { get; set; }
        }

        public class HavingItemV5
        {
            public string Aggregate { get; set; }
            public string Table { get; set; }
            public string Column { get; set; }
            public string Operator { get; set; }
            public string ValueType { get; set; } = "Static";
            public string Value { get; set; }
            public string ParameterName { get; set; }
        }

        public class OrderItemV5
        {
            public string Table { get; set; }
            public string Column { get; set; }
            public string Direction { get; set; } = "ASC";
        }



        public class ReportViewConfig5
        {
            public string ViewType { get; set; } = "Table";
            public string ChartType { get; set; }
            public string Theme { get; set; } = "Default";
            public string PrimaryColor { get; set; } = "#4CAF50";
            public string SecondaryColor { get; set; } = "#2196F3";
            public string BackgroundColor { get; set; } = "#FFFFFF";
            public string TextColor { get; set; } = "#333333";
            public List<SummaryBoxConfig5> SummaryBoxes { get; set; } = new List<SummaryBoxConfig5>();
            public string XAxisColumn { get; set; }
            public string YAxisColumn { get; set; }
            public List<string> SeriesColumns { get; set; } = new List<string>();
            public ExportOptionsConfig5 ExportOptions { get; set; } = new ExportOptionsConfig5();
        }

        public class SummaryBoxConfig5
        {
            public string Title { get; set; }
            public string Column { get; set; }
            public string Aggregate { get; set; }
            public string Icon { get; set; }
            public string Color { get; set; } = "#4CAF50";
        }

        public class ExportOptionsConfig5
        {
            public bool AllowExcelExport { get; set; } = true;
            public bool AllowPDFExport { get; set; } = true;
            public bool AllowCSVExport { get; set; } = true;
            public bool AllowCopyToClipboard { get; set; } = true;
            public string ExcelSheetName { get; set; } = "Report";
            public bool ExcelIncludeFilters { get; set; } = false;
            public string PDFOrientation { get; set; } = "Landscape";
            public string CSVDelimiter { get; set; } = ",";
            public string DefaultExportFormat { get; set; } = "Excel";
        }



        public class QueryRequestV5
        {
            /// <summary>
            /// Report source type: "QueryBuilder" or "StoredProcedure"
            /// </summary>
            public string SourceType { get; set; } = "QueryBuilder";

            /// <summary>
            /// Stored procedure name (only used when SourceType = "StoredProcedure")
            /// </summary>
            public string StoredProcedureName { get; set; }

            // Query Builder fields (only used when SourceType = "QueryBuilder")
            public string BaseTable { get; set; }
            public bool Distinct { get; set; }
            public int? Top { get; set; }
            public List<ColumnItemV5> Columns { get; set; } = new List<ColumnItemV5>();
            public List<JoinItemV5> Joins { get; set; } = new List<JoinItemV5>();
            public List<WhereItemV5> Where { get; set; } = new List<WhereItemV5>();
            public List<GroupItemV5> GroupBy { get; set; } = new List<GroupItemV5>();
            public List<HavingItemV5> Having { get; set; } = new List<HavingItemV5>();
            public List<OrderItemV5> OrderBy { get; set; } = new List<OrderItemV5>();

            // Common fields (used by both QueryBuilder and StoredProcedure)
            public List<ReportParameterV5> Parameters { get; set; } = new List<ReportParameterV5>();
            public ReportViewConfig5 ViewConfig { get; set; } = new ReportViewConfig5();
        }

        public class SavedReport5
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string JsonRequest { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public string CreatedBy { get; set; }
            public string Description { get; set; }
            public bool IsActive { get; set; }
        }



        public class ReportExecutionResponse5
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public List<string> Columns { get; set; } = new List<string>();
            public List<Dictionary<string, object>> Rows { get; set; } = new List<Dictionary<string, object>>();
            public ReportViewConfig5 ViewConfig { get; set; }
            public int TotalRecords { get; set; }
            public string ExecutionTime { get; set; }
        }

        public class ExportResponse5
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public byte[] FileData { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
        }
        #endregion

    }



    public class SendDashboardViewModel
    {
        public int DashboardId { get; set; }
        public string Name { get; set; }
        public string Theme { get; set; }
        public List<SendDashboardSection> Sections { get; set; }
    }

    public class SendDashboardSection
    {
        public int Id { get; set; }
        public int ColumnWidth { get; set; }
        public string Title { get; set; }
        public List<string> Files { get; set; }
        public int? SavedReportId { get; set; }
        public string ReportConfigJson { get; set; }
        public IEnumerable<dynamic> Report { get; set; }
    }

    public class DropdownList
    {
        public int reportId { get; set; }
        public List<dropdowns> dropdowns { get; set; }
    }
    public class dropdowns
    {
        public string paramName { get; set; }
        public string query { get; set; }
        public Dictionary<string, object> Data { get; set; }

    }

    public class sendDropdownres
    {
        public int statusCode { get; set; }
        public string message { get; set; }
        public Dictionary<string, object> Data { get; set; }

    }
    public class StateModel
    {
        public string StateName { get; set; }

    }

    public class GroupColumnJson
    {
        public string ReportName { get; set; }
        public List<ColumnGroup> columnGroups { get; set; }
    }


    public class ColumnGroup
    {
        public string GroupName { get; set; }
        public List<ReportColumn> Columns { get; set; }
        public string Color { get; set; }
        public string SubHeaderColor { get; set; }
    }

    public class ReportColumn
    {
        public string ActualName { get; set; }
        public string DisplayName { get; set; }
    }

    public class DynamicGrouping
    {
        public string ActualName { get; set; }
        public string DisplayName { get; set; }
        public string ReportName { get; set; }
        public string GroupName { get; set; }
    }


    public class ReportTrackingFilter
    {
        public int UploadId { get; set; }
        public string StateName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string SHMail { get; set; }
        public string AmMail { get; set; }
        public string TmMail { get; set; }
        public string billingTtl { get; set; }


    }

    public class BDRCModel
    {
        public List<BDRCList> Bdrcclist { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }

    }
    public class BDRCList
    {
        public int UploadId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string StateName { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string Status { get; set; }

        public int BatchId { get; set; }
        public int BillPlan_W1 { get; set; }
        public int BillPlan_W2 { get; set; }
        public int BillPlan_W3 { get; set; }
        public int BillPlan_W4 { get; set; }
        public int BillPlan_W5 { get; set; }

        public int DelPlan_W1 { get; set; }
        public int DelPlan_W2 { get; set; }
        public int DelPlan_W3 { get; set; }
        public int DelPlan_W4 { get; set; }
        public int DelPlan_W5 { get; set; }

        public int RetPlan_W1 { get; set; }
        public int RetPlan_W2 { get; set; }
        public int RetPlan_W3 { get; set; }
        public int RetPlan_W4 { get; set; }
        public int RetPlan_W5 { get; set; }

        public int CollPlan_W1 { get; set; }
        public int CollPlan_W2 { get; set; }
        public int CollPlan_W3 { get; set; }
        public int CollPlan_W4 { get; set; }
        public int CollPlan_W5 { get; set; }

        public int BGPlan_W1 { get; set; }
        public int BGPlan_W2 { get; set; }
        public int BGPlan_W3 { get; set; }
        public int BGPlan_W4 { get; set; }
        public int BGPlan_W5 { get; set; }
        public string IsEdit { get; set; }

    }

    public class ForecastModel
    {
        public List<ForecastList> forecastlist { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }

    }
    public class ForecastList
    {
        public int? UploadId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public string? StateName { get; set; }
        public int? BatchId { get; set; }

        public string? ModelCode { get; set; }
        public string? ModelName { get; set; }

        public int? W1_BillingPlan { get; set; }
        public int? W2_BillingPlan { get; set; }
        public int? W3_BillingPlan { get; set; }
        public int? W4_BillingPlan { get; set; }
        public int? W5_BillingPlan { get; set; }

        public int? Total_BillingPlan { get; set; }

        public string? IsEdit { get; set; }

    }

    public class PricePositionModel
    {
        public string stateName { get; set; }
        public string hpRange { get; set; }
        public string hp { get; set; }
        public string make { get; set; }
        public string bom { get; set; }
        public string avgVolPerMonth { get; set; }
        public string variantCode { get; set; }
        public string ndp { get; set; }
        public string freight { get; set; }
        public string accessories { get; set; }
        public string dlrMargin { get; set; }
        public string mop { get; set; }
        public string AccessoryName { get; set; }
        public string mopDate { get; set; }
        public IFormFile mopProof { get; set; }
        public IFormFile rcCopy { get; set; }
        public string driveType { get; set; }
        public string implementPrice { get; set; }
        public string rtoInsurance { get; set; }
        public string offerPrice { get; set; } = "0";

    }

    public class UpdatePricePosition
    {
        public string Id { get; set; }
        //public string stateName { get; set; }
        //public string hpRange { get; set; }
        //public string hp { get; set; }
        //public string make { get; set; }
        //public string bom { get; set; }
        public int avgVolPerMonth { get; set; }
        //public string variantCode { get; set; }
        public long ndp { get; set; }
        public long freight { get; set; }
        public long accessories { get; set; }
        public long dlrMargin { get; set; }
        public long mop { get; set; }
        //public string mopProof { get; set; }
        //public string rcCopy { get; set; }
    }


    public class UpdatePricePositionv1
    {
        public string Id { get; set; }
        //public string stateName { get; set; }
        //public string hpRange { get; set; }
        //public string hp { get; set; }
        //public string make { get; set; }
        //public string bom { get; set; }
        public int avgVolPerMonth { get; set; }
        //public string variantCode { get; set; }
        public long ndp { get; set; }
        public long freight { get; set; }
        public long accessories { get; set; }
        public long dlrMargin { get; set; }
        public long mop { get; set; }
        public string driveType { get; set; }
        public long implementPrice { get; set; }
        public long rtoInsurance { get; set; }
        public long offerPrice { get; set; }
        //public string mopProof { get; set; }
        //public string rcCopy { get; set; }
    }

    public class PricePositionApproval
    {
        public List<string> IDs { get; set; }
        public int isCommited { get; set; }
        public string Remark { get; set; }
    }

    public class RevisedBDRCRequest
    {
        public int month { get; set; }

        public int year { get; set; }

        public int week { get; set; }

        public string stateName { get; set; } = "All";
    }


    public class AdvanceMoreThan90Whatsapp
    {
        public string stateName { get; set; }
        public string dealerCode { get; set; }
        public string location { get; set; }
        public string customerName { get; set; }
        public string deliveryDate { get; set; }
        public string financeStatus { get; set; }
        public string stateHeadName { get; set; }
        public string stateHeadMobile { get; set; }

    }
}

