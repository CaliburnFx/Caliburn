namespace Caliburn.PresentationFramework.ApplicationModel
{
    /// <summary>
    /// An <see cref="IPresenter"/> that is aware of its position within a hierarchy.
    /// </summary>
    public interface IPresenterNode : IPresenter
    {
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        IPresenterHost Parent { get; set; }
    }
}