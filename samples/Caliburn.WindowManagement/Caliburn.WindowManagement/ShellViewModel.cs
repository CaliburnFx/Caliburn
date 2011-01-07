namespace Caliburn.WindowManagement {
    using System.ComponentModel.Composition;
    using PresentationFramework.ApplicationModel;

    [Export(typeof(IShell))]
    public class ShellViewModel : IShell {
        readonly IWindowManager windowManager;

        [ImportingConstructor]
        public ShellViewModel(IWindowManager windowManager) {
            this.windowManager = windowManager;
        }

        public void OpenModeless() {
            windowManager.ShowWindow(new DialogViewModel(), "Modeless");
        }

        public void OpenModal() {
            var result = windowManager.ShowDialog(new DialogViewModel());
        }

        public void OpenPopup() {
            windowManager.ShowPopup(new DialogViewModel(), "Popup");
        }
    }
}