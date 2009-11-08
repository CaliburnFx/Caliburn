namespace Caliburn.Testability
{
    /// <summary>
    /// Represents a validation error.
    /// </summary>
    public interface IError
    {
        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        IElement Element { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        BoundType Type { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        string Message { get; }
    }
}