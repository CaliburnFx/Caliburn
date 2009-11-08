namespace Tests.Caliburn.Testability.ChangeNotificationSamples
{
    public class PropertyWithReferenceType : ChangeNotificationBase
    {
        private NoNotificationNecessary _someField;

        public NoNotificationNecessary SomeField
        {
            get { return _someField; }
            set
            {
                _someField = value;
                RaisePropertyChanged("SomeField");
            }
        }
    }
}