using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Kleinrechner.SplishSplash.Hub.Hubs;
using Kleinrechner.SplishSplash.Hub.Validator;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;

namespace Kleinrechner.SplishSplash.Hub
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                // configure SwaggerDoc and others

                // add Basic Authentication
                var basicSecurityScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    Reference = new OpenApiReference { Id = "BasicAuth", Type = ReferenceType.SecurityScheme }
                };
                c.AddSecurityDefinition(basicSecurityScheme.Reference.Id, basicSecurityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {basicSecurityScheme, new string[] { }}
                });
            });

            services.AddCors();

            services.AddControllers()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UpdateLoginUserValidator>());

            services.AddSignalR();
            //.AddMessagePackProtocol();

            Authentication.Infrastructure.Startup.ConfigureServices(services, Configuration);

            SettingsService.Infrastructure.Startup.ConfigureServices(services, Configuration);
            Hubs.Infrastructure.Startup.ConfigureServices(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SplishSplash Hub API V1");
            });

            app.UseRouting();

            Authentication.Infrastructure.Startup.Configure(app, env);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync($"Welcome to SplishSplash.Hub{Environment.NewLine}" +
                                                      $"Assembly {this.GetType().Assembly.GetName().Name}{Environment.NewLine}" +
                                                      $"Version {this.GetType().Assembly.GetName().Version}{Environment.NewLine}" +
                                                      $".NET Core {Environment.Version}{Environment.NewLine}" +
                                                      $"Environment.OSVersion: {Environment.OSVersion}{Environment.NewLine}" +
                                                      $"Environment.Is64BitOperatingSystem: {Environment.Is64BitOperatingSystem}{Environment.NewLine}" +
                                                      $"Environment.Is64BitProcess: {Environment.Is64BitProcess}", Encoding.UTF8);
                });
                endpoints.MapHub<SplishSplashHub>("/splishsplashhub");
            });
        }
    }
}
