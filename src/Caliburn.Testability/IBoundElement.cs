namespace Caliburn.Testability
{
    /// <summary>
    /// Represents an element which can be bound.
    /// </summary>
    public interface IBoundElement : IElement
    {
        /// <summary>
        /// Gets the type the element is bound to.
        /// </summary>
        /// <value>The type.</value>
        BoundType Type { get; }
    }
}