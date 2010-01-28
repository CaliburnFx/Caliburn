namespace Caliburn.Silverlight.NavigationShell.Shell.Controls
{
    using System.Windows.Controls;

    public partial class Spinner : UserControl
    {
        public Spinner()
        {
            InitializeComponent();
            Loaded += (s, e) => spinAnimation.Begin();
        }
    }
}