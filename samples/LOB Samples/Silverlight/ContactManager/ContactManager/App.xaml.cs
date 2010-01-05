namespace ContactManager
{
    using System.Reflection;
    using Caliburn.Core.IoC;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Configuration;
    using Microsoft.Practices.ServiceLocation;
    using Presenters.Interfaces;

    public partial class App : CaliburnApplication
    {
        public App()
        {
            InitializeComponent();
        }

        protected override IServiceLocator CreateContainer()
        {
            var container = new SimpleContainer();
            container.RegisterSingleton<IStateManager, DeepLinkStateManager>();
            return container;
        }

        protected override Assembly[] SelectAssemblies()
        {
            return new[] {Assembly.GetExecutingAssembly()};
        }

        protected override void ConfigurePresentationFramework(PresentationFrameworkConfiguration module)
        {
            module.RegisterAllScreensWithSubjects();
        }

        protected override object CreateRootModel()
        {
            return Container.GetInstance<IShellPresenter>();
        }
    }
}