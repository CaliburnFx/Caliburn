namespace Tests.Caliburn.Testability.ChangeNotificationSamples
{
    public class NotificationWithWrongName : ChangeNotificationBase
    {
        private string @string;

        public string RaisesWrongNotification
        {
            get { return @string; }
            set
            {
                @string = value;
                RaisePropertyChanged("Oops!");
            }
        }
    }
}