namespace Tests.Caliburn.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using global::Caliburn.PresentationFramework;

    public class FakeMessage : IRoutedMessage
    {
        public IInteractionNode InitializeCalledWith;
        public Action InvalidatedHandler;
        private IInteractionNode _source;
        private FreezableCollection<Parameter> _parameters = new FreezableCollection<Parameter>();

        public IAvailabilityEffect AvailabilityEffect { get; set; }

        public IInteractionNode Source
        {
            get { return _source; }
        }

        public FreezableCollection<Parameter> Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public void Initialize(IInteractionNode node)
        {
            _source = node;
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