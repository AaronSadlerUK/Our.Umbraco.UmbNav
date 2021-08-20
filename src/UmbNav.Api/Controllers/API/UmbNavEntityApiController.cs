using System.Collections.Generic;
using System.Linq;
using Constants = UmbNav.Core.Constants;
#if NETCOREAPP
using Umbraco.Cms.Core;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;
#else

using System.Net;
using System.Net.Http;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedCache;
#endif

namespace UmbNav.Api.Controllers.API
{
    [PluginController(Constants.PackageName)]
    public class UmbNavEntityApiController : UmbracoAuthorizedJsonController
    {
        private readonly IContentService _contentService;
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

        public UmbNavEntityApiController(IContentService contentService, IPublishedSnapshotAccessor publishedSnapshotAccessor)
        {
            _contentService = contentService;
            _publishedSnapshotAccessor = publishedSnapshotAccessor;
        }

#if NETCOREAPP
        public IActionResult GetById(string id, string culture = null)
#else
        public HttpResponseMessage GetById(string id, string culture = null)
#endif
        {
            var udiList = new List<Udi>();
#if NETCOREAPP
            var udi = UdiParser.Parse(id);
#else
        var udi = Udi.Parse(id);
#endif
            udiList.Add(udi);
            var entity = _contentService.GetByIds(udiList).FirstOrDefault();

            if (entity != null)
            {
                string entityName = entity.Name;
                string entityUrl = "#";

                if (entity.Published)
                {
#if NETCOREAPP
                    if (_publishedSnapshotAccessor.TryGetPublishedSnapshot(out var publishedSnapshot))
                    {
                        var publishedEntity = publishedSnapshot.Content.GetById(entity.Key);

                        if (publishedEntity != null)
                        {
                            entityName = publishedEntity.Name(culture);
                            entityUrl = publishedEntity.Url(culture);
                        }
                    }
#else
                    var publishedEntity = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(entity.Key);

                    if (publishedEntity != null)
                    {
                        entityName = publishedEntity.Name(culture);
                        entityUrl = publishedEntity.Url(culture);
                    }
#endif
                }

                var menuItem = new
                {
                    id = entity.Id,
                    udi = entity.GetUdi(),
                    key = entity.Key,
                    name = entityName,
                    icon = entity.ContentType.Icon,
                    url = entityUrl,
                    published = entity.Published,
                    naviHide = entity.HasProperty("umbracoNaviHide") && entity.GetValue<bool>("umbracoNaviHide") ||
                               entity.HasProperty("umbracoNavihide") && entity.GetValue<bool>("umbracoNavihide"),
                    culture = culture
                };

#if NETCOREAPP
                return Ok(menuItem);
#else
                return Request.CreateResponse(HttpStatusCode.OK, menuItem);
#endif
            }

            return null;
        }
    }
}