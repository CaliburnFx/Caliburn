namespace Tests.Caliburn.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using global::Caliburn.PresentationFramework.RoutedMessaging;

    public class FakeMessage : IRoutedMessage
    {
        public IInteractionNode InitializeCalledWith;
        public Action InvalidatedHandler;
        IInteractionNode source;
        FreezableCollection<Parameter> parameters = new FreezableCollection<Parameter>();

        public IAvailabilityEffect AvailabilityEffect { get; set; }

        public IInteractionNode Source
        {
            get { return source; }
        }

        public FreezableCollection<Parameter> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        public void Initialize(IInteractionNode node)
        {
            source = node;
            InitializeCalledWith = node;
        }

        public bool RelatesTo(object potentialTarget)
        {
            return true;
        }

        public event Action Invalidated
        {
            add { InvalidatedHandler = value; }
            remove { InvalidatedHandler = null; }
        }

        public bool Equals(IRoutedMessage other)
        {
            return ReferenceEquals(this, other);
        }

        public IEnumerable<IRoutedMessageHandler> GetDefaultHandlers(IInteractionNode node)
        {
            yield break;
        }
    }
}