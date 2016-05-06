namespace Caliburn.Core.Validation
{
    /// <summary>
    /// Represents a validation error.
    /// </summary>
    public interface IError
    {
        /// <summary>
        /// Gets the name of the invalid property.
        /// </summary>
        /// <value>The name of the property.</value>
        string Key { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>The message.</value>
        string Message { get; }
    }
}