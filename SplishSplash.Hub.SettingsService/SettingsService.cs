using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Kleinrechner.SplishSplash.Hub.Authentication.Abstractions;
using Kleinrechner.SplishSplash.Hub.SettingsService.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Kleinrechner.SplishSplash.Hub.SettingsService
{
    public class SettingsService : ISettingsService
    {
        #region Fields

        private readonly IWebHostEnvironment _webHostEnvironment;

        #endregion

        #region Ctor

        public SettingsService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #endregion

        #region Methods

        public void Save(AuthenticationSettings value)
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data", "AuthenticationSettings.json");

            var jsonString = System.IO.File.ReadAllText(filePath);
            var authenticationSettingsModel = JsonConvert.DeserializeObject<AuthenticationSettingsModel>(jsonString);
            authenticationSettingsModel.AuthenticationSettings = value;

            //serialize the new updated object to a string
            var toWrite = JsonConvert.SerializeObject(authenticationSettingsModel, Formatting.Indented);

            //overwrite the file and it wil contain the new data
            System.IO.File.WriteAllText(filePath, toWrite);
        }

        #endregion
    }
}
