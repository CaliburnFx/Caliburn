namespace GameLibrary {
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Windows.Controls;
    using Caliburn.Core;
    using Caliburn.Core.Configuration;
    using Caliburn.Core.InversionOfControl;
    using Caliburn.MEF;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Conventions;
    using Caliburn.PresentationFramework.Views;
    using Caliburn.ShellFramework;
    using Framework;

    public class AppBootstrapper : Bootstrapper<IShell> {
        protected override IServiceLocator CreateContainer() {
            var catalog = new AggregateCatalog(
                SelectAssemblies().Select(x => new AssemblyCatalog(x)).ToArray()
                );

            var container = CompositionHost.Initialize(catalog);

            return new MEFAdapter(container);
        }

        protected override void Configure(IConfigurationBuilder builder) {
            base.Configure(builder);

            builder.With.Core()
                .LocateLoggerWith(x => new DebugLog(x));
            builder.With.PresentationFramework()
                .Using(x => x.ViewLocator<DefaultViewLocator>())
                    .Configured(x => x.AddNamespaceAlias("GameLibrary.Model", "GameLibrary.Views"))
                .Using(x => x.ConventionManager<DefaultConventionManager>())
                    .Configured(x => {
                        x.AddElementConvention<BusyIndicator>("IsBusyChanged", BusyIndicator.IsBusyProperty, (c, o) => c.IsBusy = (bool)o, c => c.IsBusy);
                        x.AddElementConvention<Rating>("ValueChanged", Rating.ValueProperty, (c, o) => c.Value = (double?)o, c => c.Value);
                    });
            builder.With.ShellFramework();
        }
    }
}