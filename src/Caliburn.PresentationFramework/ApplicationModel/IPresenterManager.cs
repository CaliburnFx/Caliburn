namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;

    /// <summary>
    /// An <see cref="IPresenter"/> capable of managing other presenters.
    /// </summary>
    public interface IPresenterManager : IPresenterHost
    {
        /// <summary>
        /// Gets or sets the current presenter.
        /// </summary>
        /// <value>The current presenter.</value>
        IPresenter CurrentPresenter { get; set; }

        /// <summary>
        /// Shuts down the current presenter.
        /// </summary>
        void ShutdownCurrent(Action<bool> completed);
    }
}