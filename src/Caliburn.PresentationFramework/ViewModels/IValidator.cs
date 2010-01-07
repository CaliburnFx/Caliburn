namespace Caliburn.PresentationFramework.ViewModels
{
    using System.Collections.Generic;

    public interface IValidator
    {
        IEnumerable<IValidationError> Validate(object instance);
        IEnumerable<IValidationError> Validate(object instance, string propertyName);
    }
}