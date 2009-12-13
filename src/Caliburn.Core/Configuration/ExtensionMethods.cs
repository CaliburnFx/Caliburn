namespace Caliburn.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using IoC;

    /// <summary>
    /// Various extension methods related to modules and configuration.
    /// </summary>
    public static class ExtensionMethods
    {
        private static readonly Type _moduleType = typeof(IModule);

        /// <summary>
        /// Configures the core.
        /// </summary>
        /// <param name="hook">The hook.</param>
        /// <returns></returns>
        public static CoreConfiguration Core(this IModuleHook hook)
        {
            return CaliburnModule<CoreConfiguration>.Instance;
        }

        public static void Insepct(this Assembly assembly, ICollection<IComponentRegistration> componentList, ICollection<IModule> modules)
        {
            var types = assembly.GetExportedTypes();

            foreach (var type in types)
            {
                foreach (var attribute in type.GetCustomAttributes(true).OfType<RegisterAttribute>())
                    componentList.Add(attribute.GetComponentInfo(type));
            }

            foreach (var type in types)
            {
                if(!_moduleType.IsAssignableFrom(type) || type.IsAbstract || type.IsInterface) 
                    continue;

                var singleton = type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);

                if(singleton != null)
                    modules.Add((IModule)singleton.GetValue(null, null));
                else modules.Add((IModule)Activator.CreateInstance(type));
            }
        }
    }
}