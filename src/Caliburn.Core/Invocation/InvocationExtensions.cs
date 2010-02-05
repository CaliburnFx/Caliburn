namespace Caliburn.Core.Invocation
{
    /// <summary>
    /// Hosts invocation related extension methods.
    /// </summary>
    public static class InvocationExtensions
    {
        /// <summary>
        /// Wires an event handler to the sender for the specified event.
        /// </summary>
        /// <param name="eventHandlerFactory">The event handler factory.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>The event handler.</returns>
        public static IEventHandler Wire(this IEventHandlerFactory eventHandlerFactory, object sender, string eventName)
        {
            var eventInfo = sender.GetType().GetEvent(eventName);

            if (eventInfo == null)
                throw new CaliburnException(
                    string.Format("The event '{0}' does not exist on '{1}'.", eventName, sender.GetType().FullName)
                    );

            return eventHandlerFactory.Wire(sender, eventInfo);
        }
    }
}