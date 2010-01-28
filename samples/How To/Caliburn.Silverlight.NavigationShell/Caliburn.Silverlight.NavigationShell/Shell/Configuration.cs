namespace Caliburn.Silverlight.NavigationShell.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Core.Configuration;
    using Core.IoC;
    using Framework;
    using Microsoft.Practices.ServiceLocation;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.ViewModels;
    using ShellFramework.History;
    using ShellFramework.Questions;
    using ShellFramework.Resources;
    using ShellFramework.Services;
    using ViewModels;

    public class Configuration : ModuleBase
    {
        protected override void InitializeCore(IServiceLocator locator)
        {
            var viewLocator = (DefaultViewLocator)locator.GetInstance<IViewLocator>();

            viewLocator.AddNamespaceAlias("Caliburn.ShellFramework.Questions", "Caliburn.Silverlight.NavigationShell.Shell.Views");
            viewLocator.AddNamespaceAlias("Caliburn.ShellFramework.Services", "Caliburn.Silverlight.NavigationShell.Shell.Views");
        }

        protected override IEnumerable<IComponentRegistration> GetComponentsCore()
        {
            yield return PerRequest<IHistoryCoordinator, HistoryCoordinator>();
            yield return PerRequest<IQuestionDialog, QuestionDialogViewModel>();
            yield return Singleton<IResourceManager, DefaultResourceManager>();
            yield return Singleton<IStateManager, DeepLinkStateManager>();
            yield return Singleton<IShell, ShellViewModel>();
            yield return Singleton<ILoader, DefaultLoader>();

            var moduleTypes = from type in Assembly.GetExecutingAssembly().GetExportedTypes()
                              where typeof(ITaskBarItem).IsAssignableFrom(type)
                                    && !type.IsAbstract && !type.IsInterface
                                    && !typeof(LazyTaskBarItem).IsAssignableFrom(type)
                              select type;

            foreach (var type in moduleTypes)
            {
                yield return Singleton(typeof(ITaskBarItem), type, type.FullName);
            }
        }

        private static IComponentRegistration Singleton(Type service, Type implementation, string name)
        {
            return new Singleton
            {
                Service = service,
                Implementation = implementation,
                Name = name
            };
        }
    }
}