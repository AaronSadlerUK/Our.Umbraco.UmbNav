using UmbNavV8.Core.Interfaces;
using UmbNavV8.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace UmbNavV8.Core.Composers
{
    [RuntimeLevel(MaxLevel = RuntimeLevel.Run)]
    public class RegisterUmbNavServicesComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IUmbNavMenuBuilderService, UmbNavMenuBuilderService>(Lifetime.Request);
        }
    }
}
