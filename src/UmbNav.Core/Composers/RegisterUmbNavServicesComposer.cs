using UmbNav.Core.Interfaces;
using UmbNav.Core.Services;
#if NETCOREAPP
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;
#else
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

namespace UmbNav.Core.Composers
{
#if !NETCOREAPP
    [RuntimeLevel(MinLevel = RuntimeLevel.Boot)]
#endif
    public class RegisterUmbNavServicesComposer : IUserComposer
    {
#if NETCOREAPP
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddUnique<IUmbNavMenuBuilderService, UmbNavMenuBuilderService>();

        }
#else
        public void Compose(Composition composition)
        {
            composition.Register<IUmbNavMenuBuilderService, UmbNavMenuBuilderService>(Lifetime.Request);
        }
#endif
    }
}
