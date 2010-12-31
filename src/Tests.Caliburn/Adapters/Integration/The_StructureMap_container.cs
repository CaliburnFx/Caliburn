namespace Tests.Caliburn.Adapters.Integration
{
    using global::Caliburn.StructureMap;
    using NUnit.Framework;
    using StructureMap;
    using StructureMap.Configuration.DSL;
    using IContainer = global::Caliburn.Core.InversionOfControl.IContainer;

    [TestFixture]
    public class The_StructureMap_container : ContainerIntegrationTestBase
    {
        protected override IContainer CreateContainerAdapter()
        {
            var registry = new Registry();
            StructureMap.IContainer container = new Container(registry);

            return new StructureMapAdapter(container);
        }
    }
}