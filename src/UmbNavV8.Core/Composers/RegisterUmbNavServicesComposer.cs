#if NETCOREAPP
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;
#else
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif
using UmbNavV8.Core.Interfaces;
using UmbNavV8.Core.Services;

namespace UmbNavV8.Core.Composers
{
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
