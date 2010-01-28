namespace Caliburn.Silverlight.NavigationShell.Baz
{
    using System.Collections.Generic;
    using Core.Configuration;
    using Core.IoC;
    using Microsoft.Practices.ServiceLocation;

    public class Configuration : ModuleBase
    {
        protected override IEnumerable<IComponentRegistration> GetComponentsCore()
        {
            //yield module specific components
            return base.GetComponentsCore();
        }

        protected override void InitializeCore(IServiceLocator locator)
        {
            //execute module specific initialization
            base.InitializeCore(locator);
        }
    }
}