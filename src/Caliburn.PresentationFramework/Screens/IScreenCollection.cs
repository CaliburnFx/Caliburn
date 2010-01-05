namespace Caliburn.PresentationFramework.Screens
{
    using System;

    /// <summary>
    /// Represents an <see cref="IScreen"/> that hosts other instances of <see cref="IScreen"/>.
    /// </summary>
    public interface IScreenCollection : IScreen
    {
        /// <summary>
        /// Gets the screens that are currently managed.
        /// </summary>
        /// <value>The presenters.</value>
        IObservableCollection<IScreen> Screens { get; }

        /// <summary>
        /// Opens the specified screen.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <param name="completed">Called when the open action is finished.</param>
        void OpenScreen(IScreen screen, Action<bool> completed);

        /// <summary>
        /// Shuts down the specified screen.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <param name="completed">Called when the open action is finished.</param>
        void ShutdownScreen(IScreen screen, Action<bool> completed);
    }

    public interface IScreenCollection<T> : IScreenCollection
        where T : class, IScreen
    {
        /// <summary>
        /// Gets the screens that are currently managed.
        /// </summary>
        /// <value>The screens.</value>
        new IObservableCollection<T> Screens { get; }

        /// <summary>
        /// Opens the specified screen.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <param name="completed">Called when the open action is finished.</param>
        void OpenScreen(T screen, Action<bool> completed);

        /// <summary>
        /// Shuts down the specified screen.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <param name="completed">Called when the open action is finished.</param>
        void ShutdownScreen(T screen, Action<bool> completed);
    }
}