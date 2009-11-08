namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System.ComponentModel;

    /// <summary>
    /// The 'P' in MVP.
    /// </summary>
    public interface IPresenter : INotifyPropertyChanged
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
}