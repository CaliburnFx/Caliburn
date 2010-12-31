namespace Tests.Caliburn.Adapters.Integration
{
    using Autofac;
    using global::Caliburn.Autofac;
    using NUnit.Framework;
    using IContainer = global::Caliburn.Core.InversionOfControl.IContainer;

    [TestFixture]
    public class The_Autofac_container : ContainerIntegrationTestBase
    {
        protected override IContainer CreateContainerAdapter()
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();
            return new AutofacAdapter(container);
        }

        [Test, Ignore]
        public override void can_inject_dependencies_on_public_properties() {}
    }
}