namespace Caliburn.EventAggregation {
    using System.ComponentModel.Composition;
    using PresentationFramework;
    using PresentationFramework.ApplicationModel;

    [Export(typeof(IShell))]
    public class ShellViewModel : PropertyChangedBase, IShell, IHandle<object> {
        string lastEvent = "No Events Yet";

        [ImportingConstructor]
        public ShellViewModel(LeftViewModel left, RightViewModel right, IEventAggregator events) {
            Left = left;
            Right = right;

            events.Subscribe(this);
        }

        public LeftViewModel Left { get; private set; }
        public RightViewModel Right { get; private set; }

        public string LastEvent {
            get { return lastEvent; }
        }

        public void Handle(object message) {
            lastEvent = "Last Event: " + message;
            NotifyOfPropertyChange(() => LastEvent);
        }
    }
}