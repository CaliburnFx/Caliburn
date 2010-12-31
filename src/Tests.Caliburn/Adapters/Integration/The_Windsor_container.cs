namespace Tests.Caliburn.Adapters.Integration
{
    using Castle.Windsor;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.Windsor;
    using NUnit.Framework;

    [TestFixture]
    public class The_Windsor_container : ContainerIntegrationTestBase
    {
        protected override IContainer CreateContainerAdapter()
        {
            IWindsorContainer container = new WindsorContainer();
            return new WindsorAdapter(container);
        }
    }
}