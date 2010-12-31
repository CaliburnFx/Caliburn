namespace Tests.Caliburn.Testability.ChangeNotificationSamples
{
    public class ParentNotification : ChangeNotificationBase
    {
        public string NoNotification { get; set; }
    }

    public class ChildNotification : ParentNotification
    {
        private string notification;

        public string Notification
        {
            get { return notification; }
            set
            {
                notification = value;
                RaisePropertyChanged("Notification");
            }
        }
    }
}