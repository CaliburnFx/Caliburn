namespace Tests.Caliburn.Testability.ChangeNotificationSamples
{
    public class NotificationOnAllProperties : ChangeNotificationBase
    {
        private bool @bool;
        private int @int;
        private object @object;
        private string @string;

        public object Object
        {
            get { return @object; }
            set
            {
                @object = value;
                RaisePropertyChanged("Object");
            }
        }

        public string String
        {
            get { return @string; }
            set
            {
                @string = value;
                RaisePropertyChanged("String");
            }
        }

        public int Int
        {
            get { return @int; }
            set
            {
                @int = value;
                RaisePropertyChanged("Int");
            }
        }

        public bool Bool
        {
            get { return @bool; }
            set
            {
                @bool = value;
                RaisePropertyChanged("Bool");
            }
        }
    }
}