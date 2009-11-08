namespace Tests.Caliburn.Testability.ChangeNotificationSamples
{
    public class NoNotificationNecessary : ChangeNotificationBase
    {
        private readonly string _string = string.Empty;

        public string String
        {
            get { return _string; }
        }
    }
}