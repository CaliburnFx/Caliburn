namespace Caliburn.Testability
{
    /// <summary>
    /// A general prupose implementation of <see cref="GeneralError"/>.
    /// </summary>
    public class GeneralError : IError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralError"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        public GeneralError(IElement item, BoundType type, string message)
        {
            Element = item;
            Type = type;
            Message = message;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        public IElement Element { get; private set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public BoundType Type { get; private set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; private set; }
    }
}