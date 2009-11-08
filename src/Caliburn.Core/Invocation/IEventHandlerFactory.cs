namespace Caliburn.Core.Invocation
{
    using System.Reflection;

    /// <summary>
    /// A service capable of creating instances of <see cref="IEventHandler"/> for an instance and a given event.
    /// </summary>
    public interface IEventHandlerFactory
    {
        /// <summary>
        /// Wires an event handler to the sender for the specified event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>The event handler.</returns>
        IEventHandler Wire(object sender, string eventName);

        /// <summary>
        /// Wires an event handler to the sender for the specified event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventInfo">The event info.</param>
        /// <returns>The event handler.</returns>
        IEventHandler Wire(object sender, EventInfo eventInfo);
    }
}