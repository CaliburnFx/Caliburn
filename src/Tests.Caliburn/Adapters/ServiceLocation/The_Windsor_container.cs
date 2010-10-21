namespace Tests.Caliburn.Adapters.ServiceLocation
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Components;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.Windsor;
    using NUnit.Framework;

    [TestFixture]
    public class The_Windsor_container : ServiceLocatorTests
    {
        protected override IServiceLocator CreateServiceLocator()
        {
            var container = new WindsorContainer()
                .Register(
                    AllTypes.FromThisAssembly().BasedOn<ILogger>().WithService.FirstInterface()
                );

            return new WindsorAdapter(container);
        }
    }
}