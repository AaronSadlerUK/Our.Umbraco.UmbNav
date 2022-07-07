using System;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace UmbNav.Api.Extensions
{
    public static class ContentServiceExtensions
    {
        public static IContent? GetById(this IContentService contentService, Udi id)
        {
            if (id is not GuidUdi guidUdi)
                throw new InvalidOperationException("The UDI provided isn't of type " + typeof(GuidUdi) + " which is required by content");

            return contentService.GetById(guidUdi.Guid);
        }
    }
}
