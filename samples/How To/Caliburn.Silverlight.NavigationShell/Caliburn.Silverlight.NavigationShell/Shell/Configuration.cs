namespace Caliburn.Silverlight.NavigationShell.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Core.Configuration;
    using Core.IoC;
    using Framework;
    using ViewModels;

    public class Configuration : ModuleBase
    {
        public override IEnumerable<IComponentRegistration> GetComponents()
        {
            yield return Singleton<IShell, ShellViewModel>();

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