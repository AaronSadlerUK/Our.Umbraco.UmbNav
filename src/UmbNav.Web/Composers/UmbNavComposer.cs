using UmbNav.Web.Filters;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace UmbNav.Web.Composers
{
    public class UmbNavComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.ManifestFilters().Append<UmbNavManifestFilter>();

            builder.AddUmbNav();
        }
    }
}
