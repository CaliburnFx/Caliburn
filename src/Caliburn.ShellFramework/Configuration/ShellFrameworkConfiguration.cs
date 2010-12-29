namespace Caliburn.ShellFramework.Configuration
{
    using System;
    using Core.Configuration;
    using Core.InversionOfControl;
    using Menus;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.ViewModels;
    using PresentationFramework.Views;
    using Questions;
    using Resources;
    using Results;

#if SILVERLIGHT
    using ShellFramework.History;
#endif

    /// <summary>
    /// The shell framework module.
    /// </summary>
    public class ShellFrameworkConfiguration :
        ConventionalModule<ShellFrameworkConfiguration, IShellFrameworkServicesDescription>
    {
        private string viewNamespace;

        /// <summary>
        /// Adds a namespace alias to the <see cref="IViewLocator"/> from Caliburn.ShellFramework.Questions to your custom namespace.
        /// </summary>
        /// <param name="viewNamespace">The view namespace.</param>
        /// <returns>The configruation.</returns>
        public ShellFrameworkConfiguration RedirectViewNamespace(string viewNamespace)
        {
            this.viewNamespace = viewNamespace;
            return this;
        }

#if SILVERLIGHT
        /// <summary>
        /// Configures deep linking.
        /// </summary>
        /// <typeparam name="TState">The type of the state manager.</typeparam>
        /// <typeparam name="TCoordinator">The type of the history coordinator.</typeparam>
        /// <returns>The configuration.</returns>
        public ShellFrameworkConfiguration ConfigureDeepLinking<TState, TCoordinator>()
            where TState : IStateManager
            where TCoordinator : IHistoryCoordinator
        {
            AddService<Singleton, TState>(typeof(IStateManager));
            AddService<PerRequest, TCoordinator>(typeof(IHistoryCoordinator));
            return this;
        }
#endif

        /// <summary>
        /// Determines the default implementation.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>The default implemenation.</returns>
        protected override Type DetermineDefaultImplementation(Type service)
        {
            return typeof(IQuestionDialog).IsAssignableFrom(service)
                ? typeof(QuestionDialogViewModel)
                : base.DetermineDefaultImplementation(service);
        }

        /// <summary>
        /// Initializes the module with the specified locator.
        /// </summary>
        /// <param name="locator">The locator.</param>
        public override void Initialize(IServiceLocator locator)
        {
            base.Initialize(locator);

            MenuModel.Initialize(locator.GetInstance<IInputManager>(), locator.GetInstance<IResourceManager>());

            if(!string.IsNullOrEmpty(viewNamespace))
            {
                var viewLocator = locator.GetInstance<IViewLocator>() as DefaultViewLocator;
                if(viewLocator != null)
                    viewLocator.AddNamespaceAlias("Caliburn.ShellFramework.Questions", viewNamespace);
            }
        }
    }
}