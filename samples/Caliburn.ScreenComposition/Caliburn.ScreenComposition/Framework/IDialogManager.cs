namespace Caliburn.ScreenComposition.Framework {
    using System;
    using PresentationFramework.Screens;

    public interface IDialogManager {
        void ShowDialog(IScreen dialogModel);
        void ShowMessageBox(string message, string title = null, MessageBoxOptions options = MessageBoxOptions.Ok, Action<IMessageBox> callback = null);
    }
}