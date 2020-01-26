﻿using Microsoft.Extensions.Options;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Cooking.WPF.Services
{
    /// <summary>
    /// Class for configuration updates.
    /// </summary>
    public class SettingsService
    {
        /// <summary>
        /// Write new app settings.
        /// </summary>
        /// <param name="appSettings">New version of settings.</param>
        public void UpdateAppSettings(AppSettings appSettings)
            => File.WriteAllText(Consts.AppSettingsFilename, JsonSerializer.Serialize(appSettings));
    }
}
