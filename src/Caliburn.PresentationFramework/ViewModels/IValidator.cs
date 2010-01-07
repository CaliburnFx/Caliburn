namespace Caliburn.PresentationFramework.ViewModels
{
    using System.Collections.Generic;

    /// <summary>
    /// A service that validates the state of classes and their properties.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>The validation errors.</returns>
        IEnumerable<IValidationError> Validate(object instance);

        /// <summary>
        /// Validates the specified property on the instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The validation errors.</returns>
        IEnumerable<IValidationError> Validate(object instance, string propertyName);
    }
}