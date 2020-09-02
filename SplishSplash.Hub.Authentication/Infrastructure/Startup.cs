using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Kleinrechner.SplishSplash.Hub.Authentication.Abstractions;
using Kleinrechner.SplishSplash.Hub.Authentication.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Kleinrechner.SplishSplash.Hub.Authentication.Infrastructure
{
    public class Startup
    {
        #region Fields
        #endregion

        #region Ctor
        #endregion

        #region Methods

        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthenticationSettings>(configuration.GetSection(AuthenticationSettings.SectionName));
            services.AddTransient<Abstractions.IAuthenticationService, Services.AuthenticationService>();

            // configure basic authentication 
            services.AddAuthentication("BasicAuthentication")
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                    };
                    options.Events = new JwtBearerEvents()
                    {
                        OnTokenValidated = context =>
                        {
                            var authenticationService = context.HttpContext.RequestServices.GetRequiredService<Abstractions.IAuthenticationService>();
                            var nameClaim = context.Principal.FindFirst(ClaimTypes.NameIdentifier);
                            var hashClaim = context.Principal.FindFirst(ClaimTypes.Hash);
                            var loginUser = authenticationService.GetLoginUsers()
                                .FirstOrDefault(x => x.LoginName.ToLower() == nameClaim.Value.ToLower());

                            if (loginUser != null)
                            {
                                if (hashClaim.Value != loginUser.GetMD5Hash())
                                {
                                    context.Fail("User has been changed, please login again!");
                                }
                            }
                            else
                            {
                                context.Fail("User does not exist!");
                            }
                            return Task.CompletedTask;
                        }
                    };
                })
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        #endregion
    }
}
