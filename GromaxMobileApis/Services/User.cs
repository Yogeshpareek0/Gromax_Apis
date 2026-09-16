using GromaxMobileApis.Interfaces;
using GromaxMobileApis.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GromaxMobileApis.Services
{
    public class User : IUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public User(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetDealerCode()
        {
            return _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == "dealercode")?.Value;
        }

        public string GetMobile()
        {
            return _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == "mobilenumber")?.Value;
        }

        public string GetName()
        {
            return _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x=>x.Type== "LoginName")?.Value;
        }

        public string GetPlatform()
        {
            return _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == "PlatformType")?.Value;
        }

        public string GetPositionName()
        {
            return _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == "LoginPosition")?.Value;

        }

        public string GetUserName()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}
