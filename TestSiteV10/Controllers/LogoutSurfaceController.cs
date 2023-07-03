using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Security;
using Umbraco.Cms.Web.Website.Controllers;

namespace TestSiteV10.Controllers
{
    public class LogoutSurfaceController : SurfaceController
    {
        private readonly IMemberManager _memberManager;
        private readonly IMemberSignInManager _memberSignInManager;

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (_memberManager.IsLoggedIn())
            {
                TempData.Clear();
                await _memberSignInManager.SignOutAsync();
                return RedirectToCurrentUmbracoPage();
            }

            return Redirect("/");
        }

        public LogoutSurfaceController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, IMemberManager memberManager, IMemberSignInManager memberSignInManager) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _memberManager = memberManager;
            _memberSignInManager = memberSignInManager;
        }
    }
}
