using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace UmbNavV8.Api.Controllers.API
{
    [PluginController(Core.Constants.PackageName)]
    public class UmbNavEntityApiController : UmbracoAuthorizedJsonController
    {
        public HttpResponseMessage GetById(string id, string culture = null)
        {
            var udiList = new List<Udi>();
            var udi = Udi.Parse(id);
            udiList.Add(udi);
            var entity = Services.ContentService.GetByIds(udiList).FirstOrDefault();

            if (entity != null)
            {
                string entityName = entity.Name;
                string entityUrl = "#";

                if (entity.Published)
                {
                    var publishedEntity = Umbraco.Content(entity.Key);

                    if (publishedEntity != null)
                    {
                        entityName = publishedEntity.Name(culture);
                        entityUrl = publishedEntity.Url(culture);
                    }
                }

                var menuItem = new
                {
                    id = entity.Id,
                    udi = entity.GetUdi(),
                    name = entityName,
                    icon = entity.ContentType.Icon,
                    url = entityUrl,
                    published = entity.Published,
                    naviHide = entity.HasProperty("umbracoNaviHide") && entity.GetValue<bool>("umbracoNaviHide") ||
                               entity.HasProperty("umbracoNavihide") && entity.GetValue<bool>("umbracoNavihide"),
                    culture = culture
                };

                return Request.CreateResponse(HttpStatusCode.OK, menuItem);
            }

            return null;
        }
    }
}