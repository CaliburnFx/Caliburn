#if SILVERLIGHT_30 || SILVERLIGHT_40

namespace Caliburn.PresentationFramework.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using Core;
    using Core.Behaviors;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The default implementation of <see cref="IValidator"/>.
    /// </summary>
    public class DefaultValidator : IValidator
    {
        private readonly IServiceLocator _serviceLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValidator"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public DefaultValidator(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        /// <summary>
        /// Indicates whether the specified property should be validated.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        /// true if should be validated; otherwise false
        /// </returns>
        public bool ShouldValidate(PropertyInfo property)
        {
            return EnsureProperty(property)
                .GetAttributes<ValidationAttribute>(true).Any();
        }

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>The validation errors.</returns>
        public IEnumerable<IValidationError> Validate(object instance)
        {
            return from property in instance.GetType().GetProperties()
                   from error in GetValidationErrors(instance, property)
                   select error;
        }

        /// <summary>
        /// Validates the specified property on the instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The validation errors.</returns>
        public IEnumerable<IValidationError> Validate(object instance, string propertyName)
        {
            var property = instance.GetType().GetProperty(propertyName);
            return GetValidationErrors(instance, property);
        }

        private IEnumerable<IValidationError> GetValidationErrors(object instance, PropertyInfo property)
        {
            var context = new ValidationContext(instance, _serviceLocator, null);
            var validators = from attribute in EnsureProperty(property)
                                 .GetAttributes<ValidationAttribute>(true)
                             where attribute.GetValidationResult(property.GetValue(instance, null), context) != ValidationResult.Success
                             select new DefaultValidationError(
                                 instance,
                                 property.Name,
                                 attribute.FormatErrorMessage(property.Name)
                                 );

            return validators.OfType<IValidationError>();
        }

        private static PropertyInfo EnsureProperty(PropertyInfo info)
        {
            if (typeof(IProxy).IsAssignableFrom(info.DeclaringType))
                return info.DeclaringType.BaseType.GetProperty(info.Name) ?? info;
            return info;
        }
    }
}

#endif