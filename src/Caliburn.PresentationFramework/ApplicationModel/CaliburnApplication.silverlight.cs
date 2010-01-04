#if SILVERLIGHT

namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows;
    using Configuration;
    using Core;
    using Core.Configuration;
    using Core.IoC;
    using Microsoft.Practices.ServiceLocation;
    using System;
    using System.Windows.Browser;
    using System.Diagnostics;
    using ViewModels;
    using Screens;

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
			BeforeConfiguration();

            UnhandledException += OnUnhandledException;
            Exit += OnExit;
            Startup += OnStartup;

            _container = CreateContainer();

            var builder = CaliburnFramework
                .Configure(_container, Register);

            builder.With.Assemblies(SelectAssemblies());

            ConfigureCore(builder.With.Core());
            ConfigurePresentationFramework(builder.With.PresentationFramework());

            builder.Start();   
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
        protected virtual void Register(IEnumerable<IComponentRegistration> registrations)
        {
            var registry = _container as IRegistry;

            if (registry == null)
                throw new CaliburnException(
                    "Cannot configure Caliburn. Override Register or provide an IServiceLocator that also implements IRegistry."
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
		/// Called before the Caliburn initialization phase. 
		/// </summary>
		protected virtual void BeforeConfiguration() { }

        /// <summary>
        /// Configures the presentation framework module.
        /// </summary>
        /// <param name="module">The module.</param>
        protected virtual void ConfigurePresentationFramework(PresentationFrameworkConfiguration module) {}

        /// <summary>
        /// Configures the core module.
        /// </summary>
        /// <param name="module">The module.</param>
        protected virtual void ConfigureCore(CoreConfiguration module) { }

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
            var locator = Container.GetInstance<IViewLocator>();
            var view = locator.Locate(model, null, null);

            var binder = Container.GetInstance<IViewModelBinder>();
            binder.Bind(model, view, null);

            var screen = model as IScreen;
            if(screen != null)
            {
                screen.Initialize();
                screen.Activate();
            }

            RootVisual = (UIElement)view;
        }

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