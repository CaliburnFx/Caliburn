namespace Caliburn.PresentationFramework.Screens
{
    using System;

    /// <summary>
    /// An <see cref="IScreen"/> capable of conducting other screens.
    /// </summary>
    public interface IScreenConductor : IScreenHost
    {
        /// <summary>
        /// Gets or sets the active screen.
        /// </summary>
        /// <value>The active screen.</value>
        IScreen ActiveScreen { get; set; }

        /// <summary>
        /// Shuts down the active screen.
        /// </summary>
        void ShutdownActiveScreen(Action<bool> completed);
    }

    /// <summary>
    /// A generic version of <see cref="IScreenConductor"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IScreenConductor<T> : IScreenHost<T>, IScreenConductor
        where T : class, IScreen
    {
        /// <summary>
        /// Gets or sets the active screen.
        /// </summary>
        /// <value>The active screen.</value>
        new T ActiveScreen { get; set; }
    }
}