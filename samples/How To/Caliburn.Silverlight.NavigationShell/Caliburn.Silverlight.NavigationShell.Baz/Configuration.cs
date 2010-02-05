namespace Caliburn.Silverlight.NavigationShell.Baz
{
    using System.Collections.Generic;
    using Core.Configuration;
    using Core.IoC;
    using Microsoft.Practices.ServiceLocation;

    public class Configuration : ModuleBase
    {
        public override IEnumerable<IComponentRegistration> GetComponents()
        {
            //yield module specific components
            yield break;
        }

        public override void Initialize(IServiceLocator locator)
        {
            //execute module specific initialization
        }
    }
}