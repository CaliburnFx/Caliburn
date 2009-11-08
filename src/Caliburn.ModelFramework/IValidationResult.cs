namespace Caliburn.ModelFramework
{
    /// <summary>
    /// The result of validation a model.
    /// </summary>
    public interface IValidationResult
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        string Message { get; }
    }
}