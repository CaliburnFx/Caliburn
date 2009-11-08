namespace Caliburn.Testability
{
    /// <summary>
    /// Implemented by classes capable of visiting instances of <see cref="IElement"/>.
    /// </summary>
    public interface IElementVisitor
    {
        /// <summary>
        /// Gets a value indicating whether the visitor wants the enumerator to stop visiting elements.
        /// </summary>
        /// <value><c>true</c> if visiting should stop; otherwise, <c>false</c>.</value>
        bool ShouldStopVisiting { get; }

        /// <summary>
        /// Prepares the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        void Prepare(ElementEnumeratorSettings settings);

        /// <summary>
        /// Visits the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        void Visit(DependencyObjectElement element);

        /// <summary>
        /// Visits the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        void Visit(StyleElement element);

        /// <summary>
        /// Visits the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        void Visit(GroupStyleElement element);

        /// <summary>
        /// Visits the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        void Visit(DataTemplateElement element);

        /// <summary>
        /// Visits the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        void Visit(ControlTemplateElement element);
    }
}