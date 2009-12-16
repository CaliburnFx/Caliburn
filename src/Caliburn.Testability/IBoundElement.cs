namespace Caliburn.Testability
{
    public interface IBoundElement : IElement
    {
        /// <summary>
        /// Gets the type the element is bound to.
        /// </summary>
        /// <value>The type.</value>
        BoundType Type { get; }
    }
}