namespace FluentConfig.Shell
{
    using System;
    using System.Windows;
    using CaliBrism.Modules.Calculator;
    using Caliburn.Core;
    using Caliburn.Core.Configuration;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Configuration;
    using Caliburn.Prism;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.ServiceLocation;
    using Presenters;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            CaliburnFramework
                .Configure()
                .With.Core()
                .AfterStart(() =>{
                    var binder = (DefaultBinder)ServiceLocator.Current.GetInstance<IBinder>();
                    binder.EnableMessageConventions();
                    binder.EnableBindingConventions();
                })
                .With.PresentationFramework()
                .With.CompositeApplicationLibrary(CreateShell)
                .WithModuleCatalog(new ModuleCatalog().AddModule(typeof(CalculatorModule)))
                .Start();
        }

        private DependencyObject CreateShell()
        {
            ServiceLocator.Current.GetInstance<IWindowManager>()
                .Show(ServiceLocator.Current.GetInstance<ShellPresenter>(), null, ExecuteShutdownModel);

            return MainWindow;
        }

        private void ExecuteShutdownModel(ISubordinate subordinate, Action completed)
        {
            completed();
        }
    }
}