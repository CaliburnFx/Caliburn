namespace Tests.Caliburn.Adapters.Integration
{
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.Ninject;
    using Ninject;
    using NUnit.Framework;

    [TestFixture]
    public class The_Ninject_container : ContainerIntegrationTestBase
    {
        protected override IContainer CreateContainerAdapter()
        {
            var kernel = new StandardKernel();
            return new NinjectAdapter(kernel);
        }
    }
}