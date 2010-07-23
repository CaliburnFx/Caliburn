namespace Caliburn.PresentationFramework.Screens
{
    using System;

    /// <summary>
    /// Denotes an instance which requires deactivation.
    /// </summary>
    public interface IDeactivate
    {
        /// <summary>
        /// Raised before deactivation.
        /// </summary>
        event EventHandler<DeactivationEventArgs> AttemptingDeactivation;

        /// <summary>
        /// Deactivates this instance.
        /// </summary>
        /// <param name="close">Inidcates whether or not this instance is being closed.</param>
        void Deactivate(bool close);

        /// <summary>
        /// Raised after deactivation.
        /// </summary>
        event EventHandler<DeactivationEventArgs> Deactivated;
    }

    /// <summary>
    /// EventArgs sent during deactivation.
    /// </summary>
    public class DeactivationEventArgs : EventArgs
    {
        /// <summary>
        /// Indicates whether the sender was closed in addition to being deactivated.
        /// </summary>
        public bool WasClosed;
    }
}