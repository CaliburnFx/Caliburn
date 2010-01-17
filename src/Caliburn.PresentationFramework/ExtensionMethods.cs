namespace Caliburn.PresentationFramework
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using Conventions;
    using Core;
    using Core.Invocation;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Extension methods related to the PresentationFramework classes.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Safely converts an object to a string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted string or null, if the value was null.</returns>
        internal static string SafeToString(this object value)
        {
            return value == null ? null : value.ToString();
        }

        /// <summary>
        /// Gets the parent of the dependency object.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The parent element.</returns>
        public static DependencyObject GetParent(this DependencyObject dependencyObject)
        {
#if !SILVERLIGHT
            return LogicalTreeHelper.GetParent(dependencyObject) ??
                   VisualTreeHelper.GetParent(dependencyObject);
#else
            return VisualTreeHelper.GetParent(dependencyObject);
#endif
        }

        /// <summary>
        /// Sets the data context of the dependency object.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dataContext">The data context value.</param>
        public static void SetDataContext(this DependencyObject dependencyObject, object dataContext)
        {
            var fe = dependencyObject as FrameworkElement;
            if (fe != null)
                fe.DataContext = dataContext;
#if !SILVERLIGHT
            else
            {
                var fce = dependencyObject as FrameworkContentElement;
                if (fce != null)
                    fce.DataContext = dataContext;
            }
#endif
        }

        /// <summary>
        /// Gets the data context of the depdendency object.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The data context value.</returns>
        public static object GetDataContext(this DependencyObject dependencyObject)
        {
            var fe = dependencyObject as FrameworkElement;
            if (fe != null)
                return fe.DataContext;
#if !SILVERLIGHT
            var fce = dependencyObject as FrameworkContentElement;
            if (fce != null)
                return fce.DataContext;
#endif
            throw new CaliburnException(
                string.Format(
                    "Instance {0} must be a FrameworkElement or FrameworkContentElement in order to get its DataContext property.",
                    dependencyObject.GetType().Name
                    )
                );
        }

        /// <summary>
        /// Wires the delegate to the Loaded event of the element.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="callback">The callback.</param>
        public static void OnLoad(this DependencyObject dependencyObject, RoutedEventHandler callback)
        {
            var fe = dependencyObject as FrameworkElement;
            if (fe != null)
                fe.Loaded += callback;
#if !SILVERLIGHT
            else
            {
                var fce = dependencyObject as FrameworkContentElement;
                if(fce != null)
                    fce.Loaded += callback;
            }
#endif
        }

        /// <summary>
        /// Gets the binding expression from the dependency object.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        /// <returns>The binding expression.</returns>
        public static BindingExpression GetBindingExpression(this DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            var fe = dependencyObject as FrameworkElement;
            if (fe != null)
                return fe.GetBindingExpression(dependencyProperty);
#if !SILVERLIGHT
            var fce = dependencyObject as FrameworkContentElement;
            if (fce != null)
                return fce.GetBindingExpression(dependencyProperty);
#endif
            return null;
        }

        /// <summary>
        /// Sets the binding on the dependency object.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        /// <param name="binding">The binding.</param>
        public static void SetBinding(this DependencyObject dependencyObject, DependencyProperty dependencyProperty, Binding binding)
        {
            var fe = dependencyObject as FrameworkElement;
            if (fe != null)
                fe.SetBinding(dependencyProperty, binding);
#if !SILVERLIGHT
            else
            {
                var fce = dependencyObject as FrameworkContentElement;
                if(fce != null)
                    fce.SetBinding(dependencyProperty, binding);
            }
#endif
        }

        /// <summary>
        /// Finds a child element by name.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="name">The name.</param>
        /// <returns>The found element.</returns>
        public static DependencyObject FindName(this DependencyObject parent, string name)
        {
            var fe = parent as FrameworkElement;
            if (fe != null)
                return fe.FindName(name) as DependencyObject;
#if !SILVERLIGHT
            var fce = parent as FrameworkContentElement;
            if (fce != null)
                return fce.FindName(name) as DependencyObject;
#endif
            return null;
        }

        /// <summary>
        /// Finds an element by name or fails.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element.</param>
        /// <param name="name">The name.</param>
        /// <param name="shouldFail">Indicates whether an exception should be throw if the named item is not found.</param>
        /// <returns>The found element.</returns>
        public static T FindNameExhaustive<T>(this DependencyObject element, string name, bool shouldFail)
            where T : class
        {
            T found = (name == "$this" ? element as T : element.FindName(name) as T)
                ?? element.GetResource<T>(name);

            if (found == null && shouldFail) throw new CaliburnException(
                    string.Format("Could not locate {0} with name {1}.", typeof(T).Name, name)
                    );

            return found;
        }

        /// <summary>
        /// Gets the value of the Name property of this instance.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The name.</returns>
        public static string GetName(this DependencyObject dependencyObject)
        {
            var fe = dependencyObject as FrameworkElement;
            if (fe != null) 
                return dependencyObject.GetValue(FrameworkElement.NameProperty) as string;

#if !SILVERLIGHT
            var fce = dependencyObject as FrameworkContentElement;
            if (fce != null) 
                return dependencyObject.GetValue(FrameworkContentElement.NameProperty) as string;
#endif
            return null;
        }

        /// <summary>
        /// Gets the resource by searching the hierarchy of of elements.
        /// </summary>
        /// <typeparam name="T">The type of resource.</typeparam>
        /// <param name="element">The element.</param>
        /// <param name="key">The key.</param>
        /// <returns>The resource.</returns>
        public static T GetResource<T>(this DependencyObject element, object key)
        {
            var currentElement = element;

            while (currentElement != null)
            {
                var fe = currentElement as FrameworkElement;

                if (fe != null)
                {
                    if(fe.Resources.Contains(key))
                        return (T)fe.Resources[key];
                }
#if !SILVERLIGHT
                else
                {
                    var fce = currentElement as FrameworkContentElement;
                    if(fce != null)
                    {
                        if (fce.Resources.Contains(key))
                            return (T)fce.Resources[key];
                    }
                }

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

        /// <summary>
        /// Finds the interaction defaults or fail.
        /// </summary>
        /// <param name="conventionManager">The convention manager.</param>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static IElementConvention FindElementConventionOrFail(this IConventionManager conventionManager, object element)
        {
            var type = element.GetType();
            var defaults = conventionManager.GetElementConvention(type);

            if (defaults == null)
                throw new CaliburnException(
                    string.Format("Could not locate an IElementConvention for {0}.  Please register one with the IConventionManager.", type.Name)
                    );

            return defaults;
        }

        /// <summary>
        /// Binds the specified parameter to an element's property without using databinding.
        /// Rather, event name conventions are used to wire to property changes and push updates to the parameter value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="element">The element.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="path">The path.</param>
        public static void Bind(this Parameter parameter, DependencyObject element, string elementName, string path)
        {
            if (string.IsNullOrEmpty(elementName))
                return;

            bool isLoaded = false;

            element.OnLoad(
                (s, e) =>{
                    if(isLoaded)
                        return;

                    isLoaded = true;

                    var source = element.FindNameExhaustive<object>(elementName, false);
                    if(source == null)
                        return;

                    if(!string.IsNullOrEmpty(path))
                    {
                        var sourceType = source.GetType();
                        var property = sourceType.GetProperty(path);

                        EventInfo changeEvent = null;

                        if(path == "SelectedItem")
                            changeEvent = sourceType.GetEvent("SelectionChanged");
                        if(changeEvent == null)
                            changeEvent = sourceType.GetEvent(path + "Changed");
                        if(changeEvent == null)
                            WireToDefaultEvent(parameter, sourceType, source, property);
                        else
                        {
                            ServiceLocator.Current.GetInstance<IEventHandlerFactory>().Wire(source, changeEvent)
                                .SetActualHandler(parameters =>{
                                    parameter.Value = property.GetValue(source, null);
                                });
                        }
                    }
                    else WireToDefaultEvent(parameter, source.GetType(), source, null);
                });
        }

        private static void WireToDefaultEvent(Parameter parameter, Type type, object source, PropertyInfo property)
        {
            var defaults = ServiceLocator.Current.GetInstance<IConventionManager>()
                .GetElementConvention(type);

            if (defaults == null)
                throw new CaliburnException(
                    "Insuficient information provided for wiring action parameters.  Please set interaction defaults for " + type.FullName
                    );

            if (property == null)
                ServiceLocator.Current.GetInstance<IEventHandlerFactory>().Wire(source, defaults.EventName)
                    .SetActualHandler(parameters =>{
                        parameter.Value = defaults.GetValue(source);
                    });
            else ServiceLocator.Current.GetInstance<IEventHandlerFactory>().Wire(source, defaults.EventName)
                    .SetActualHandler(parameters => {
                        parameter.Value = property.GetValue(source, null);
                    });
        }
    }
}