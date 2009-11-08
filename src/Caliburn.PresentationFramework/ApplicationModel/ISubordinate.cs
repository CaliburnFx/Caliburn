namespace Caliburn.PresentationFramework.ApplicationModel
{
    /// <summary>
    /// A model that is owned by an <see cref="IPresenter"/>.
    /// </summary>
    public interface ISubordinate
    {
        /// <summary>
        /// Gets the <see cref="IPresenter"/> that owns this instance.
        /// </summary>
        /// <value>The master.</value>
        IPresenter Master { get; }
    }
}