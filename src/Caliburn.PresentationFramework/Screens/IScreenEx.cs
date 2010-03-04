namespace Caliburn.PresentationFramework.Screens
{
    using ApplicationModel;
    using Core.Metadata;
    using Views;

    /// <summary>
    /// An <see cref="IScreen"/> which also implements <see cref="IMetadataContainer"/>, <see cref="IHierarchicalScreen"/>, <see cref="ILifecycleNotifier"/> and <see cref="IViewAware"/>.
    /// </summary>
    public interface IScreenEx : IMetadataContainer, IHierarchicalScreen, ILifecycleNotifier, IViewAware
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        bool IsActive { get; }

        /// <summary>
        /// Tries to close this screen.
        /// </summary>
        void Close();
    }
}