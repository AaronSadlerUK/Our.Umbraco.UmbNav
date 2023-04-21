using Umbraco.Cms.Core.DependencyInjection;

namespace UmbNav.Web
{
    public static class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder AddUmbNav(this IUmbracoBuilder builder)
        {
            builder.BackOfficeAssets()
                .Append<PickletreeJsFile>();

            return builder;
        }
    }
}
