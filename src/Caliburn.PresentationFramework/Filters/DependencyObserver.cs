namespace Caliburn.PresentationFramework.Filters
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Core.Metadata;

    /// <summary>
    /// Metadata which can be used to trigger availability changes in triggers based on <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    public class DependencyObserver : IMetadata
    {
        private readonly IRoutedMessageHandler _messageHandler;
        private readonly Dictionary<string, IList<IMessageTrigger>> _triggersToNotify;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyObserver"/> class.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        /// <param name="notifier">The notifier.</param>
        public DependencyObserver(IRoutedMessageHandler messageHandler, INotifyPropertyChanged notifier)
        {
            _messageHandler = messageHandler;
            _triggersToNotify = new Dictionary<string, IList<IMessageTrigger>>();

            notifier.PropertyChanged += Notifier_PropertyChanged;
        }

        /// <summary>
        /// Makes the metadata aware of the relationship between an <see cref="IMessageTrigger"/> and its dependencies.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <param name="dependencies">The dependencies.</param>
        public void MakeAwareOf(IMessageTrigger trigger, IEnumerable<string> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                IList<IMessageTrigger> triggers;

                if(!_triggersToNotify.TryGetValue(dependency, out triggers))
                {
                    triggers = new List<IMessageTrigger>();
                    _triggersToNotify[dependency] = triggers;
                }

                if(!triggers.Contains(trigger))
                    triggers.Add(trigger);
            }
        }

        private void Notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IList<IMessageTrigger> triggers;

            if(!_triggersToNotify.TryGetValue(e.PropertyName, out triggers)) 
                return;

            foreach (var messageTrigger in triggers)
            {
                _messageHandler.UpdateAvailability(messageTrigger);
            }
        }
    }
}