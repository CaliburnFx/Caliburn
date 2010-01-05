namespace Caliburn.PresentationFramework.Screens
{
    /// <summary>
    /// An <see cref="IScreen"/> that is aware of its position within a hierarchy.
    /// </summary>
    public interface IHierarchicalScreen : IScreen
    {
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        IScreenCollection Parent { get; set; }
    }
}