using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Kleinrechner.SplishSplash.Hub.Authentication.Abstractions;
using Kleinrechner.SplishSplash.Hub.Authentication.Abstractions.Models;
using Kleinrechner.SplishSplash.Hub.Authentication.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Kleinrechner.SplishSplash.Hub.Authentication.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        #region Fields

        private IConfiguration _config;
        private IAuthenticationService _authenticationService;

        #endregion

        #region Ctor

        public LoginController(IConfiguration config, IAuthenticationService authenticationService)
        {
            _config = config;
            _authenticationService = authenticationService;
        }

        #endregion

        #region Methods

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            IActionResult response = Unauthorized();

            var loginUser = _authenticationService.GetLoginUsers().FirstOrDefault(x =>
                x.LoginName.ToLower() == loginModel.Username.ToLower() && x.PasswordMD5Hash == loginModel.Password.GetMD5Hash());

            if (loginUser != null)
            {
                var claimList = loginUser.GetClaims();
                var tokenString = GenerateJsonNWebToken(claimList);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string GenerateJsonNWebToken(List<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion
    }
}
