namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System;
    using System.Reflection;
    using System.Windows;
    using Conventions;
    using Core;
    using Core.InversionOfControl;

    /// <summary>
    /// Extensions related to routed UI messaging.
    /// </summary>
    public static class MessagingExtensions
    {
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
            var source = element.FindNameExhaustive<DependencyObject>(elementName, false);
            if (source == null)
                return;

            if (!string.IsNullOrEmpty(path))
            {
                var sourceType = source.GetType();
                var property = sourceType.GetProperty(path);

                EventInfo changeEvent = null;

                if (path == "SelectedItem")
                    changeEvent = sourceType.GetEvent("SelectionChanged");
                if (changeEvent == null)
                    changeEvent = sourceType.GetEvent(path + "Changed");
                if (changeEvent == null)
                    WireToDefaultEvent(parameter, sourceType, source, property);
                else parameter.Wire(source, changeEvent, () => property.GetValue(source, null));
            }
            else WireToDefaultEvent(parameter, source.GetType(), source, null);
        }

        static void WireToDefaultEvent(Parameter parameter, Type type, DependencyObject source, PropertyInfo property)
        {
            var defaults = IoC.Get<IConventionManager>()
                .GetElementConvention(type);

            if (defaults == null)
                throw new CaliburnException(
                    "Insuficient information provided for wiring action parameters.  Please set interaction defaults for " + type.FullName
                    );

            var eventInfo = type.GetEvent(defaults.EventName);

            if (property == null)
                parameter.Wire(source, eventInfo, () => defaults.GetValue(source));
            else parameter.Wire(source, eventInfo, () => property.GetValue(source, null));
        }
    }
}