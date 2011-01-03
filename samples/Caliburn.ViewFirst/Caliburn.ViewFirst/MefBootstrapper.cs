namespace Caliburn.ViewFirst {
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using System.Windows;
    using Core.InversionOfControl;
    using MEF;
    using PresentationFramework.ApplicationModel;

    public class MefBootstrapper : Bootstrapper {
        protected override IServiceLocator CreateContainer() {
            var container = CompositionHost.Initialize(
                new AggregateCatalog(
                    SelectAssemblies().Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()
                    )
                );

            return new MEFAdapter(container);
        }

        protected override void DisplayRootView() {
            Application.Current.RootVisual = new ShellView();
        }
    }
}