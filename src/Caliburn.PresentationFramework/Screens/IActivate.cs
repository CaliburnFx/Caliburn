namespace Caliburn.PresentationFramework.Screens
{
    using System;

    /// <summary>
    /// Denotes an instance which requireds activation.
    /// </summary>
    public interface IActivate
    {
        ///<summary>
        /// Indicates whether or not this instace is active.
        ///</summary>
        bool IsActive { get; }

        /// <summary>
        /// Activates this instance.
        /// </summary>
        void Activate();

        /// <summary>
        /// Raised after activation occurs.
        /// </summary>
        event EventHandler<ActivationEventArgs> Activated;
    }

    /// <summary>
    /// EventArgs sent during activation.
    /// </summary>
    public class ActivationEventArgs : EventArgs
    {
        /// <summary>
        /// Inidicates whether the sender was initialized in addition to being activated.
        /// </summary>
        public bool WasInitialized;
    }
}