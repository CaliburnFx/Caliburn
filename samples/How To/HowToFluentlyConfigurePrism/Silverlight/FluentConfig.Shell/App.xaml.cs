namespace FluentConfig.Shell
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Browser;
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
    using Caliburn.PresentationFramework.ViewModels;

    public partial class App : Application
    {
        public App()
        {
            Startup += Application_Startup;
            Exit += Application_Exit;
            UnhandledException += Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CaliburnFramework
                .Configure()
                .With.Core()
                .With.PresentationFramework()
                .With.CompositeApplicationLibrary(CreateShell)
                .WithModuleCatalog(new ModuleCatalog().AddModule(typeof(CalculatorModule)))
                .Start();
        }

        private DependencyObject CreateShell()
        {
            IShellPresenter model = ServiceLocator.Current.GetInstance<ShellPresenter>();
            var viewStrategy = ServiceLocator.Current.GetInstance<IViewLocator>();
            var view = (FrameworkElement)viewStrategy.Locate(model, null, null);

            var binder = ServiceLocator.Current.GetInstance<IViewModelBinder>();
            binder.Bind(model, view, null);

            RootVisual = view;

            return view;
        }

        private void Application_Exit(object sender, EventArgs e) {}

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if(!Debugger.IsAttached)
            {
                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch(Exception) {}
        }
    }
}