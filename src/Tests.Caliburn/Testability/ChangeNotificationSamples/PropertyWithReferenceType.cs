namespace Tests.Caliburn.Testability.ChangeNotificationSamples
{
    public class PropertyWithReferenceType : ChangeNotificationBase
    {
        private NoNotificationNecessary someField;

        public NoNotificationNecessary SomeField
        {
            get { return someField; }
            set
            {
                someField = value;
                RaisePropertyChanged("SomeField");
            }
        }
    }
}