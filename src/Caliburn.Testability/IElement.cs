namespace Caliburn.Testability
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a dependency object.
    /// </summary>
    public interface IElement
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        /// 
        string Name { get; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        IEnumerable<IElement> GetChildren(ElementEnumeratorSettings settings);

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        void Accept(IElementVisitor visitor);
    }
}