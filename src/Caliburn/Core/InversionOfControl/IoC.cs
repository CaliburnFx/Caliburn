namespace Caliburn.Core.InversionOfControl
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A static gateway to the <see cref="IServiceLocator"/>.
    /// </summary>
    public static class IoC
    {
        static IServiceLocator locator;
        static IBuilder builder;

        /// <summary>
        /// Initializes the gateway.
        /// </summary>
        /// <param name="locator">The <see cref="IServiceLocator"/> to use.</param>
        public static void Initialize(IServiceLocator locator)
        {
            IoC.locator = locator;
            builder = locator as IBuilder;
        }

        /// <summary>
        /// Gets an instance by type and key.
        /// </summary>
        public static object GetInstance(Type serviceType, string key)
        {
            return locator.GetInstance(serviceType, key);
        }

        /// <summary>
        /// Gets all instances of a particular type.
        /// </summary>
        public static IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return locator.GetAllInstances(serviceType);
        }

        /// <summary>
        /// Passes an existing instance to the IoC container to enable dependencies to be injected.
        /// </summary>
        public static void BuildUp(object instance)
        {
            if(builder != null)
                builder.BuildUp(instance);
        }

        /// <summary>
        /// Gets an instance by type.
        /// </summary>
        /// <typeparam name="T">The type to resolve from the container.</typeparam>
        /// <returns>The resolved instance.</returns>
        public static T Get<T>()
        {
            return (T)GetInstance(typeof(T), null);
        }

        /// <summary>
        /// Gets an instance from the container using type and key.
        /// </summary>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <param name="key">The key to look up.</param>
        /// <returns>The resolved instance.</returns>
        public static T Get<T>(string key)
        {
            return (T)GetInstance(typeof(T), key);
        }
    }
}