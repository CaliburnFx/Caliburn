namespace Caliburn.ModelFramework
{
    /// <summary>
    /// A base implementation of <see cref="IValidationResult"/>.
    /// </summary>
    public class SimpleValidationResult : IValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleValidationResult"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SimpleValidationResult(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; private set; }
    }
}