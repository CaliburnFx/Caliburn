namespace Caliburn.PresentationFramework.Filters
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Core;
    using Core.Invocation;
    using RoutedMessaging;

    /// <summary>
    /// Observes a single property along a property path.
    /// </summary>
    public class SinglePropertyPathObserver : IChangeMonitorNode
    {
        readonly IRoutedMessageHandler messageHandler;
        readonly IList<IMessageTrigger> triggersToNotify = new List<IMessageTrigger>();
        readonly PropertyPathMonitor monitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SinglePropertyPathObserver"/> class.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        /// <param name="methodFactory">The method factory.</param>
        /// <param name="notifier">The notifier.</param>
        /// <param name="propertyPath">The property path.</param>
        public SinglePropertyPathObserver(IRoutedMessageHandler messageHandler, IMethodFactory methodFactory, INotifyPropertyChanged notifier, string propertyPath)
        {
            this.messageHandler = messageHandler;
            monitor = new PropertyPathMonitor(methodFactory, notifier, propertyPath, this);
        }

        /// <summary>
        /// Registers the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        public void RegisterTrigger(IMessageTrigger trigger)
        {
            if (!triggersToNotify.Contains(trigger))
                triggersToNotify.Add(trigger);
        }

        /// <summary>
        /// Raises change notification.
        /// </summary>
        public void NotifyChange()
        {
            triggersToNotify.Apply(x => messageHandler.UpdateAvailability(x));
        }

        /// <summary>
        /// The parent node.
        /// </summary>
        /// <value></value>
        public IChangeMonitorNode Parent
        {
            get { return null; }
        }

        /// <summary>
        /// Indicates whether to stop monitoring changes.
        /// </summary>
        /// <returns></returns>
        public bool ShouldStopMonitoring()
        {
            return false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (monitor != null)
                monitor.Dispose();
        } 
    }
}