#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using Core.Metadata;

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
            var helper = messageHandler.GetMetadata<AutoCheckAvailabilityHelper>();
            if(helper != null) return;

            helper = new AutoCheckAvailabilityHelper(messageHandler);
            messageHandler.AddMetadata(helper);
        }

        /// <summary>
        /// Makes the filter aware of the <see cref="IMessageTrigger"/>.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        /// <param name="trigger">The trigger.</param>
        public void MakeAwareOf(IRoutedMessageHandler messageHandler, IMessageTrigger trigger)
        {
            var helper = messageHandler.GetMetadata<AutoCheckAvailabilityHelper>();
            if(helper == null) return;

            helper.MakeAwareOf(trigger);
        }

        private class AutoCheckAvailabilityHelper : IMetadata
        {
            private readonly IRoutedMessageHandler _messageHandler;
            private readonly IList<IMessageTrigger> _triggersToNotify;

            //HACK: prevents the handler being garbage collected
            //see: http://msdn.microsoft.com/en-us/library/system.windows.input.commandmanager.requerysuggested.aspx
            private readonly EventHandler _handlerReference;

            internal AutoCheckAvailabilityHelper(IRoutedMessageHandler messageHandler)
            {
                _messageHandler = messageHandler;
                _triggersToNotify = new List<IMessageTrigger>();
                _handlerReference = CommandManagerRequerySuggested;

                CommandManager.RequerySuggested += _handlerReference;
            }

            private void CommandManagerRequerySuggested(object sender, EventArgs e)
            {
                foreach(var messageTrigger in _triggersToNotify)
                {
                    _messageHandler.UpdateAvailability(messageTrigger);
                }
            }

            internal void MakeAwareOf(IMessageTrigger trigger)
            {
                if(!_triggersToNotify.Contains(trigger))
                    _triggersToNotify.Add(trigger);
            }
        }
    }
}

#endif