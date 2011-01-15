namespace Caliburn.EventAggregation {
    using System.ComponentModel.Composition;
    using PresentationFramework;
    using PresentationFramework.ApplicationModel;

    [Export(typeof(LeftViewModel))]
    public class LeftViewModel : IHandle<RightEvent> {
        readonly IEventAggregator eventAgg;
        readonly IObservableCollection<int> events = new BindableCollection<int>();
        int count = 1;

        [ImportingConstructor]
        public LeftViewModel(IEventAggregator eventAgg) {
            this.eventAgg = eventAgg;
            eventAgg.Subscribe(this);
        }

        public IObservableCollection<int> Events {
            get { return events; }
        }

        public void Handle(RightEvent message) {
            events.Add(message.Number);
        }

        public void Publish() {
            eventAgg.Publish(new LeftEvent {
                Number = count++
            });
        }
    }
}