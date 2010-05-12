namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Actions;
    using Core.Validation;
    using RoutedMessaging;
    using ViewModels;
    using Views;

    public abstract class ViewConventionBase
    {
        protected static IMessageBinder MessageBinder;
        protected static IViewModelDescriptionFactory ViewModelDescriptionFactory;
        protected static IValidator Validator;

        /// <summary>
        /// Initializes the specified message binder.
        /// </summary>
        /// <param name="messageBinder">The message binder.</param>
        /// <param name="viewModelDescriptionFactory">The view model description factory.</param>
        /// <param name="validator">The validator.</param>
        public static void Initialize(IMessageBinder messageBinder, IViewModelDescriptionFactory viewModelDescriptionFactory, IValidator validator)
        {
            MessageBinder = messageBinder;
            ViewModelDescriptionFactory = viewModelDescriptionFactory;
            Validator = validator;
        }
    }

    /// <summary>
    /// A base class for binding conventions.
    /// </summary>
    public abstract class ViewConventionBase<T> : ViewConventionBase, IViewConvention<T>
    {
        /// <summary>
        /// Tries to creates the application of the convention.
        /// </summary>
        /// <param name="conventionManager">The convention manager.</param>
        /// <param name="description">The description.</param>
        /// <param name="element">The element.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        /// The convention application, or null if not applicable
        /// </returns>
        public abstract IViewApplicable TryCreateApplication(IConventionManager conventionManager, IViewModelDescription description, IElementDescription element, T target);

        /// <summary>
        /// Determines the property path.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <returns></returns>
        protected static string DeterminePropertyPath(string fullName)
        {
            return fullName.Replace('_', '.');
        }

        /// <summary>
        /// Gets the boud property.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        protected static PropertyInfo GetBoundProperty(PropertyInfo info, string path)
        {
            var parts = path.Split(new[] {"."}, StringSplitOptions.RemoveEmptyEntries);

            if(parts.Length == 1)
                return string.Compare(path, info.Name, StringComparison.CurrentCultureIgnoreCase) == 0 ? info : null;

            for(int i = 1; i < parts.Length; i++)
            {
                info = info.PropertyType.GetProperty(parts[i]);
                if(info == null) break;
            }

            return info;
        }

        /// <summary>
        /// Tries to generate a new property and property path based on a predicate.
        /// </summary>
        /// <param name="originalProperty">The original property.</param>
        /// <param name="originalPropertyPath">The original property path.</param>
        /// <param name="newPropertyPath">The new property path.</param>
        /// <param name="newProperty">The new property.</param>
        /// <param name="deriveBaseName">A function that derives the base property name.</param>
        /// <param name="predicate">A predicate that matches properties.</param>
        /// <returns>True if a match was found, false otherwise.</returns>
        protected static bool TryGetByPattern(PropertyInfo originalProperty, string originalPropertyPath, out string newPropertyPath, out PropertyInfo newProperty, Func<string, string> deriveBaseName, Func<PropertyInfo, string, bool> predicate)
        {
            var index = originalPropertyPath.LastIndexOf(".");
            var subPath = index == -1
                              ? originalPropertyPath
                              : originalPropertyPath.Substring(0, index);

            var subProperty = GetBoundProperty(originalProperty, subPath);
            var singularName = index == -1
                                   ? deriveBaseName(originalPropertyPath)
                                   : deriveBaseName(originalPropertyPath.Substring(index + 1));

            var found = (index == -1 ? originalProperty.ReflectedType : subProperty.PropertyType).GetProperties()
                .FirstOrDefault(x => predicate(x, singularName));

            newProperty = found;

            if (newProperty == null)
            {
                newPropertyPath = null;
                return false;
            }

            if (index == -1)
                newPropertyPath = newProperty.Name;
            else newPropertyPath = subPath + "." + newProperty.Name;

            return true;
        }

        /// <summary>
        /// Creates the action message.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The message.</returns>
        protected static string CreateActionMessage(IAction action)
        {
            var message = action.Name;

            if (action.Requirements.Count > 0)
            {
                message += "(";

                foreach (var requirement in action.Requirements)
                {
                    var paramName = requirement.Name;
                    var specialValue = "$" + paramName;

                    if (MessageBinder.IsSpecialValue(specialValue))
                        paramName = specialValue;

                    message += paramName + ",";
                }

                message = message.Remove(message.Length - 1, 1);

                message += ")";
            }

            return message;
        }

        /// <summary>
        /// Indicates whether the specified property should be violated.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        protected virtual bool ShouldValidate(PropertyInfo property)
        {
#if SILVERLIGHT_20
            return false;   
#else
            return Validator.ShouldValidate(property);
#endif
        }
    }
}