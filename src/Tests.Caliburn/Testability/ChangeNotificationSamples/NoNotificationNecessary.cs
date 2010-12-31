namespace Tests.Caliburn.Testability.ChangeNotificationSamples
{
    public class NoNotificationNecessary : ChangeNotificationBase
    {
        private readonly string @string = string.Empty;

        public string String
        {
            get { return @string; }
        }
    }
}