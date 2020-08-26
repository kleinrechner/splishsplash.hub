using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Kleinrechner.SplishSplash.Hub.Authentication.Abstractions;
using Kleinrechner.SplishSplash.Hub.Authentication.Abstractions.Models;
using Kleinrechner.SplishSplash.Hub.Authentication.Extensions;
using Kleinrechner.SplishSplash.Hub.Models;
using Kleinrechner.SplishSplash.Hub.SettingsService.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Kleinrechner.SplishSplash.Hub.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(Roles = nameof(LoginUserRoles.Administrator))]
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

        /// <response code="200">Successful operation</response>
        [HttpGet("Get")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LoginUser>))]
        public IEnumerable<LoginUser> Get()
        {
            var loginUsers = _authenticationSettings.Value.Users.Select(x => x.WithoutPassword()).ToList();
            return loginUsers;
        }

        /// <param name="loginName">Name of the login user</param>
        /// <response code="200">Successful operation</response>
        /// <response code="404">User could not be found</response>
        [HttpGet("{loginName}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginUser))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get([FromRoute] string loginName)
        {
            if (!string.IsNullOrWhiteSpace(loginName))
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
            else
            {
                return BadRequest("LoginName not set");
            }
        }

        /// <param name="loginName">Name of the login user</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">User already exists or invalid role</response>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginUser))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] CreateLoginUser createLoginUser)
        {
            var authenticationSettings = _authenticationSettings.Value;
            var loginUsers = authenticationSettings.Users;

            var loginUser = new LoginUser();
            loginUser.DisplayName = createLoginUser.DisplayName;
            loginUser.LoginName = createLoginUser.LoginName;
            loginUser.PasswordMD5Hash = createLoginUser.Password.GetMD5Hash();
            loginUser.Role = createLoginUser.Role;

            loginUsers.Add(loginUser);
            authenticationSettings.Users = loginUsers;
            _settingsService.Save(authenticationSettings);

            return Ok(loginUser.WithoutPassword());
        }

        /// <param name="loginName">Name of the login user</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid role</response>
        /// <response code="404">User could not be found</response>
        [HttpPut("{loginName}/update")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginUser))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Update([FromRoute] string loginName, [FromBody] UpdateLoginUser updateLoginUser)
        {
            if (!string.IsNullOrWhiteSpace(loginName))
            {
                var authenticationSettings = _authenticationSettings.Value;
            
                var loginUsers = authenticationSettings.Users;
                var loginUser = loginUsers.FirstOrDefault(x => x.LoginName.ToLower() == loginName.ToLower());
                if (loginUser != null)
                {
                    loginUser.Role = updateLoginUser.Role;
                    loginUser.DisplayName = updateLoginUser.DisplayName;

                    _settingsService.Save(authenticationSettings);

                    return Ok(loginUser.WithoutPassword());
                }
                else
                {
                    return NotFound(loginName);
                }
            }
            else
            {
                return BadRequest("LoginName not set");
            }
        }

        /// <param name="loginName">Name of the login user</param>
        /// <response code="200">Successful operation</response>
        /// <response code="404">User could not be found</response>
        [HttpPut("{loginName}/password")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginUser))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult UpdatePassword([FromRoute] string loginName, [FromBody] string password)
        {
            if (!string.IsNullOrWhiteSpace(loginName))
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
            else
            {
                return BadRequest("LoginName not set");
            }
        }

        /// <param name="loginName">Name of the login user</param>
        /// <response code="200">Successful operation</response>
        /// <response code="404">User could not be found</response>
        [HttpDelete("{loginName}/delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete([FromRoute] string loginName)
        {
            if (!string.IsNullOrWhiteSpace(loginName))
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
            else
            {
                return BadRequest("LoginName not set");
            }
        }

        #endregion
    }
}
