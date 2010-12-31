namespace Tests.Caliburn.Adapters.Integration
{
    using global::Caliburn.Core.InversionOfControl;
    using NUnit.Framework;

    [TestFixture]
    public class The_Simple_Container : ContainerIntegrationTestBase
    {
        protected override IContainer CreateContainerAdapter()
        {
            return new SimpleContainer(true);
        }
    }
}