namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;

    /// <summary>
    /// Represents an <see cref="IPresenter"/> that hosts other instances of <see cref="IPresenter"/>.
    /// </summary>
    public interface IPresenterHost : IPresenter
    {
        /// <summary>
        /// Gets the presenters that are currently managed.
        /// </summary>
        /// <value>The presenters.</value>
        IObservableCollection<IPresenter> Presenters { get; }

        /// <summary>
        /// Opens the specified presenter.
        /// </summary>
        /// <param name="presenter">The presenter.</param>
        /// <param name="completed">Called when the open action is finished.</param>
        void Open(IPresenter presenter, Action<bool> completed);

        /// <summary>
        /// Shuts down the specified presenter.
        /// </summary>
        /// <param name="presenter">The presenter.</param>
        /// <param name="completed">Called when the open action is finished.</param>
        void Shutdown(IPresenter presenter, Action<bool> completed);
    }
}