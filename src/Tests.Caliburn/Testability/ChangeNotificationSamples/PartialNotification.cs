namespace Tests.Caliburn.Testability.ChangeNotificationSamples
{
    public class PartialNotification : ChangeNotificationBase
    {
        private string _notification;
        public string NoNotification { get; set; }

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