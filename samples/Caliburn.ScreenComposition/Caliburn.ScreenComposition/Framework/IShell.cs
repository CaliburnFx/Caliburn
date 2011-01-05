namespace Caliburn.ScreenComposition.Framework {
    using PresentationFramework.Screens;

    public interface IShell : IConductor, IGuardClose {
        IDialogManager Dialogs { get; }
    }
}