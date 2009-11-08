namespace Tests.Caliburn.Testability.ChangeNotificationSamples
{
    public class NotificationOnAllProperties : ChangeNotificationBase
    {
        private bool _bool;
        private int _int;
        private object _object;
        private string _string;

        public object Object
        {
            get { return _object; }
            set
            {
                _object = value;
                RaisePropertyChanged("Object");
            }
        }

        public string String
        {
            get { return _string; }
            set
            {
                _string = value;
                RaisePropertyChanged("String");
            }
        }

        public int Int
        {
            get { return _int; }
            set
            {
                _int = value;
                RaisePropertyChanged("Int");
            }
        }

        public bool Bool
        {
            get { return _bool; }
            set
            {
                _bool = value;
                RaisePropertyChanged("Bool");
            }
        }
    }
}