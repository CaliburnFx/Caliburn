namespace Caliburn.Silverlight.NavigationShell
{
    using System.Reflection;
    using Framework;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.Configuration;
    using PresentationFramework.ViewModels;
    using PresentationFramework.Views;
    using ShellFramework;
    using ShellFramework.History;
    using Unity;

    public partial class App : CaliburnApplication
    {
        private IUnityContainer _container;

        public App()
        {
            InitializeComponent();
        }

        protected override IServiceLocator CreateContainer()
        {
            _container = new UnityContainer();
            return new UnityAdapter(_container);
        }

        protected override Assembly[] SelectAssemblies()
        {
            return new[] {Assembly.GetExecutingAssembly()};
        }

        protected override object CreateRootModel()
        {
            AddLazyTaskBarItem("Baz");
            return Container.GetInstance<IShell>();
        }

        protected override void ConfigurePresentationFramework(PresentationFrameworkConfiguration module)
        {
            module.With.ShellFramework()
                .ConfigureDeepLinking<DeepLinkStateManager, DefaultHistoryCoordinator>()
                .RedirectViewNamespace("Caliburn.Silverlight.NavigationShell.Shell.Views");

            module
                .RegisterAllScreensWithSubjects(true)
                .Using(x => x.ViewLocator<DefaultViewLocator>())
                    .Configured(x =>{
                        x.AddNamespaceAlias("Caliburn.Silverlight.NavigationShell.Framework",
                                            "Caliburn.Silverlight.NavigationShell.Shell.Views");
                    });
        }

        private void AddLazyTaskBarItem(string name)
        {
            _container.RegisterInstance(
                typeof(ITaskBarItem),
                name,
                new LazyTaskBarItem(
                    name,
                    "Caliburn.Silverlight.NavigationShell." + name + ".dll",
                    name + "Icon.png"
                    )
                );
        }
    }
}