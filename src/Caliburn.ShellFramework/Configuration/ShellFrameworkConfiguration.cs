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

    public class ShellFrameworkConfiguration :
        ConventionalModule<ShellFrameworkConfiguration, IShellFrameworkServicesDescription>
    {
        private string viewNamespace;

        public ShellFrameworkConfiguration RedirectViewNamespace(string viewNamespace)
        {
            this.viewNamespace = viewNamespace;
            return this;
        }

#if SILVERLIGHT
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