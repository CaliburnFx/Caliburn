namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Threading;
    using Configuration;
    using Core;
    using Core.Configuration;
    using Core.InversionOfControl;

    /// <summary>
    /// Instantiate this class in order to configure the framework.
    /// </summary>
    public class Bootstrapper
    {
        /// <summary>
        /// The container.
        /// </summary>
        protected IServiceLocator container;

        /// <summary>
        /// Creates an instance of the bootstrapper.
        /// </summary>
        public Bootstrapper()
        {
            if (PresentationFrameworkConfiguration.IsInDesignMode)
                StartDesignTime();
            else StartRuntime();
        }

        /// <summary>
        /// Called by the bootstrapper's constructor at design time to start the framework.
        /// </summary>
        protected virtual void StartDesignTime() { }

        /// <summary>
        /// Called by the bootstrapper's constructor at runtime to start the framework.
        /// </summary>
        protected virtual void StartRuntime()
        {
            container = CreateContainer();
            var builder = CaliburnFramework
                .Configure(container, Register);

            Configure(builder);

            builder.Start();

            Application = Application.Current;
            PrepareApplication();
        }

        /// <summary>
        /// The application.
        /// </summary>
        public Application Application { get; private set; }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public IServiceLocator Container
        {
            get { return container; }
        }

        /// <summary>
        /// Provides an opportunity to hook into the application object.
        /// </summary>
        protected virtual void PrepareApplication()
        {
            Application.Startup += OnStartup;
            Application.DispatcherUnhandledException += OnUnhandledException;
            Application.Exit += OnExit;
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
            var registry = container as IRegistry;

            if(registry == null)
            {
                throw new CaliburnException(
                    "Cannot configure Caliburn. Override Register or provide an IServiceLocator that also implements IRegistry."
                    );
            }

            registry.Register(registrations);
        }

        /// <summary>
        /// Override to tell the framework where to find assemblies to inspect for views, etc.
        /// </summary>
        /// <returns>A list of assemblies to inspect.</returns>
        protected virtual IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] { Assembly.GetEntryAssembly() };
        }

        /// <summary>
        /// Used to configure the framework.
        /// </summary>
        /// <param name="builder">The configuration builder.</param>
        protected virtual void Configure(IConfigurationBuilder builder)
        {
            builder.With.Assemblies(SelectAssemblies().ToArray());
            builder.With.Core();
            builder.With.PresentationFramework();
        }

        /// <summary>
        /// Override this to add custom behavior to execute after the application starts.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The args.</param>
        protected virtual void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootView();
        }

        /// <summary>
        /// Override this to add custom behavior on exit.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        protected virtual void OnExit(object sender, EventArgs e) {}

        /// <summary>
        /// Override to display your UI at startup.
        /// </summary>
        protected virtual void DisplayRootView() {}

        /// <summary>
        /// Override this to add custom behavior for unhandled exceptions.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        protected virtual void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {}
    }

    /// <summary>
    /// A strongly-typed version of <see cref="Bootstrapper"/> that specifies the type of root model to create for the application.
    /// </summary>
    /// <typeparam name="TRootModel">The type of root model for the application.</typeparam>
    public class Bootstrapper<TRootModel> : Bootstrapper
    {
        /// <summary>
        /// Override to display your UI at startup.
        /// </summary>
        protected override void DisplayRootView()
        {
            var viewModel = Container.GetInstance<TRootModel>();
            Container.GetInstance<IWindowManager>()
                .ShowWindow(viewModel);
        }
    }
}
