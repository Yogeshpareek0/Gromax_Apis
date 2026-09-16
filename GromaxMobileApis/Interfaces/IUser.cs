using GromaxMobileApis.Models;
using System.Threading.Tasks;

namespace GromaxMobileApis.Interfaces
{
    public interface IUser
    {
        string GetUserName();
        string GetPositionName();
        string GetName();
        string GetPlatform();
        string GetMobile();
        string GetDealerCode();
    }
}
