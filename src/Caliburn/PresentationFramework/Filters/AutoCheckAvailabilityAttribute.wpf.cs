#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.Filters
{
    using System;
    using System.Collections.Generic;
    using Core;
    using System.Windows.Input;
    using RoutedMessaging;

    /// <summary>
    /// A filter capable of updating trigger availability based on CommandManager notifications.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AutoCheckAvailabilityAttribute : Attribute, IHandlerAware
    {
        /// <summary>
        /// Gets the priority used to order filters.
        /// </summary>
        /// <value>The order.</value>
        /// <remarks>Higher numbers are evaluated first.</remarks>
        public int Priority { get; set; }

        /// <summary>
        /// Makes the filter aware of the <see cref="IRoutedMessageHandler"/>.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        public void MakeAwareOf(IRoutedMessageHandler messageHandler)
        {
            var helper = messageHandler.Metadata.FirstOrDefaultOfType<AutoCheckAvailabilityHelper>();
            if(helper != null) return;

            helper = new AutoCheckAvailabilityHelper(messageHandler);
            messageHandler.Metadata.Add(helper);
        }

        /// <summary>
        /// Makes the filter aware of the <see cref="IMessageTrigger"/>.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        /// <param name="trigger">The trigger.</param>
        public void MakeAwareOf(IRoutedMessageHandler messageHandler, IMessageTrigger trigger)
        {
            var helper = messageHandler.Metadata.FirstOrDefaultOfType<AutoCheckAvailabilityHelper>();
            if(helper == null) return;

            helper.MakeAwareOf(trigger);
        }

        private class AutoCheckAvailabilityHelper
        {
            readonly IRoutedMessageHandler messageHandler;
            readonly IList<IMessageTrigger> triggersToNotify;

            //HACK: prevents the handler being garbage collected
            //see: http://msdn.microsoft.com/en-us/library/system.windows.input.commandmanager.requerysuggested.aspx
            readonly EventHandler handlerReference;

            internal AutoCheckAvailabilityHelper(IRoutedMessageHandler messageHandler)
            {
                this.messageHandler = messageHandler;
                triggersToNotify = new List<IMessageTrigger>();
                handlerReference = CommandManagerRequerySuggested;

                CommandManager.RequerySuggested += handlerReference;
            }

            void CommandManagerRequerySuggested(object sender, EventArgs e)
            {
                foreach(var messageTrigger in triggersToNotify)
                {
                    messageHandler.UpdateAvailability(messageTrigger);
                }
            }

            internal void MakeAwareOf(IMessageTrigger trigger)
            {
                if(!triggersToNotify.Contains(trigger))
                    triggersToNotify.Add(trigger);
            }
        }
    }
}

#endif