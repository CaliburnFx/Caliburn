using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Tests.Caliburn.Adapters.Components;

namespace Tests.Caliburn.Adapters.ServiceLocation
{
    using global::Caliburn.Windsor;

    [TestFixture]
    public class The_Windsor_container : ServiceLocatorTests
    {
        protected override IServiceLocator CreateServiceLocator()
        {
            IWindsorContainer container = new WindsorContainer()
                .Register(
                AllTypes.FromThisAssembly().BasedOn<ILogger>().WithService.FirstInterface()
                );

            return new WindsorAdapter(container);
        }
    }
}