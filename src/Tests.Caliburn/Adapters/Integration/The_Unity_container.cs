namespace Tests.Caliburn.Adapters.Integration
{
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.Unity;
    using Microsoft.Practices.Unity;
    using NUnit.Framework;

    [TestFixture]
    public class The_Unity_container : ContainerIntegrationTestBase
    {
        protected override IContainer CreateContainerAdapter()
        {
            IUnityContainer container = new UnityContainer();
            return new UnityAdapter(container);
        }
    }
}