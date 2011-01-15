namespace Caliburn.EventAggregation {
    using System.ComponentModel.Composition;
    using PresentationFramework;
    using PresentationFramework.ApplicationModel;

    [Export(typeof(RightViewModel))]
    public class RightViewModel : IHandle<LeftEvent> {
        readonly IEventAggregator eventAgg;
        readonly IObservableCollection<int> events = new BindableCollection<int>();
        int count = 1;

        [ImportingConstructor]
        public RightViewModel(IEventAggregator eventAgg) {
            this.eventAgg = eventAgg;
            eventAgg.Subscribe(this);
        }

        public IObservableCollection<int> Events {
            get { return events; }
        }

        public void Handle(LeftEvent message) {
            events.Add(message.Number);
        }

        public void Publish() {
            eventAgg.Publish(new RightEvent {
                Number = count++
            });
        }
    }
}