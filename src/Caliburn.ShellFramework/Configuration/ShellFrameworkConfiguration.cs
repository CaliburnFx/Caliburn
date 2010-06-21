namespace Caliburn.ShellFramework.Configuration
{
    using System;
    using Core.Configuration;
    using Menus;
    using Microsoft.Practices.ServiceLocation;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.ViewModels;
    using PresentationFramework.Views;
    using Questions;
    using Results;

#if SILVERLIGHT
#if !WP7
    using ShellFramework.History;
#endif
    using PresentationFramework.ApplicationModel;
    using Core.IoC;
#endif

    public class ShellFrameworkConfiguration :
        ConventionalModule<ShellFrameworkConfiguration, IShellFrameworkServicesDescription>
    {
        private string _viewNamespace;

        public ShellFrameworkConfiguration RedirectViewNamespace(string viewNamespace)
        {
            _viewNamespace = viewNamespace;
            return this;
        }

#if SILVERLIGHT && !WP7
        public ShellFrameworkConfiguration ConfigureDeepLinking<TState, TCoordinator>()
            where TState : IStateManager
            where TCoordinator : IHistoryCoordinator
        {
            AddService<Singleton, TState>(typeof(IStateManager));
            AddService<PerRequest, TCoordinator>(typeof(IHistoryCoordinator));
            return this;
        }
#endif

        protected override Type DetermineDefaultImplementation(Type service)
        {
            return typeof(IQuestionDialog).IsAssignableFrom(service)
                ? typeof(QuestionDialogViewModel)
                : base.DetermineDefaultImplementation(service);
        }

        public override void Initialize(IServiceLocator locator)
        {
            base.Initialize(locator);

            MenuModel.Initialize(locator.GetInstance<IInputManager>());

            if(!string.IsNullOrEmpty(_viewNamespace))
            {
                var viewLocator = locator.GetInstance<IViewLocator>() as DefaultViewLocator;
                if(viewLocator != null)
                    viewLocator.AddNamespaceAlias("Caliburn.ShellFramework.Questions", _viewNamespace);
            }
        }
    }
}