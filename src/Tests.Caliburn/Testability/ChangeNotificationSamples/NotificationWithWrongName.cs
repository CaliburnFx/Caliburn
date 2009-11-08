namespace Tests.Caliburn.Testability.ChangeNotificationSamples
{
    public class NotificationWithWrongName : ChangeNotificationBase
    {
        private string _string;

        public string RaisesWrongNotification
        {
            get { return _string; }
            set
            {
                _string = value;
                RaisePropertyChanged("Oops!");
            }
        }
    }
}