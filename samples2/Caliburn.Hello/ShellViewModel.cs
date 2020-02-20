using Caliburn.PresentationFramework;
using System.Windows;

namespace Caliburn.Hello
{
    public class ShellViewModel : PropertyChangedBase
    {
        string name;

        public string NameIs
        {
            get { return name; }
            set
            {
                name = value;
                NotifyOfPropertyChange(() => NameIs);
                NotifyOfPropertyChange(() => CanSayHello);
            }
        }

        public bool CanSayHello
        {
            get { return !string.IsNullOrWhiteSpace(NameIs); }
        }

        public void SayHello()
        {
            MessageBox.Show(string.Format("Hello {0}!", NameIs)); //Don't do this in real life :)
        }
    }
}
