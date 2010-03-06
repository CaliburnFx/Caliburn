namespace Caliburn.Core.Invocation
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Helper methods for generic event wirups.
    /// </summary>
    public static class EventHelper
    {
        /// <summary>
        /// Wires the event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="event">The event.</param>
        /// <param name="handler">The handler.</param>
        public static void WireEvent(object sender, EventInfo @event, Action<object, EventArgs> handler)
        {
            @event.AddEventHandler(
                sender,
                Delegate.CreateDelegate(@event.EventHandlerType, handler.Target, handler.Method)
                );
        }

        /// <summary>
        /// Unwires the event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="event">The event.</param>
        /// <param name="handler">The handler.</param>
        public static void UnwireEvent(object sender, EventInfo @event, Action<object, EventArgs> handler)
        {
            @event.RemoveEventHandler(
                sender,
                Delegate.CreateDelegate(@event.EventHandlerType, handler.Target, handler.Method)
                );
        }
    }
}