namespace Tests.Caliburn.Testability.ChangeNotificationSamples
{
    public class ParentNotification : ChangeNotificationBase
    {
        public string NoNotification { get; set; }
    }

    public class ChildNotification : ParentNotification
    {
        private string _notification;

        public string Notification
        {
            get { return _notification; }
            set
            {
                _notification = value;
                RaisePropertyChanged("Notification");
            }
        }
    }
}