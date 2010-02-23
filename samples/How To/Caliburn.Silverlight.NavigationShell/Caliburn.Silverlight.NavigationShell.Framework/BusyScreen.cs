namespace Caliburn.Silverlight.NavigationShell.Framework
{
    using PresentationFramework.Screens;

    public class BusyScreen : Screen
    {
        private string _message = "Loading...";

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }
    }
}