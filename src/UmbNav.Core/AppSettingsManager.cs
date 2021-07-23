using System;
using System.Configuration;

namespace UmbNav.Core
{
    internal class AppSettingsManager
    {
        internal static bool GetDisableUmbracoCloudSync()
        {
            return ConfigurationManager.AppSettings[Constants.PackageName + ".DisableUmbracoCloudSync"] != null
                   && Convert.ToBoolean(ConfigurationManager.AppSettings[Constants.PackageName + ".DisableUmbracoCloudSync"]);
        }

        internal static bool GetDisableUmbracoCloudDependencySync()
        {
            return ConfigurationManager.AppSettings[Constants.PackageName + ".DisableUmbracoCloudDependencySync"] != null
                   && Convert.ToBoolean(ConfigurationManager.AppSettings[Constants.PackageName + ".DisableUmbracoCloudDependencySync"]);
        }
    }
}
