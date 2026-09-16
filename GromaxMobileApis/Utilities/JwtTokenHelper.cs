using GromaxMobileApis.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GromaxMobileApis.Utilities
{
    public static class JwtTokenHelper
    {
        public static string GenerateJwtToken(string MobileNo, string DealerCode, string UserName, string position, IConfiguration _configuration, out string jti, out DateTime expiryTime, string PlatformType, string Name)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            jti = Guid.NewGuid().ToString();
            expiryTime = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"]));
            //expiryTime = DateTime.UtcNow.AddDays(Convert.ToDouble(jwtSettings["ExpireDays"]));
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: new[] { new Claim(ClaimTypes.Name, UserName), new Claim("mobilenumber", MobileNo),
                    new Claim("dealercode", DealerCode), new Claim("id", jti), new Claim("LoginPosition", position),
                    new Claim("username", UserName), new Claim("PlatformType", PlatformType), new Claim("LoginName", Name),
                    new Claim(ClaimTypes.Role, position) },

                expires: expiryTime,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
