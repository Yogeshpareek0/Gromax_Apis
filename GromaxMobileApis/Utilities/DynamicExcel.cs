using Dapper;
using Dapper;
using ExcelDataReader;
using GromaxMobileApis.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GromaxMobileApis.Models.ReportMasterModel;
using static System.Net.WebRequestMethods;


namespace GromaxMobileApis.Utilities
{

    public class DynamicExcel
    {
        private readonly StagingService _stager;
        private string connectionString;
        private readonly IReportMaster _reportMaster;

        public DynamicExcel(StagingService stagingService, IConfiguration configurtion, IReportMaster Re)
        {
            _stager = stagingService;
            connectionString = configurtion.GetConnectionString("DefaultConnection");
            _reportMaster = Re;
        }
        private string Normalize(string s)
        {
            if (s == null) return "";
            s = s.ToLowerInvariant().Trim();
            s = Regex.Replace(s, @"[\s_\-\.]+", "");
            s = Regex.Replace(s, @"[^\w]", "");
            return s;
        }
        public (Dictionary<string, string> map, List<string> unmatched) AutoMap(DataTable excelTable, List<UploadColumnMapping> mappings)
        {
            var headers = excelTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            var normalizedHeaders = headers.ToDictionary(h => Normalize(h), h => h);

            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var usedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var m in mappings)
            {
                var candidate = headers.FirstOrDefault(h => string.Equals(h?.Trim(), m.ExcelColumn?.Trim(), StringComparison.OrdinalIgnoreCase));
                if (candidate != null)
                {
                    map[m.ExcelColumn] = candidate;
                    usedHeaders.Add(candidate);
                }
            }

            foreach (var m in mappings)
            {
                if (map.ContainsKey(m.ExcelColumn)) continue;
                var norm = Normalize(m.ExcelColumn);
                if (norm == "") continue;
                if (normalizedHeaders.TryGetValue(norm, out var hdr) && !usedHeaders.Contains(hdr))
                {
                    map[m.ExcelColumn] = hdr;
                    usedHeaders.Add(hdr);
                }
            }

            foreach (var m in mappings)
            {
                if (map.ContainsKey(m.ExcelColumn)) continue;
                var norm = Normalize(m.ExcelColumn);
                if (string.IsNullOrEmpty(norm)) continue;

                string bestHdr = null;
                int bestScore = -1;
                foreach (var h in headers.Except(usedHeaders))
                {
                    var hn = Normalize(h);
                    int score = 0;
                    if (hn.Contains(norm)) score += 10;
                    if (norm.Contains(hn)) score += 8;
                    int prefix = CommonPrefixLength(hn, norm);
                    score += prefix;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestHdr = h;
                    }
                }

                if (bestScore > 0 && bestHdr != null)
                {
                    map[m.ExcelColumn] = bestHdr;
                    usedHeaders.Add(bestHdr);
                }
            }

            var unmatched = headers.Where(h => !usedHeaders.Contains(h)).ToList();

            foreach (var m in mappings)
            {
                if (!map.ContainsKey(m.ExcelColumn)) map[m.ExcelColumn] = null;
            }

            return (map, unmatched);
        }

        private int CommonPrefixLength(string a, string b)
        {
            int i = 0;
            int n = Math.Min(a.Length, b.Length);
            while (i < n && a[i] == b[i]) i++;
            return i;
        }
        private DataTable ReadExcelToDataTable(Stream inputStream)
        {
            // Required for .NET Core / .NET 5 to support non-UTF encodings 
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var reader = ExcelReaderFactory.CreateReader(inputStream))
            {
                // Configure Excel-to-DataTable
                var conf = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true      // First row treated as column header
                    }
                };

                var ds = reader.AsDataSet(conf);

                // No sheet? Return empty DataTable
                if (ds.Tables.Count == 0)
                    return new DataTable();

                return ds.Tables[0];  // Return first sheet
            }
        }


        public UploadPreviewResponse Preview(int uploadId, IFormFile file, List<UploadColumnMapping> mappings, UploadMaster TargetTable)
        {

            DataTable excel;
            try
            {
                using (var s = file.OpenReadStream())
                {
                    excel = ReadExcelToDataTable(s);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to read Excel: " + ex.Message);
            }

            var (map, unmatched) = AutoMap(excel, mappings);

            string stagingTable;
            try
            {
                stagingTable = CreateStagingAndUpload(excel, mappings, map);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload to staging: " + ex.Message);
            }

            var (previewDt, total, valid, invalid) = GetPreview(stagingTable, 200);
            var Preview = ConvertDataTableToList(previewDt);
            UploadPreviewResponse _res = new UploadPreviewResponse()
            {
                StagingTable = stagingTable,
                Mappings = mappings,
                UploadId = uploadId,
                TargetTable = TargetTable.TargetTable,
                UploadName = TargetTable.UploadName,
                Total = total,
                Valid = valid,
                Invalid = invalid,
                Unmatched = unmatched,
                Preview = Preview

            };
            return _res;
            //Session["Upload_StagingTable"] = stagingTable;
            //Session["Upload_Mappings"] = mappings;
            //Session["Upload_TargetTable"] = TargetTable.TargetTable;
            //Session["Upload_Name"] = TargetTable.UploadName;
            //Session["Upload_ID"] = uploadId;

            //ViewBag.Total = total;
            //ViewBag.Valid = valid;
            //ViewBag.Invalid = invalid;
            //ViewBag.Unmatched = unmatched;

            //return previewDt;
        }
        public static List<Dictionary<string, object>> ConvertDataTableToList(DataTable dt)
        {
            var list = new List<Dictionary<string, object>>();

            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }
                list.Add(dict);
            }
            return list;
        }
        public string CreateStagingAndUpload(
            DataTable excelDt,
            List<UploadColumnMapping> mappings,
            Dictionary<string, string> excelMap)
        {
            var stagingName = _stager.BuildStagingTableName(
                mappings.FirstOrDefault()?.UploadID.ToString() ?? "upload"
            );

            _stager.DropStagingTableIfExists(stagingName);
            _stager.CreateStagingTable(stagingName, mappings);

            // Build staging DataTable structure
            var stagingDt = new DataTable();
            foreach (var m in mappings)
            {
                stagingDt.Columns.Add(m.TargetColumn, typeof(string));
            }

            // Copy rows from Excel DataTable to staging DataTable
            foreach (DataRow er in excelDt.Rows)
            {
                var nr = stagingDt.NewRow();

                foreach (var m in mappings)
                {
                    var mappedHeader = excelMap.ContainsKey(m.ExcelColumn)
                        ? excelMap[m.ExcelColumn]
                        : null;

                    if (!string.IsNullOrEmpty(mappedHeader) && excelDt.Columns.Contains(mappedHeader))
                    {
                        var val = er[mappedHeader];
                        nr[m.TargetColumn] = string.IsNullOrWhiteSpace(val?.ToString())
                            ? DBNull.Value
                            : val.ToString().Trim();
                    }
                    else
                    {
                        nr[m.TargetColumn] = DBNull.Value;
                    }
                }

                stagingDt.Rows.Add(nr);
            }

            // Bulk insert into staging table
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();

                using (var bulk = new SqlBulkCopy(con))
                {
                    bulk.DestinationTableName = stagingName;
                    bulk.BatchSize = 1000;
                    bulk.BulkCopyTimeout = 600;

                    foreach (DataColumn c in stagingDt.Columns)
                    {
                        bulk.ColumnMappings.Add(c.ColumnName, c.ColumnName);
                    }

                    bulk.WriteToServer(stagingDt);
                }
            }

            _stager.RunServerSideValidation(stagingName, mappings);

            return stagingName;
        }


        public (DataTable preview, int total, int valid, int invalid) GetPreview(
     string stagingTable,
     int top = 200)
        {
            // Validate table name to avoid SQL Injection
            if (!Regex.IsMatch(stagingTable, @"^[A-Za-z0-9_\[\]]+$"))
                throw new Exception("Invalid table name!");

            var preview = new DataTable();
            var summary = new DataTable();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 1️⃣ Get preview rows
                using (var cmd = new SqlCommand(
                    $"SELECT TOP({top}) *, __IsValid, __ErrorMsg FROM [{stagingTable}] ORDER BY __UploadedOn DESC",
                    conn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(preview);
                }

                // 2️⃣ Get summary
                using (var cmd = new SqlCommand(
                    $@"SELECT 
                    COUNT(*) AS Total, 
                    SUM(CASE WHEN __IsValid = 1 THEN 1 ELSE 0 END) AS Valid, 
                    SUM(CASE WHEN __IsValid = 0 THEN 1 ELSE 0 END) AS Invalid 
                FROM [{stagingTable}];",
                    conn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(summary);
                }
            }

            int total = Convert.ToInt32(summary.Rows[0]["Total"]);
            int valid = Convert.ToInt32(summary.Rows[0]["Valid"]);
            int invalid = Convert.ToInt32(summary.Rows[0]["Invalid"]);

            return (preview, total, valid, invalid);
        }


        public int MoveValidRowsToTarget(
            string stagingTable,
            string targetTable,
            List<UploadColumnMapping> mappings,
            int uploadId,
            string connectionString)
        {
            // safety validation
            if (!Regex.IsMatch(stagingTable, @"^[A-Za-z0-9_\[\]]+$")) throw new Exception("Invalid staging table name");
            if (!Regex.IsMatch(targetTable, @"^[A-Za-z0-9_\[\]]+$")) throw new Exception("Invalid target table name");

            var cols = string.Join(", ", mappings.Select(m => $"[{m.TargetColumn}]"));

            // 1️⃣ Insert valid rows
            var insertSql = $@"
        INSERT INTO [{targetTable}] ({cols})
        SELECT {cols}
        FROM [{stagingTable}]
        WHERE __IsValid = 1;
    ";

            ExecuteNonQuery(insertSql, connectionString);

            // 2️⃣ Count moved rows
            var countSql = $@"SELECT COUNT(*) FROM [{stagingTable}] WHERE __IsValid = 1;";

            int moved = ExecuteScalarInt(countSql, connectionString);

            // 3️⃣ Insert errors into log table
            var logSql = $@"
        INSERT INTO UploadStagingLog(UploadID, RowNumber, ExcelData, ErrorMessage)
        SELECT 
            {uploadId},
            ROW_NUMBER() OVER(ORDER BY (SELECT NULL)),
            (SELECT * FROM [{stagingTable}] FOR JSON AUTO),
            __ErrorMsg
        FROM [{stagingTable}]
        WHERE __IsValid = 0 
        AND (__ErrorMsg IS NOT NULL AND LTRIM(RTRIM(__ErrorMsg)) <> '');
    ";

            try
            {
                ExecuteNonQuery(logSql, connectionString);
            }
            catch
            {
                // ignore logging errors
            }

            return moved;
        }
        public void ExecuteNonQuery(string sql, string connectionString)
        {
            using var conn = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(sql, conn);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        public int ExecuteScalarInt(string sql, string connectionString)
        {
            using var conn = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(sql, conn);
            conn.Open();

            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }


        public void DropStaging(string stagingTable)
        {
            _stager.DropStagingTableIfExists(stagingTable);
        }

        public async Task<IEnumerable<dynamic>> ExecuteQueryForExport(QueryRequestV5 req, List<ReportParameterV5> Parameters)
        {
            //DataTable dtt = new DataTable();
            if (req.SourceType == "StoredProcedure")
            {
                // Execute stored procedure
                if (string.IsNullOrWhiteSpace(req.StoredProcedureName))
                    throw new Exception("Stored procedure name is required");

                var dtt = await _reportMaster.ExecuteStoredProcedure(req.StoredProcedureName, Parameters);
                return dtt;
            }

            // Execute dynamic SQL query (existing logic)
            var sql = BuildSqlWithPlaceholders(req);
            var dt = await _reportMaster.ExecuteSqlQuery(sql, Parameters);
            return dt;



        }
        private string BuildSqlWithPlaceholders(QueryRequestV5 req)
        {
            if (req == null) throw new ArgumentNullException(nameof(req));
            var selectParts = new List<string>();

            if (req.Columns != null && req.Columns.Any())
            {
                foreach (var c in req.Columns)
                {
                    if (c.IsExpression && !string.IsNullOrWhiteSpace(c.Expression))
                    {
                        var expr = c.Expression.Trim();
                        if (!string.IsNullOrWhiteSpace(c.Alias) && !expr.ToUpperInvariant().Contains(" AS "))
                            expr += $" AS [{c.Alias}]";
                        selectParts.Add(expr);
                    }
                    else
                    {
                        var colRef = $"{c.Table}.{c.Column}";
                        if (!string.IsNullOrWhiteSpace(c.Aggregate))
                            colRef = $"{c.Aggregate}({colRef})";
                        if (!string.IsNullOrWhiteSpace(c.Alias))
                            colRef += $" AS [{c.Alias}]";
                        selectParts.Add(colRef);
                    }
                }
            }
            else
            {
                selectParts.Add("*");
            }

            var distinctTxt = req.Distinct ? "DISTINCT " : "";
            var topTxt = (req.Top.HasValue && req.Top.Value > 0) ? $"TOP ({req.Top.Value}) " : "";

            var sb = new StringBuilder();
            sb.Append($"SELECT {distinctTxt}{topTxt}{string.Join(", ", selectParts)}");
            sb.AppendLine();
            sb.Append($"FROM {req.BaseTable}");

            // JOINS
            if (req.Joins != null)
            {
                foreach (var j in req.Joins)
                {
                    var onParts = new List<string>();

                    if (!string.IsNullOrWhiteSpace(j.MainColumn) && !string.IsNullOrWhiteSpace(j.JoinColumn))
                        onParts.Add($"{req.BaseTable}.{j.MainColumn} = {j.JoinTable}.{j.JoinColumn}");

                    if (j.ExtraConditions != null)
                    {
                        foreach (var c in j.ExtraConditions)
                        {
                            if (string.IsNullOrWhiteSpace(c.MainColumn) || string.IsNullOrWhiteSpace(c.JoinColumn)) continue;
                            onParts.Add($"{req.BaseTable}.{c.MainColumn} {c.Operator} {j.JoinTable}.{c.JoinColumn}");
                        }
                    }

                    if (!onParts.Any())
                        continue;

                    sb.AppendLine();
                    sb.Append($"{j.JoinType} JOIN {j.JoinTable} ON {string.Join(" AND ", onParts)}");
                }
            }

            // WHERE
            if (req.Where != null && req.Where.Any())
            {
                var whereParts = new List<string>();
                for (int i = 0; i < req.Where.Count; i++)
                {
                    var w = req.Where[i];
                    string part = "";

                    if (w.ValueType == "Static")
                    {
                        part = $"{w.Table}.{w.Column} {w.Operator} {FormatLiteralForSql(w.Value, w.Operator)}";
                    }
                    else if (w.ValueType == "Parameter")
                    {
                        if (w.Operator?.ToUpperInvariant() == "BETWEEN")
                        {
                            if (string.IsNullOrWhiteSpace(w.ParameterName) || string.IsNullOrWhiteSpace(w.ParameterName2))
                                throw new Exception("BETWEEN requires two parameters.");
                            part = $"{w.Table}.{w.Column} BETWEEN @{w.ParameterName} AND @{w.ParameterName2}";
                        }
                        else if (w.Operator?.ToUpperInvariant() == "IN")
                        {
                            part = $"{w.Table}.{w.Column} IN (@{w.ParameterName})";
                        }
                        else
                        {
                            //part = $"{w.Table}.{w.Column} {w.Operator} @{w.ParameterName}";
                            part = $"{w.Table}.{w.Column} {w.Operator} {w.ParameterName}";
                        }
                    }
                    else if (w.IsExpression)
                    {
                        part = $"({w.Value})";
                    }
                    else
                    {
                        part = $"{w.Table}.{w.Column} {w.Operator} {FormatLiteralForSql(w.Value, w.Operator)}";
                    }

                    if (i > 0)
                        part = $"{w.Condition} {part}";

                    whereParts.Add(part);
                }

                if (whereParts.Any())
                {
                    sb.AppendLine();
                    sb.Append("WHERE " + string.Join(" ", whereParts));
                }
            }

            // GROUP BY
            if (req.GroupBy != null && req.GroupBy.Any())
            {
                var groupParts = req.GroupBy.Select(g => g.IsExpression && !string.IsNullOrWhiteSpace(g.Expression) ? g.Expression : $"{g.Table}.{g.Column}");
                sb.AppendLine();
                sb.Append("GROUP BY " + string.Join(", ", groupParts));
            }

            // HAVING
            if (req.Having != null && req.Having.Any())
            {
                var havingParts = new List<string>();
                foreach (var h in req.Having)
                {
                    var left = $"{h.Aggregate}({h.Table}.{h.Column})";
                    if (h.ValueType == "Static")
                        havingParts.Add($"{left} {h.Operator} {h.Value}");
                    else
                        havingParts.Add($"{left} {h.Operator} @{h.ParameterName}");
                }
                if (havingParts.Any())
                {
                    sb.AppendLine();
                    sb.Append("HAVING " + string.Join(" AND ", havingParts));
                }
            }

            // ORDER BY
            if (req.OrderBy != null && req.OrderBy.Any())
            {
                var orderParts = req.OrderBy.Select(o => $"{o.Table}.{o.Column} {o.Direction}");
                sb.AppendLine();
                sb.Append("ORDER BY " + string.Join(", ", orderParts));
            }

            return sb.ToString();
        }

        private string FormatLiteralForSql(string raw, string op)
        {
            if (raw == null) return "NULL";
            raw = raw.Trim();
            if (raw.StartsWith("=")) return raw.Substring(1);
            if (!string.IsNullOrWhiteSpace(op) && op.ToUpperInvariant() == "IN") return raw;
            if (!string.IsNullOrWhiteSpace(op) && op.ToUpperInvariant() == "LIKE") return $"'{EscapeSql(raw)}'";
            double d;
            if (double.TryParse(raw, out d)) return raw;
            return $"'{EscapeSql(raw)}'";
        }
        private string EscapeSql(string v) => v?.Replace("'", "''");
    }


    public class StagingService
    {
        private string connectionString;
        public StagingService(IConfiguration configurtion)
        {
            connectionString = configurtion.GetConnectionString("DefaultConnection");
        }
        public string BuildStagingTableName(string uploadName)
        {
            var safe = MakeSafeName(uploadName);
            var suffix = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return $"stg_{safe}_{suffix}";
        }

        private string MakeSafeName(string s)
        {
            var sb = new StringBuilder();
            foreach (var c in s)
            {
                if (char.IsLetterOrDigit(c) || c == '_') sb.Append(c);
                else if (char.IsWhiteSpace(c)) sb.Append('_');
            }
            var res = sb.ToString();
            if (string.IsNullOrEmpty(res)) res = "upload";
            return res;
        }

        public void CreateStagingTable(string stagingTableName, List<UploadColumnMapping> mappings)
        {
            var sb = new StringBuilder();
            sb.Append($"CREATE TABLE [{stagingTableName}] (");

            foreach (var m in mappings)
            {
                var col = m.TargetColumn;
                sb.Append($"[{col}] NVARCHAR(MAX) NULL,");
            }

            sb.Append("[__IsValid] BIT NULL DEFAULT 0,");
            sb.Append("[__ErrorMsg] NVARCHAR(2000) NULL,");
            sb.Append("[__UploadedOn] DATETIME NULL DEFAULT GETDATE()");
            sb.Append(");");

            var sql = sb.ToString();

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public void DropStagingTableIfExists(string stagingTableName)
        {

            // safety validation (prevents SQL injection)
            if (!Regex.IsMatch(stagingTableName, @"^[A-Za-z0-9_\[\]]+$"))
                throw new Exception("Invalid table name");

            var sql = $@"
        IF OBJECT_ID(N'{stagingTableName}', N'U') IS NOT NULL
            DROP TABLE [{stagingTableName}];
    ";

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        //public void RunServerSideValidation(string stagingTableName, List<UploadColumnMapping> mappings)
        //{
        //    var errorParts = new List<string>();
        //    foreach (var m in mappings)
        //    {
        //        var col = $"[{m.TargetColumn}]";
        //        var safeExcelName = m.ExcelColumn.Replace("'", "''");

        //        if (m.IsMandatory)
        //        {
        //            errorParts.Add($@"CASE WHEN ({col} IS NULL OR LTRIM(RTRIM({col})) = '') THEN '{safeExcelName} required' ELSE '' END");
        //        }
        //        else
        //        {
        //            errorParts.Add("''");
        //        }

        //        var dt = (m.DataType ?? "").ToLowerInvariant();
        //        if (dt.StartsWith("int"))
        //        {
        //            errorParts.Add($@"CASE WHEN ({col} IS NOT NULL AND LTRIM(RTRIM({col})) <> '' AND TRY_CAST({col} AS bigint) IS NULL) THEN '{safeExcelName} must be integer' ELSE '' END");
        //        }
        //        else if (dt.StartsWith("decimal") || dt.StartsWith("numeric"))
        //        {
        //            errorParts.Add($@"CASE WHEN ({col} IS NOT NULL AND LTRIM(RTRIM({col})) <> '' AND TRY_CAST({col} AS decimal(38,10)) IS NULL) THEN '{safeExcelName} must be numeric' ELSE '' END");
        //        }
        //        else if (dt.StartsWith("datetime") || dt == "date")
        //        {
        //            errorParts.Add($@"CASE WHEN ({col} IS NOT NULL AND LTRIM(RTRIM({col})) <> '' AND TRY_CAST({col} AS datetime) IS NULL) THEN '{safeExcelName} must be datetime' ELSE '' END");
        //        }
        //        else if (dt == "bit")
        //        {
        //            errorParts.Add($@"CASE WHEN ({col} IS NOT NULL AND LTRIM(RTRIM({col})) <> '' AND TRY_CAST({col} AS bit) IS NULL) THEN '{safeExcelName} must be boolean' ELSE '' END");
        //        }
        //        else
        //        {
        //            errorParts.Add("''");
        //        }
        //    }

        //    var concat = string.Join(" + '; ' + ", errorParts);

        //    var sql = $@"
        //        UPDATE [{stagingTableName}]
        //        SET __ErrorMsg = (
        //            SELECT LTRIM(RTRIM(REPLACE(REPLACE({concat}, '; ;', ';'), ';;',';')))
        //        );

        //        UPDATE [{stagingTableName}]
        //        SET __IsValid = CASE WHEN (__ErrorMsg IS NULL OR LTRIM(RTRIM(__ErrorMsg)) = '') THEN 1 ELSE 0 END;
        //    ";

        //    SqlHelper.ExecuteNonQuery(sql);
        //}

        public void RunServerSideValidation(string stagingTableName, List<UploadColumnMapping> mappings)
        {
            if (string.IsNullOrEmpty(stagingTableName) || mappings == null || mappings.Count == 0)
                return;

            var validations = new List<string>();

            foreach (var m in mappings)
            {
                var col = $"[{m.TargetColumn}]";
                var safeExcelName = m.ExcelColumn.Replace("'", "''");
                var checks = new List<string>();

                // 1. Mandatory
                if (m.IsMandatory)
                    checks.Add($@"CASE WHEN ({col} IS NULL OR LTRIM(RTRIM({col})) = '') 
                          THEN '{safeExcelName} required' ELSE '' END");

                // Clean Excel value from hidden characters
                var cleanCol = $@"
            LTRIM(RTRIM(
                REPLACE(REPLACE(REPLACE({col}, CHAR(160), ''), CHAR(9), ''), CHAR(13), '')
            ))
        ";

                // 2. Datatype
                var dt = (m.DataType ?? "").ToLowerInvariant();

                if (dt.StartsWith("int"))
                    checks.Add($@"CASE WHEN ({col} IS NOT NULL AND LTRIM(RTRIM({col})) <> '' 
                          AND TRY_CAST({cleanCol} AS bigint) IS NULL)
                          THEN '{safeExcelName} must be integer' ELSE '' END");

                else if (dt.StartsWith("decimal") || dt.StartsWith("numeric"))
                    checks.Add($@"CASE WHEN ({col} IS NOT NULL AND LTRIM(RTRIM({col})) <> '' 
                          AND TRY_CAST({cleanCol} AS decimal(38,10)) IS NULL)
                          THEN '{safeExcelName} must be numeric' ELSE '' END");

                else if (dt.StartsWith("datetime") || dt == "date")
                    checks.Add($@"CASE WHEN ({col} IS NOT NULL AND LTRIM(RTRIM({col})) <> '' 
                          AND TRY_CAST({cleanCol} AS datetime) IS NULL)
                          THEN '{safeExcelName} must be datetime' ELSE '' END");

                else if (dt == "bit")
                    checks.Add($@"CASE WHEN ({col} IS NOT NULL AND LTRIM(RTRIM({col})) <> '' 
                          AND TRY_CAST({cleanCol} AS bit) IS NULL)
                          THEN '{safeExcelName} must be boolean' ELSE '' END");

                if (checks.Count > 0)
                    validations.Add(string.Join(" + '; ' + ", checks));
            }

            var concat = validations.Count > 0 ? string.Join(" + '; ' + ", validations) : "''";

            var sql = $@"
        UPDATE [{stagingTableName}]
        SET __ErrorMsg = NULLIF(
                LTRIM(RTRIM(
                    REPLACE(REPLACE(REPLACE({concat}, '; ;', ';'), ';;', ';'), ';', '')
                )),
            '');

        UPDATE [{stagingTableName}]
        SET __IsValid = CASE WHEN __ErrorMsg IS NULL THEN 1 ELSE 0 END;
    ";

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }





    }



    public class DapperParameterHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DapperParameterHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public DynamicParameters BuildParameters(
            List<ReportParameterV5> parametersList)
        {
            var parameters = new DynamicParameters();

            if (parametersList == null || parametersList.Count == 0)
                return parameters;
            var user = _httpContextAccessor.HttpContext.User;

            // 🔑 JWT claims
            var tokenUsername = user?.FindFirst("username")?.Value;
            var tokenLoginAs = user?.FindFirst("LoginPosition")?.Value;

            foreach (var p in parametersList)
            {
                object value = GetTypedValue(p);

                if (p.DisplayName?.ToLower() == "loginas")
                {
                    parameters.Add("@" + p.Name, tokenLoginAs);
                }
                else if (p.DisplayName?.ToLower() == "username")
                {
                    parameters.Add("@" + p.Name, tokenUsername);
                }
                else
                {
                    parameters.Add("@" + p.Name, value);
                }
            }

            return parameters;
        }

        private static object GetTypedValue(ReportParameterV5 p)
        {
            if (string.IsNullOrWhiteSpace(p.DefaultValue))
            {
                if (p.IsRequired)
                    throw new Exception($"Required parameter missing: {p.Name}");

                return null;   // ✅ FIX
            }

            return p.DataType?.ToLower() switch
            {
                "int" => int.Parse(p.DefaultValue),
                "decimal" => decimal.Parse(p.DefaultValue),
                "date" => DateTime.Parse(p.DefaultValue),
                "datetime" => DateTime.Parse(p.DefaultValue),
                "bit" => p.DefaultValue == "1" || p.DefaultValue.ToLower() == "true",
                _ => p.DefaultValue
            };
        }

    }


}
