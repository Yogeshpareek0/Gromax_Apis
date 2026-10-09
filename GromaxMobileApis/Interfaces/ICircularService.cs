using System.Collections.Generic;
using System.Threading.Tasks;
using static GromaxMobileApis.Models.CircularModel;

namespace GromaxMobileApis.Interfaces
{
    public interface ICircularService
    {
        Task<IEnumerable<dynamic>> getCircularsListdb(CircularListRequest m);
        Task<int> addCirculardb(AddCircularModel m);
        Task<object> getCircularDetaildb(string circularName);
        Task<int> updateCirculardb(AddCircularModel m);
        Task<int> updateCircularSentStatusdb(UpdateCircularSentStatusRequest m);
        Task<IEnumerable<dynamic>> getMsgSentReportdb(GetMsgSentReportRequest m);
        Task<IEnumerable<CircularDetailsAppResponse>> getCircularDetailAppDb(GetCircularDetailAppRequest m);
        Task<CircularDropdownResponse> dropDownListDb();
    }
}
