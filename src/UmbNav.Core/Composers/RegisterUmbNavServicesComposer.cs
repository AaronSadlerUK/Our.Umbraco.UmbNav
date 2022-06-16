using UmbNav.Core.Interfaces;
using UmbNav.Core.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

namespace UmbNav.Core.Composers
{
    public class RegisterUmbNavServicesComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddUnique<IUmbNavMenuBuilderService, UmbNavMenuBuilderService>();

        }
    }
}
