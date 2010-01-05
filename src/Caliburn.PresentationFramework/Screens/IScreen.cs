namespace Caliburn.PresentationFramework.Screens
{
    using System.ComponentModel;

    /// <summary>
    /// Implemented by screens.
    /// </summary>
    public interface IScreen : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        string DisplayName { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Determines whether this instance can shutdown.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can shutdown; otherwise, <c>false</c>.
        /// </returns>
        bool CanShutdown();

        /// <summary>
        /// Shuts down this instance.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Activates this instance.
        /// </summary>
        void Activate();

        /// <summary>
        /// Deactivates this instance.
        /// </summary>
        void Deactivate();
    }

    /// <summary>
    /// A screen with a subject.
    /// </summary>
    /// <typeparam name="T">The subject's type.</typeparam>
    public interface IScreen<T> : IScreen
    {
        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>The subject.</value>
        T Subject { get; }

        /// <summary>
        /// Configures the screen with the subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>Self</returns>
        IScreen<T> WithSubject(T subject);
    }
}