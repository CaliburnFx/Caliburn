namespace Caliburn.SimpleNavigation {
    using System.Windows;
    using PresentationFramework.Screens;

    public class PageTwoViewModel : Screen {
        protected override void OnActivate() {
            MessageBox.Show("Page Two Activated"); //Don't do this in a real VM.
            base.OnActivate();
        }
    }
}