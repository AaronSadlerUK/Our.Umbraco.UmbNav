using System;
using System.Configuration;

namespace UmbNav.Core
{
    internal class AppSettingsManager
    {
        internal static bool GetDisableUmbracoCloudSync()
        {
            return ConfigurationManager.AppSettings[UmbNavConstants.PackageName + ".DisableUmbracoCloudSync"] != null
                   && Convert.ToBoolean(ConfigurationManager.AppSettings[UmbNavConstants.PackageName + ".DisableUmbracoCloudSync"]);
        }

        internal static bool GetDisableUmbracoCloudDependencySync()
        {
            return ConfigurationManager.AppSettings[UmbNavConstants.PackageName + ".DisableUmbracoCloudDependencySync"] != null
                   && Convert.ToBoolean(ConfigurationManager.AppSettings[UmbNavConstants.PackageName + ".DisableUmbracoCloudDependencySync"]);
        }
    }
}
