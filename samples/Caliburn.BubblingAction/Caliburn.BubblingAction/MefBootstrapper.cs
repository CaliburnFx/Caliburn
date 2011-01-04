namespace Caliburn.BubblingAction {
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using Core.InversionOfControl;
    using MEF;
    using PresentationFramework.ApplicationModel;

    public class MefBootstrapper : Bootstrapper<IShell> {
        protected override IServiceLocator CreateContainer() {
            var container = CompositionHost.Initialize(
                new AggregateCatalog(
                    SelectAssemblies().Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()
                    )
                );

            return new MEFAdapter(container);
        }
    }
}