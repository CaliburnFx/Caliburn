namespace Caliburn.PresentationFramework.Screens
{
    using System;

    /// <summary>
    /// An baseclass for implementations of <see cref="IScreenConductor"/>.
    /// </summary>
    /// <typeparam name="T">A type of <see cref="IScreen"/>.</typeparam>
    public abstract class ScreenConductorBase<T> : ScreenCollectionBase<T>, IScreenConductor<T>
        where T : class, IScreen
    {
        /// <summary>
        /// Gets or sets the active screen.
        /// </summary>
        /// <value>The active screen.</value>
        public abstract T ActiveScreen { get; set; }

        /// <summary>
        /// Gets or sets the active screen.
        /// </summary>
        /// <value>The active screen.</value>
        IScreen IScreenConductor.ActiveScreen
        {
            get { return ActiveScreen; }
            set { ActiveScreen = (T)value; }
        }

        /// <summary>
        /// Shuts down the active screen.
        /// </summary>
        /// <param name="completed"></param>
        public abstract void ShutdownActiveScreen(Action<bool> completed);
    }
}