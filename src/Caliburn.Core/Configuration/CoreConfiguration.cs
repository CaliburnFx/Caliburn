namespace Caliburn.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Behaviors;
    using InversionOfControl;
    using Logging;

    /// <summary>
    /// Configures Caliburn's core.
    /// </summary>
    public class CoreConfiguration : ConventionalModule<CoreConfiguration, ICoreServicesDescription>
    {
        readonly List<Action> afterStart = new List<Action>();

        /// <summary>
        /// Adds actions to execute immediately following the framework startup.
        /// </summary>
        /// <param name="doThis">The action to execute after framework startup.</param>
        /// <returns></returns>
        public CoreConfiguration AfterStart(Action doThis)
        {
            afterStart.Add(doThis);
            return this;
        }

        /// <summary>
        /// Enables customization of the discoverable types within a given assembly.
        /// </summary>
        /// <param name="assemblyInspector">The assembly inspector.</param>
        /// <returns></returns>
        public CoreConfiguration LocateTypesWith(Func<Assembly,IEnumerable<Type>> assemblyInspector)
        {
            CoreExtensions.GetInspectableTypesImplementation = assemblyInspector;
            return this;
        }

        /// <summary>
        /// Enables customization of default constructor selection used by the default container and various integrated containers when proxying.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <returns>The configuration.</returns>
        public CoreConfiguration SelectConstructorsWith(Func<Type, ConstructorInfo> selector)
        {
            IoCExtensions.SelectEligibleConstructorImplementation = selector;
            return this;
        }

        /// <summary>
        /// Enables customization of checks made by the framework to determine if a certain type should be proxied.
        /// </summary>
        /// <param name="requiresProxy">The checker</param>
        /// <returns>The configuration.</returns>
        public CoreConfiguration CheckForProxyRequirementWith(Func<Type, bool> requiresProxy)
        {
            BehaviorExtensions.DefaultShouldCreateProxyImplementation = requiresProxy;
            return this;
        }

        /// <summary>
        /// Customizes the way loggers are located.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <returns>The configuration.</returns>
        public CoreConfiguration LocateLoggerWith(Func<Type, ILog> locator)
        {
            LogManager.Initialize(locator);
            return this;
        }

        /// <summary>
        /// Determines the model type using the provided custom delegate.
        /// </summary>
        /// <param name="getType">Gets the type.</param>
        /// <returns>The configuration.</returns>
        public CoreConfiguration DetermineModelTypeWith(Func<object, Type> getType)
        {
            CoreExtensions.GetModelTypeImplementation = getType;
            return this;
        }

        internal void ExecuteAfterStart()
        {
            afterStart.Apply(x => x());
            afterStart.Clear();
        }
    }
}