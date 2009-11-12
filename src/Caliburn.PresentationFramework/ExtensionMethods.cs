﻿namespace Caliburn.PresentationFramework
{
    using System.Windows;
    using System.Windows.Media;
    using Core;

    /// <summary>
    /// Extension methods related to the PresentationFramework classes.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the resource by searching the hierarchy of of elements.
        /// </summary>
        /// <typeparam name="T">The type of resource.</typeparam>
        /// <param name="element">The element.</param>
        /// <param name="key">The key.</param>
        /// <returns>The resource.</returns>
        public static T GetResource<T>(this FrameworkElement element, object key)
        {
            DependencyObject currentElement = element;

            while (currentElement != null)
            {
                var frameworkElement = currentElement as FrameworkElement;

                if (frameworkElement != null && frameworkElement.Resources.Contains(key))
                    return (T)frameworkElement.Resources[key];

#if !SILVERLIGHT
                currentElement = (LogicalTreeHelper.GetParent(currentElement) ??
                    VisualTreeHelper.GetParent(currentElement));
#else
                currentElement = VisualTreeHelper.GetParent(currentElement);
#endif
            }

            if (Application.Current != null && Application.Current.Resources.Contains(key))
                return (T)Application.Current.Resources[key];

            return default(T);
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets the resource by searching the hierarchy of of elements.
        /// </summary>
        /// <typeparam name="T">The type of resource.</typeparam>
        /// <param name="element">The element.</param>
        /// <param name="key">The key.</param>
        /// <returns>The resource.</returns>
        public static T GetResource<T>(this FrameworkContentElement element, object key)
        {
            DependencyObject currentElement = element;

            while (currentElement != null)
            {
                var fce = currentElement as FrameworkContentElement;

                if (fce != null && fce.Resources.Contains(key))
                    return (T)fce.Resources[key];

                currentElement = (LogicalTreeHelper.GetParent(currentElement) ??
                    VisualTreeHelper.GetParent(currentElement));

            }

            if (Application.Current != null && Application.Current.Resources.Contains(key))
                return (T)Application.Current.Resources[key];

            return default(T);
        }
#endif

        /// <summary>
        /// Adds the routed UI messaging module's configuration to the system.
        /// </summary>
        /// <param name="hook">The hook.</param>
        /// <returns>The configuration.</returns>
        public static PresentationFrameworkModule WithPresentationFramework(this IConfigurationHook hook)
        {
            return new PresentationFrameworkModule(hook);
        }

        /// <summary>
        /// Finds the interaction defaults or fail.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static InteractionDefaults FindDefaultsOrFail(this IRoutedMessageController controller, object element)
        {
            var type = element.GetType();
            var defaults = controller.GetInteractionDefaults(type);

            if (defaults == null)
                throw new CaliburnException(
                    string.Format("Could not locate InteractionDefaults for {0}.  Please register with the IRoutedMessageController.", type.Name)
                    );

            return defaults;
        }

        /// <summary>
        /// Finds an element by name or fails.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element.</param>
        /// <param name="name">The name.</param>
        /// <param name="shouldFail">Indicates whether an exception should be throw if the named item is not found.</param>
        /// <returns></returns>
        public static T FindName<T>(this FrameworkElement element, string name, bool shouldFail)
            where T : class
        {
            T found = (name == "$this" ? element as T : element.FindName(name) as T) ?? element.GetResource<T>(name);
            if (found == null && shouldFail) throw new CaliburnException(
                    string.Format("Could not locate {0} with name {1}.", typeof(T).Name, name)
                    );
            return found;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Finds an element by name or fails.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static T FindNameOrFail<T>(this FrameworkContentElement element, string name)
            where T : class
        {
            T found = name == "$this" ? element as T : element.FindName(name) as T;
            if (found == null) found = element.GetResource<T>(name);
            if (found == null) throw new CaliburnException(
                    string.Format("Could not locate {1} with name {0}.", typeof(T).Name, name)
                    );
            return found;
        }
#endif
    }
}