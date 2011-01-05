namespace Caliburn.SimpleMDI {
    using PresentationFramework.Screens;

    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive {
        int count = 1;

        public void OpenTab() {
            ActivateItem(new TabViewModel {
                DisplayName = "Tab " + count++
            });
        }
    }
}