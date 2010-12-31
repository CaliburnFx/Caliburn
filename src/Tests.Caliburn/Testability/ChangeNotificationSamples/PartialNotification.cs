namespace Tests.Caliburn.Testability.ChangeNotificationSamples
{
    public class PartialNotification : ChangeNotificationBase
    {
        private string notification;
        public string NoNotification { get; set; }

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