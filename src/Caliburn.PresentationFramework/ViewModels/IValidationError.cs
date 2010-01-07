namespace Caliburn.PresentationFramework.ViewModels
{
    /// <summary>
    /// Represents a validation error.
    /// </summary>
    public interface IValidationError
    {
        /// <summary>
        /// Gets the invalid instance.
        /// </summary>
        /// <value>The instance.</value>
        object Instance { get; }

        /// <summary>
        /// Gets the name of the invalid property.
        /// </summary>
        /// <value>The name of the property.</value>
        string PropertyName { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>The message.</value>
        string Message { get; }
    }
}