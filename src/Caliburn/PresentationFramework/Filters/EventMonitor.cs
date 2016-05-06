namespace Caliburn.PresentationFramework.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Core.Invocation;
    using Invocation;
    using RoutedMessaging;

    /// <summary>
    /// Used to monitor XXXChanged events for properties.
    /// </summary>
    public class EventMonitor
    {
        readonly IRoutedMessageHandler messageHandler;
        readonly IList<IMessageTrigger> triggersToNotify;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventMonitor"/> class.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        /// <param name="eventInfo">The event info.</param>
        internal EventMonitor(IRoutedMessageHandler messageHandler, EventInfo eventInfo)
        {
            this.messageHandler = messageHandler;
            triggersToNotify = new List<IMessageTrigger>();

            EventHelper.WireEvent(messageHandler.Unwrap(), eventInfo, ChangedEventHandler);
        }

        /// <summary>
        /// Tries to hook the event.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns></returns>
        public static EventMonitor TryHook(IRoutedMessageHandler messageHandler, string eventName)
        {
            var target = messageHandler.Unwrap();
            var eventInfo = target.GetType().GetEvent(eventName);
            if(eventInfo == null)
                return null;
            return new EventMonitor(messageHandler, eventInfo);
        }

        /// <summary>
        /// The method that is called when the change event handler is raised.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void ChangedEventHandler(object sender, EventArgs e)
        {
            Execute.OnUIThread(() =>{
                foreach(var messageTrigger in triggersToNotify)
                {
                    messageHandler.UpdateAvailability(messageTrigger);
                }
            });
        }

        internal void MakeAwareOf(IMessageTrigger trigger)
        {
            if(!triggersToNotify.Contains(trigger))
                triggersToNotify.Add(trigger);
        }
    }
}