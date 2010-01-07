#if SILVRLIGHT_30 || SILVERLIGHT_40

namespace Caliburn.PresentationFramework.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using Core;
    using Microsoft.Practices.ServiceLocation;

    public class DefaultValidator : IValidator
    {
        private readonly IServiceLocator _serviceLocator;

        public DefaultValidator(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

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

        private IEnumerable<IValidationError> GetValidationErrors(object instance, PropertyInfo property)
        {
            var context = new ValidationContext(instance, _serviceLocator, null);
            var propertyValue = property.GetValue(instance, null);
            var validators = from attribute in property.GetAttributes<ValidationAttribute>(true)
                             where attribute.GetValidationResult(propertyValue, context) != ValidationResult.Success
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