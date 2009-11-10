#if SILVERLIGHT

namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows;
    using Core;
    using Core.IoC;
    using Microsoft.Practices.ServiceLocation;
    using System;
    using System.Windows.Browser;
    using System.Diagnostics;

    /// <summary>
    /// A base class for applications based on Caliburn.
    /// </summary>
    public class CaliburnApplication : Application
    {
        private readonly IServiceLocator _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaliburnApplication"/> class.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// More than one instance of the <see cref="T:System.Windows.Application"/> class is created per <see cref="T:System.AppDomain"/>.
        /// </exception>
        public CaliburnApplication()
        {
            UnhandledException += OnUnhandledException;
            Exit += OnExit;
            Startup += OnStartup;

            _container = CreateContainer();

            var core = CaliburnFramework
                .ConfigureCore(_container, ConfigureCaliburn);

            core.WithAssemblies(SelectAssemblies());

            var frameworkConfiguration = core.WithPresentationFramework();

            ConfigurePresentationFramework(frameworkConfiguration);

            BeforeStart(core);

            core.Start();    
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public IServiceLocator Container
        {
            get { return _container; }
        }

        /// <summary>
        /// Creates the container.
        /// </summary>
        /// <returns></returns>
        protected virtual IServiceLocator CreateContainer()
        {
            return new SimpleContainer();
        }

        /// <summary>
        /// Configures Caliburn's components.
        /// </summary>
        /// <param name="registrations">The component registrations.</param>
        protected virtual void ConfigureCaliburn(IEnumerable<IComponentRegistration> registrations)
        {
            var registry = _container as IRegistry;

            if (registry == null)
                throw new CaliburnException(
                    "Cannot configure Caliburn. Override ConfigureCaliburn or provide an IServiceLocator that also implements IRegistry."
                    );

            registry.Register(registrations);
        }

        /// <summary>
        /// Selects the assemblies which Caliburn will be able to inspect for components, views, etc.
        /// </summary>
        /// <returns></returns>
        protected virtual Assembly[] SelectAssemblies()
        {
            return new Assembly[] {};
        }

        /// <summary>
        /// Configures the presentation framework.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        protected virtual void ConfigurePresentationFramework(PresentationFrameworkModule configuration) { }

        /// <summary>
        /// Creates the root application model.
        /// </summary>
        /// <returns></returns>
        protected virtual object CreateRootModel()
        {
            return null;
        }

        /// <summary>
        /// Called on startup.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.StartupEventArgs"/> instance containing the event data.</param>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var model = CreateRootModel();
            if (model == null) return;

            SetRootVisual(model);
        }

        /// <summary>
        /// Set's the RootVisual based on the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        protected virtual void SetRootVisual(object model)
        {
            var strategy = Container.GetInstance<IViewStrategy>();
            var view = (FrameworkElement)strategy.GetView(model, null, null);

            var binder = Container.GetInstance<IBinder>();
            binder.Bind(model, view, null);

            var presenter = model as IPresenter;
            if(presenter != null)
            {
                presenter.Initialize();
                presenter.Activate();
            }

            RootVisual = view;
        }

        /// <summary>
        /// Configures additional modules befores the starting the framework.
        /// </summary>
        /// <param name="core">The core.</param>
        protected virtual void BeforeStart(CoreConfiguration core) {}

        /// <summary>
        /// Called when the application exits.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnExit(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Called when an unhandled exception occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ApplicationUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        protected virtual void OnUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if(Debugger.IsAttached) return;

            e.Handled = true;
            Deployment.Current.Dispatcher.BeginInvoke(() => ReportErrorToDOM(e));
        }

        /// <summary>
        /// Reports the error to the DOM.
        /// </summary>
        /// <param name="e">The <see cref="ApplicationUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        protected virtual void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                var errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight 2 Application " + errorMsg + "\");");
            }
            catch(Exception) {}
        }
    }
}

#endif