namespace Tests.Caliburn.Adapters.Integration
{
    using global::Caliburn.Core.InversionOfControl;
    using Xunit;

    
    public class The_Simple_Container : ContainerIntegrationTestBase
    {
        protected override IContainer CreateContainerAdapter()
        {
            return new SimpleContainer(true);
        }
    }
}