using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Kleinrechner.SplishSplash.Backend.Authentication.Abstractions;
using Kleinrechner.SplishSplash.Backend.Authentication.Extensions;
using Kleinrechner.SplishSplash.Hub.Extensions;
using Kleinrechner.SplishSplash.Hub.Models;
using Kleinrechner.SplishSplash.Hub.SettingsService.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.Swagger.Annotations;

namespace Kleinrechner.SplishSplash.Hub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class UserController : ControllerBase
    {
        #region Fields

        private readonly IOptions<AuthenticationSettings> _authenticationSettings;
        private readonly ISettingsService _settingsService;

        #endregion

        #region Ctor

        public UserController(IOptions<AuthenticationSettings> authenticationSettings, ISettingsService settingsService)
        {
            _authenticationSettings = authenticationSettings;
            _settingsService = settingsService;
        }

        #endregion

        #region Methods

        [HttpGet("Get")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<LoginUser>))]
        public IEnumerable<LoginUser> Get()
        {
            var loginUsers = _authenticationSettings.Value.Users.Select(x => x.WithoutPassword()).ToList();
            return loginUsers;
        }

        [HttpGet("{loginName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LoginUser))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IActionResult Get([FromRoute] string loginName)
        {
            var authenticationSettings = _authenticationSettings.Value;
            var loginUsers = authenticationSettings.Users;

            var loginUser = loginUsers.FirstOrDefault(x => x.LoginName.ToLower() == loginName.ToLower());
            if (loginUser != null)
            {
                return Ok(loginUser.WithoutPassword());
            }
            else
            {
                return NotFound(loginName);
            }
        }

        [HttpPost("create")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LoginUser))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IActionResult Create([FromBody] CreateLoginUser createLoginUser)
        {
            var authenticationSettings = _authenticationSettings.Value;
            var loginUsers = authenticationSettings.Users;

            var loginUser = loginUsers.FirstOrDefault(x => x.LoginName.ToLower() == createLoginUser.LoginName.ToLower());
            if (loginUser == null)
            {
                if (!ValidRole(createLoginUser.Role))
                {
                    return BadRequest("Invalid rolename");
                }

                loginUser = new LoginUser();
                loginUser.LoginName = createLoginUser.LoginName;
                loginUser.PasswordMD5Hash = createLoginUser.Password.GetMD5Hash();
                loginUser.Role = createLoginUser.Role;

                loginUsers.Add(loginUser);
                authenticationSettings.Users = loginUsers;
                _settingsService.Save(authenticationSettings);

                return Ok(loginUser.WithoutPassword());
            }
            else
            {
                return BadRequest("LoginName already exist");
            }
        }

        [HttpPut("{loginName}/password")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LoginUser))]
        [SwaggerResponse(HttpStatusCode.NotFound)]

        public IActionResult UpdatePassword([FromRoute] string loginName, [FromBody] string password)
        {
            var authenticationSettings = _authenticationSettings.Value;
            var loginUsers = authenticationSettings.Users;

            var loginUser = loginUsers.FirstOrDefault(x => x.LoginName.ToLower() == loginName.ToLower());
            if (loginUser != null)
            {
                var i = loginUsers.IndexOf(loginUser);
                loginUser.PasswordMD5Hash = password;

                loginUsers[i] = loginUser;
                authenticationSettings.Users = loginUsers;
                _settingsService.Save(authenticationSettings);

                return Ok(loginUser.WithoutPassword());
            }
            else
            {
                return NotFound(loginName);
            }
        }

        [HttpPut("{loginName}/role")]
        [SwaggerResponse(HttpStatusCode.OK, Type=typeof(LoginUser))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IActionResult UpdateRole([FromRoute] string loginName, [FromBody] string roleName)
        {
            var authenticationSettings = _authenticationSettings.Value;
            var loginUsers = authenticationSettings.Users;

            var loginUser = loginUsers.FirstOrDefault(x => x.LoginName.ToLower() == loginName.ToLower());
            if (loginUser != null)
            {
                if (!ValidRole(roleName))
                {
                    return BadRequest("Invalid rolename");
                }

                var i = loginUsers.IndexOf(loginUser);
                loginUser.Role = roleName;

                loginUsers[i] = loginUser;
                authenticationSettings.Users = loginUsers;
                _settingsService.Save(authenticationSettings);

                return Ok(loginUser.WithoutPassword());
            }
            else
            {
                return NotFound(loginName);
            }
        }

        [HttpDelete("{loginName}/delete")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IActionResult Delete([FromRoute] string loginName)
        {
            var authenticationSettings = _authenticationSettings.Value;
            var loginUsers = authenticationSettings.Users;

            var loginUser = loginUsers.FirstOrDefault(x => x.LoginName.ToLower() == loginName.ToLower());
            if (loginUser != null)
            {
                loginUsers.Remove(loginUser);

                authenticationSettings.Users = loginUsers;
                _settingsService.Save(authenticationSettings);

                return Ok();
            }
            else
            {
                return NotFound(loginName);
            }
        }

        private bool ValidRole(string roleName)
        {
            var validRoleNames = new List<string>()
            {
                "Administrator",
                "Backend",
                "Frontend"
            };

            return validRoleNames.Contains(roleName);
        }

        #endregion
    }
}
