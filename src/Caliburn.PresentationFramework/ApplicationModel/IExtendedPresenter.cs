namespace Caliburn.PresentationFramework.ApplicationModel
{
    using Core.Metadata;
    using ViewModels;

    /// <summary>
    /// Implements <see cref="IMetadataContainer"/>, <see cref="IPresenterNode"/>, <see cref="ILifecycleNotifier"/> and <see cref="IViewAware"/>.
    /// </summary>
    public interface IExtendedPresenter : IMetadataContainer, IPresenterNode, ILifecycleNotifier, IViewAware
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
        /// Closes this instance.
        /// </summary>
        void Close();
    }
}