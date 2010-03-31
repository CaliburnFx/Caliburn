namespace GameLibrary
{
    using System.ComponentModel.Composition.Hosting;
    using System.Reflection;
    using System.Windows.Controls;
    using Caliburn.Core.Logging;
    using Caliburn.MEF;
    using Caliburn.PresentationFramework.Configuration;
    using Caliburn.PresentationFramework.Conventions;
    using Caliburn.ShellFramework;
    using Framework;
    using Microsoft.Practices.ServiceLocation;

    public partial class App
    {
        public App()
        {
            LogManager.Initialize(x => new DebugLog(x));
            InitializeComponent();
        }

        protected override IServiceLocator CreateContainer()
        {
            return new MEFAdapter(CompositionHost.Initialize(new AssemblyCatalog(Assembly.GetExecutingAssembly())));
        }

        protected override object CreateRootModel()
        {
            var shell = ServiceLocator.Current.GetInstance<IShell>();

            if(IsRunningOutOfBrowser && HasElevatedPermissions)
                MainWindow.Closing += (s, args) => { args.Cancel = !ServiceLocator.Current.GetInstance<IShell>().CanShutdown(); };

            return shell;
        }

        protected override void ConfigurePresentationFramework(PresentationFrameworkConfiguration module)
        {
            module.Using(x => x.ConventionManager<DefaultConventionManager>())
                .Configured(x => {
                    x.AddElementConvention<BusyIndicator>("IsBusyChanged", BusyIndicator.IsBusyProperty, (c, o) => c.IsBusy = (bool)o, c => c.IsBusy);
                    x.AddElementConvention<Rating>("ValueChanged", Rating.ValueProperty, (c, o) => c.Value = (double?)o, c => c.Value);
                });

            module.With.ShellFramework();
        }
    }
}