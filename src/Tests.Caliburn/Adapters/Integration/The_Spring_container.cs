namespace Tests.Caliburn.Adapters.Integration
{
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.Spring;
    using NUnit.Framework;
    using Spring.Context.Support;
    using Spring.Objects.Factory.Config;

    [TestFixture]
    public class The_Spring_container : ContainerIntegrationTestBase
    {
        protected override IContainer CreateContainerAdapter()
        {
            var context = new GenericApplicationContext(false);
            return new SpringAdapter(context, AutoWiringMode.AutoDetect);
        }
    }
}