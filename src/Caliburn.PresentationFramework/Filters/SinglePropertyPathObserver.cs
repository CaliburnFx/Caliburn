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
        private IRoutedMessageHandler _messageHandler;
        private IList<IMessageTrigger> _triggersToNotify = new List<IMessageTrigger>();
        private PropertyPathMonitor _monitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SinglePropertyPathObserver"/> class.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        /// <param name="methodFactory">The method factory.</param>
        /// <param name="notifier">The notifier.</param>
        /// <param name="propertyPath">The property path.</param>
        public SinglePropertyPathObserver(IRoutedMessageHandler messageHandler, IMethodFactory methodFactory, INotifyPropertyChanged notifier, string propertyPath)
        {
            _messageHandler = messageHandler;
            _monitor = new PropertyPathMonitor(methodFactory, notifier, propertyPath, this);
        }

        /// <summary>
        /// Registers the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        public void RegisterTrigger(IMessageTrigger trigger)
        {
            if (!_triggersToNotify.Contains(trigger))
                _triggersToNotify.Add(trigger);
        }

        /// <summary>
        /// Raises change notification.
        /// </summary>
        public void NotifyChange()
        {
            _triggersToNotify.Apply(x => _messageHandler.UpdateAvailability(x));
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
            if (_monitor != null)
                _monitor.Dispose();
        } 
    }
}