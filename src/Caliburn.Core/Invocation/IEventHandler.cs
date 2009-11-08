namespace Caliburn.Core.Invocation
{
    using System;

    /// <summary>
    /// Represents a generic method of setting an event handler.
    /// </summary>
    public interface IEventHandler
    {
        /// <summary>
        /// Sets the actual handler for the event.
        /// </summary>
        /// <param name="action">The action.</param>
        void SetActualHandler(Action<object[]> action);
    }
}