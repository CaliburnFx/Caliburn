namespace Caliburn.WPF.ApplicationFramework
{
    using PresentationFramework.ApplicationModel;

    public class SettingsBase : IsolatedStorageStateManager
    {
        private const string _fileName = "Settings.xml";

        public SettingsBase()
        {
            AfterStateLoad += delegate { NotifyOfPropertyChange(""); };
        }

        public void Save()
        {
            CommitChanges(_fileName);
        }

        public void Load()
        {
            Initialize(_fileName);
        }
    }
}