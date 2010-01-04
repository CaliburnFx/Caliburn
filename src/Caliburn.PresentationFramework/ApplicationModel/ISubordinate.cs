namespace Caliburn.PresentationFramework.ApplicationModel
{
    using Screens;

    /// <summary>
    /// A model that is owned by an <see cref="IScreen"/>.
    /// </summary>
    public interface ISubordinate
    {
        /// <summary>
        /// Gets the <see cref="IScreen"/> that owns this instance.
        /// </summary>
        /// <value>The master.</value>
        IScreen Master { get; }
    }
}