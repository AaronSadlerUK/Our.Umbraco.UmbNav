using System.Collections.Generic;
using System.Linq;
using UmbNavConstants = UmbNav.Core.UmbNavConstants;
using Umbraco.Cms.Core;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;

namespace UmbNav.Api.Controllers.API
{
    [PluginController(UmbNavConstants.PackageName)]
    public class UmbNavEntityApiController : UmbracoAuthorizedJsonController
    {
        private readonly IContentService _contentService;
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

        public UmbNavEntityApiController(IContentService contentService, IPublishedSnapshotAccessor publishedSnapshotAccessor)
        {
            _contentService = contentService;
            _publishedSnapshotAccessor = publishedSnapshotAccessor;
        }

        public IActionResult GetById(string id, string culture = null)
        {
            var udiList = new List<Udi>();
            var udi = UdiParser.Parse(id);
            udiList.Add(udi);
            var entity = _contentService.GetByIds(udiList).FirstOrDefault();

            if (entity != null)
            {
                string entityName = entity.Name;
                string entityUrl = "#";

                if (entity.Published)
                {
                    if (_publishedSnapshotAccessor.TryGetPublishedSnapshot(out var publishedSnapshot))
                    {
                        var publishedEntity = publishedSnapshot.Content.GetById(entity.Key);

                        if (publishedEntity != null)
                        {
                            entityName = publishedEntity.Name(culture);
                            entityUrl = publishedEntity.Url(culture);
                        }
                    }
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

                return Ok(menuItem);
            }

            return null;
        }
    }
}