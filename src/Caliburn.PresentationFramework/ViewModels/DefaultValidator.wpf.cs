#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using Core;

    public class DefaultValidator : IValidator
    {
        public IEnumerable<IValidationError> Validate(object instance)
        {
            return from property in instance.GetType().GetProperties()
                   from error in GetValidationErrors(instance, property)
                   select error;
        }

        public IEnumerable<IValidationError> Validate(object instance, string propertyName)
        {
            var property = instance.GetType().GetProperty(propertyName);
            return GetValidationErrors(instance, property);
        }

        private static IEnumerable<IValidationError> GetValidationErrors(object instance, PropertyInfo property)
        {
            var propertyValue = property.GetValue(instance, null);
            var validators = from attribute in property.GetAttributes<ValidationAttribute>(true)
                             where !attribute.IsValid(propertyValue)
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