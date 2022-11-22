using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;

namespace UmbNav.Web.Filters;

internal class UmbNavManifestFilter : IManifestFilter
{
    public void Filter(List<PackageManifest> manifests)
    {
        manifests.Add(new PackageManifest
        {
            PackageName = "UmbNav",
            Scripts = new[]
            {
                "/App_Plugins/UmbNav/js/umbnav.settings.controller.js",
                "/App_Plugins/UmbNav/js/umbnav.controller.js",
                "/App_Plugins/UmbNav/js/umbnav.resource.js",
                "/App_Plugins/UmbNav/js/angular-ui-tree.js",
                "/App_Plugins/UmbNav/js/sortable.js"
            },
            Stylesheets = new[]
            {
                "/App_Plugins/UmbNav/css/editor.css"
            }
        });
    }
}