namespace Caliburn.ShellFramework.Services
{
    using PresentationFramework.Screens;

    public class LoadScreenViewModel : Screen
    {
        private string _message;
        private int _loadDepth;
        private readonly object _lock = new object();

        public int LoadDepth
        {
            get { return _loadDepth; }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        public void StartLoading(string message)
        {
            Message = string.IsNullOrEmpty(message)
                          ? "Loading..."
                          : message;

            lock (_lock)
            {
                _loadDepth++;
            }
        }

        public void StopLoading()
        {
            if (_loadDepth > 0)
            {
                lock (_lock)
                {
                    if (_loadDepth > 0)
                    {
                        _loadDepth--;
                    }
                }
            }
        }
    }
}