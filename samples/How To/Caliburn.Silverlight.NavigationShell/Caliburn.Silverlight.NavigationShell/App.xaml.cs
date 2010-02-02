namespace Caliburn.Silverlight.NavigationShell
{
    using System.Reflection;
    using Framework;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.Configuration;
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
            module.RegisterAllScreensWithSubjects(true);
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