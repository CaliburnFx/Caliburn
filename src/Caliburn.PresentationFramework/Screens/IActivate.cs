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
}