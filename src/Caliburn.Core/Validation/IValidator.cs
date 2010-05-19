namespace Caliburn.Core.Validation
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// A service that validates the state of classes and their properties.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Inidcates whether the specified property should be validated.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>true if should be validated; otherwise false</returns>
        bool ShouldValidate(PropertyInfo property);

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>The validation errors.</returns>
        IEnumerable<IError> Validate(object instance);

        /// <summary>
        /// Validates the specified property on the instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The validation errors.</returns>
        IEnumerable<IError> Validate(object instance, string propertyName);
    }
}