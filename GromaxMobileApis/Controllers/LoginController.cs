using GromaxMobileApis.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GromaxMobileApis.Utilities;
using System.Threading.Tasks;
using System.Collections.Generic;
using GromaxMobileApis.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace GromaxMobileApis.Controllers
{

    // [Route("Api/[Controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IDatabaseService _db;
        private readonly IDatabaseServicesweb _webdb;
        private readonly IConfiguration _configuration;
        private readonly ResponseClass _r;


        public LoginController(IDatabaseService db, IConfiguration configuration, ResponseClass r, IDatabaseServicesweb webdb)
        {
            _db = db;
            _webdb = webdb;
            _configuration = configuration;
            _r = r;
        }
        #region Mobileapp

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.LogIn.Login)]
        public async Task<IActionResult> Index(Loginmaster model)
        {
            try
            {
                string IsExists = await _db.LoginUser(model.MobileNo);
                if (IsExists == "Successfully")
                {
                    List<Loginmaster> UserRow = await _db.GetUserData(model);
                    string Dealercode = UserRow.Select(x => x.DealerCode).FirstOrDefault();
                    string username = UserRow.Select(x => x.UserName).FirstOrDefault();
                    string position = UserRow.Select(x => x.PossitionId).FirstOrDefault();
                    string Name = UserRow.Select(x => x.Name).FirstOrDefault();
                    //string username = UserRow.Select(x => x.UserName).FirstOrDefault();
                    var token = Utilities.JwtTokenHelper.GenerateJwtToken(model.MobileNo, Dealercode, username, position, _configuration, out string jti, out DateTime expiryTime, "app", Name);
                    //string id = User.FindFirst("id")?.Value; 
                    await _db.InsertToken(model.MobileNo, jti);
                    return Ok(new { UserRow, token });
                }
                else if (IsExists == "User Already Login")
                {
                    return Ok(IsExists);
                }
                return Unauthorized("Invalid credentials");


            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.LogIn.Loginv1)]
        public async Task<IActionResult> Indexv1(Loginmaster model)
        {
            try
            {
                string IsExists = await _db.LoginUser(model.MobileNo);
                if (IsExists == "Successfully")
                {
                    List<Loginmaster> UserRow = await _db.GetUserDatav1(model);
                    string Dealercode = UserRow.Select(x => x.DealerCode).FirstOrDefault();
                    string username = UserRow.Select(x => x.UserName).FirstOrDefault();
                    string position = UserRow.Select(x => x.PossitionId).FirstOrDefault();
                    string Name = UserRow.Select(x => x.Name).FirstOrDefault();

                    //string username = UserRow.Select(x => x.UserName).FirstOrDefault();
                    var token = Utilities.JwtTokenHelper.GenerateJwtToken(model.MobileNo, Dealercode, username, position, _configuration, out string jti, out DateTime expiryTime, "app", Name);
                    //string id = User.FindFirst("id")?.Value; 
                    await _db.InsertToken(model.MobileNo, jti);
                    return Ok(new { UserRow, token });
                }
                else if (IsExists == "User Already Login")
                {
                    return Ok(IsExists);
                }
                return Unauthorized("Invalid credentials");


            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [Authorize]
        [HttpPost]
        [Route(GromaxMobileApis.Utilities.ApiRoutes.LogIn.Logout)]
        public async Task<IActionResult> UserLogout(string MobileNo)
        {
            try
            {
                bool result = await _db.Logout(MobileNo);
                var result1 =await _db.BlankFCMToken(MobileNo);
                if (result)
                {
                    return Ok("logout Successfully");
                }

                else
                    return Ok("Somthing Wrong");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        #endregion





        #region WebApplication

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.LogIn.Login)]

        public async Task<IActionResult> LoginWeb(Loginmasterdto model)
        {
            try
            {
                string IsExists = await _webdb.LoginUser(model.MobileNo);
                if (IsExists == "Successfully")
                {
                    GetLoginmasterweb UserRow = await _webdb.GetUserData(model);
                    //string Dealercode = UserRow.Select(x => x.DealerCode).FirstOrDefault();
                    //string username = UserRow.Select(x => x.UserName).FirstOrDefault();

                    string Dealercode = UserRow.DealerCode;
                    string username = UserRow.UserName;
                    string position = UserRow.PossitionId;
                    string Name = UserRow.Name;

                    var token = Utilities.JwtTokenHelper.GenerateJwtToken(model.MobileNo, Dealercode, username, position, _configuration, out string jti, out DateTime expirytime, "web",Name);
                    UserRow.Token = token;
                    UserRow.Expiry = expirytime;
                    //string id = User.FindFirst("id")?.Value; 
                    await _db.InsertToken(model.MobileNo, jti);

                    return Ok(new { Message = _r.success, Data = UserRow });
                }
                else if (IsExists == "User Already Login")
                {
                    return Ok(new { message = IsExists, Data = "" });
                }

                return NotFound(new { message = "Invalid mobile no", Data = "" });




            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.LogIn.Loginv1)]

        public async Task<IActionResult> LoginWebv1(Loginmasterdto model)
        {
            try
            {
                string IsExists = await _webdb.LoginUserv1(model.MobileNo, model.Password);
                if (IsExists == "Successfully")
                {
                    GetLoginmasterweb UserRow = await _webdb.GetUserData(model);
                    //string Dealercode = UserRow.Select(x => x.DealerCode).FirstOrDefault();
                    //string username = UserRow.Select(x => x.UserName).FirstOrDefault();

                    string Dealercode = UserRow.DealerCode;
                    string username = UserRow.UserName;
                    string position = UserRow.PossitionId;
                    string Name = UserRow.Name;
                    var token = Utilities.JwtTokenHelper.GenerateJwtToken(model.MobileNo, Dealercode, username, position, _configuration, out string jti, out DateTime expirytime,"web", Name);
                    UserRow.Token = token;
                    UserRow.Expiry = expirytime;
                    //string id = User.FindFirst("id")?.Value; 
                    await _db.InsertToken(model.MobileNo, jti);

                    return Ok(new { Message = _r.success, Data = UserRow });
                }
                else if (IsExists == "User Already Login")
                {
                    return Ok(new { message = IsExists, Data = "" });
                }

                return NotFound(new { message = "Invalid", Data = "" });




            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        #endregion


        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.LogIn.UserExists)]
        public async Task<IActionResult> UserExists([FromBody] Loginmasterdto model)
        {
            try
            {
                var result = await _webdb.ExistsUserdb(model.MobileNo);
                if (result > 0)
                    return Ok(ApiResponse<string>.Success("Exists"));
                else
                    return Ok(ApiResponse<string>.Success("Not Exists"));

            }
            catch (Exception ex) { return Ok(ApiResponse<string>.Fail(ex.Message)); }

        }

        [HttpPost]
        [Route(GromaxMobileApis.Utilities.WebApiRoutes.LogIn.UpdatePassword)]
        public async Task<IActionResult> UpdatePassword([FromBody] Loginmasterdto model)
        {
            try
            {
                var result = await _webdb.UpdatePassworddb(model.MobileNo, model.Password);
                if (result > 0)
                    return Ok(ApiResponse<string>.Success(default));
                else
                    return Ok(ApiResponse<string>.Fail("Failed"));

            }
            catch (Exception ex) { return Ok(ApiResponse<string>.Fail(ex.Message)); }

        }


    }
}
