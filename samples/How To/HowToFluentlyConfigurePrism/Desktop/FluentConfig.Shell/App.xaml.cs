namespace FluentConfig.Shell
{
    using System;
    using System.Windows;
    using CaliBrism.Modules.Calculator;
    using Caliburn.Core;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.ApplicationModel;
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
                .ConfigureCore()
                .AfterStart(() =>{
                    var binder = (DefaultBinder)ServiceLocator.Current.GetInstance<IBinder>();
                    binder.EnableMessageConventions();
                    binder.EnableBindingConventions();
                })
                .WithPresentationFramework()
                .WithCompositeApplicationLibrary(CreateShell)
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