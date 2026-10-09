using Dapper;
using GromaxMobileApis.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static GromaxMobileApis.Models.CircularModel;

namespace GromaxMobileApis.Services
{
    public class CircularService : ICircularService
    {
        private readonly IDbConnection _db;      // CustomerService wala hi type
        private readonly IUser _user;     // CustomerService wala hi type

        public CircularService(IDbConnection db, IUser user)
        {
            _db = db;
            _user = user;
        }
        //Done
        public async Task<IEnumerable<dynamic>> getCircularsListdb(CircularListRequest m)
        {
            try
            {
                var param = new
                {
                    m.CircularName,
                    m.FinancialYear,
                    m.CircularType,
                    m.RowSkip,
                    username = _user.GetUserName(),
                    positionName = _user.GetPositionName()
                };
                return await _db.QueryAsync("usp_GetCircularsList", param, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }


        //Done
        public async Task<int> addCirculardb(AddCircularModel m)
        {
            try
            {
                var param = new
                {
                    m.CircularName,
                    m.CircularDate,
                    m.Description,
                    m.FinancialYear,
                    m.CircularType,
                    FilesJson = JsonConvert.SerializeObject(m.FileUrls),
                    username = _user.GetUserName(),
                    positionName = _user.GetPositionName()
                };
                return await _db.ExecuteScalarAsync<int>("usp_AddCircular", param, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        //Done
        public async Task<object> getCircularDetaildb(string circularName)
        {
            try
            {
                var row = await _db.QueryFirstOrDefaultAsync(
                    "usp_GetCircularDetail",
                    new { CircularName = circularName, username = _user.GetUserName(), positionName = _user.GetPositionName() },
                    commandType: CommandType.StoredProcedure);

                if (row == null) return null;

                var files = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(
                    row.FilesJson?.ToString() ?? "[]") ?? new List<Dictionary<string, object>>();

                return new
                {
                    Name = row.Name,
                    Description = row.Description,
                    CircularType = row.CircularType,
                    CircularDate = row.CircularDate,
                    FinancialYear = row.FinancialYear,
                    Files = files
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        //Done
        public async Task<int> updateCirculardb(AddCircularModel m)
        {
            try
            {
                var param = new
                {
                    m.CircularName,
                    m.NewCircularName,
                    m.CircularType,
                    m.CircularDate,
                    m.Description,
                    m.FinancialYear,
                    FilesJson = JsonConvert.SerializeObject(m.FileUrls),
                    username = _user.GetUserName(),
                    positionName = _user.GetPositionName()
                };
                return await _db.ExecuteScalarAsync<int>("usp_UpdateCircular", param, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        //still not done for reference clplcomunication_db me 
        //UpdateCircularSentStatus  - sp
        //GetMsgSentReport - sp
        public async Task<int> updateCircularSentStatusdb(UpdateCircularSentStatusRequest m)
        {
            try
            {
                var param = new
                {
                    m.CircularId,
                    m.Status,
                    m.Type,
                    username = _user.GetUserName()
                };
                return await _db.ExecuteScalarAsync<int>("USP_UpdateCircularSentStatus", param, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<dynamic>> getMsgSentReportdb(GetMsgSentReportRequest m)
        {
            try
            {
                var param = new
                {
                    m.CircularId,
                    m.Type,
                    m.Download,
                    m.RowSkip
                };
                return await _db.QueryAsync("USP_GetMsgSentReport", param, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<CircularDetailsAppResponse>> getCircularDetailAppDb(GetCircularDetailAppRequest m)
        {
            try
            {
                var param = new
                {
                    FY = m.FY,
                    CircularType = m.CircularType,
                    username = _user.GetUserName(),
                    positionName = _user.GetPositionName()
                };

                var row = await _db.QueryAsync(
                    "usp_GetCricularDetails_app",
                    param: param,
                    commandType: CommandType.StoredProcedure);

                var result = row.Select((x, index) =>
                {
                    string[] pdfNames = string.IsNullOrEmpty((string)x.PdfName) ? Array.Empty<string>() : ((string)x.PdfName).Split(',');

                    string[] pdfUrls = string.IsNullOrEmpty((string)x.PdfUrl)
                        ? Array.Empty<string>()
                        : ((string)x.PdfUrl).Split(',');

                    var pdfList = pdfNames
                        .Select((pdfName, i) => new CircularPdfApp
                        {
                            PdfName = pdfName.Trim(),
                            PdfUrl = i < pdfUrls.Length ? pdfUrls[i].Trim() : ""
                        })
                        .ToList();

                    return new CircularDetailsAppResponse
                    {
                        Id = index + 1,
                        FY = x.FY,
                        CircularType = x.CircularType,
                        Name = x.Name,
                        Description = x.Description,
                        CircularDate = DateTime.Parse(x.CircularDate.ToString()),
                        PdfUrls = pdfList
                    };
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CircularDropdownResponse> dropDownListDb()
        {
            try
            {
                using var multi = await _db.QueryMultipleAsync(
                    "usp_GetCircularDropDownList",
                    commandType: CommandType.StoredProcedure
                );

                var FyYear = (await multi.ReadAsync<string>()).ToList();
                var CircularType = (await multi.ReadAsync<string>()).ToList();

                var m = new CircularDropdownResponse
                {
                    FyYear = FyYear,
                    CircularType = CircularType
                };
                return m;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
