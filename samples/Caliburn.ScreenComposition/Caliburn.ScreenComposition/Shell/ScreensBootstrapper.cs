namespace Caliburn.ScreenComposition.Shell
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using System.Windows;
    using Core.InversionOfControl;
    using Customers;
    using Framework;
    using MEF;
    using Orders;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.Invocation;

    public class ScreensBootstrapper : Bootstrapper<IShell>
    {
        Window mainWindow;
        bool actuallyClosing;

        protected override IServiceLocator CreateContainer() {
            var container = CompositionHost.Initialize(
                new AggregateCatalog(
                    SelectAssemblies().Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()
                    )
                );

            var batch = new CompositionBatch();
            batch.AddExportedValue<Func<IMessageBox>>(() => container.GetExportedValue<IMessageBox>());
            batch.AddExportedValue<Func<CustomerViewModel>>(() => container.GetExportedValue<CustomerViewModel>());
            batch.AddExportedValue<Func<OrderViewModel>>(() => container.GetExportedValue<OrderViewModel>());
            container.Compose(batch);

            return new MEFAdapter(container);
        }

        protected override void DisplayRootView()
        {
            base.DisplayRootView();

            if (Application.IsRunningOutOfBrowser) {
                mainWindow = Application.MainWindow;
                mainWindow.Closing += MainWindowClosing;
            }
        }

        void MainWindowClosing(object sender, ClosingEventArgs e) {
            if (actuallyClosing)
                return;

            e.Cancel = true;

            Execute.OnUIThread(() => {
                var shell = IoC.Get<IShell>();

                shell.CanClose(result => {
                    if(result) {
                        actuallyClosing = true;
                        mainWindow.Close();
                    }
                });
            });
        }
    }
}