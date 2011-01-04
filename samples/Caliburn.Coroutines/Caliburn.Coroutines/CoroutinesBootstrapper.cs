namespace Caliburn.Coroutines {
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using System.Windows;
    using Core.Configuration;
    using Core.InversionOfControl;
    using MEF;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;

    public class CoroutinesBootstrapper : Bootstrapper<IShell> {
        CompositionContainer container;

        protected override IServiceLocator CreateContainer() {
            var catalog = new AggregateCatalog(SelectAssemblies().Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>());
            container = CompositionHost.Initialize(catalog);

            var batch = new CompositionBatch();
            batch.AddExportedValue(catalog);
            container.Compose(batch);

            return new MEFAdapter(container);
        }

        protected override void Configure(IConfigurationBuilder builder) {
            base.Configure(builder);

            Coroutine.Completed += (s, e) => {
                if(e.Error != null)
                    MessageBox.Show(e.Error.Message);
            };
        }
    }
}