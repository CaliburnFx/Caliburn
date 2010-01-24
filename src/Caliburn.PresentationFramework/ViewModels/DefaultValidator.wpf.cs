#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using Core;

    /// <summary>
    /// The default implemenation of <see cref="IValidator"/>.
    /// </summary>
    public class DefaultValidator : IValidator
    {
        /// <summary>
        /// Indicates whether the specified property should be validated.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        /// true if should be validated; otherwise false
        /// </returns>
        public bool ShouldValidate(PropertyInfo property)
        {
            return property.GetAttributes<ValidationAttribute>(true).Any();
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

        private static IEnumerable<IValidationError> GetValidationErrors(object instance, PropertyInfo property)
        {
            var validators = from attribute in property.GetAttributes<ValidationAttribute>(true)
                             where !attribute.IsValid(property.GetValue(instance, null))
                             select new DefaultValidationError(
                                 instance,
                                 property.Name,
                                 attribute.FormatErrorMessage(property.Name)
                                 );

            return validators.OfType<IValidationError>();
        }
    }
}

#endif