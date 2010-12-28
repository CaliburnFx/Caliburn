namespace Caliburn.PresentationFramework.Screens
{
    using System;

    /// <summary>
    /// EventArgs sent during activation.
    /// </summary>
    public class ActivationEventArgs : EventArgs
    {
        /// <summary>
        /// Inidicates whether the sender was initialized in addition to being activated.
        /// </summary>
        public bool WasInitialized { get; set; }
    }
}