using System.ComponentModel;

namespace Tests.Caliburn.Testability.ChangeNotificationSamples
{
    public abstract class ChangeNotificationBase : INotifyPropertyChanged
    {
        public virtual event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}