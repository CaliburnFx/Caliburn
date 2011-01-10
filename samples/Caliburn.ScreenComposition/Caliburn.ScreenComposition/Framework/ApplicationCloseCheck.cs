namespace Caliburn.ScreenComposition.Framework {
    using System;
    using System.ComponentModel.Composition;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;

    public class ApplicationCloseCheck : IResult {
        readonly Action<IDialogManager, Action<bool>> closeCheck;
        readonly IChild<IConductor> screen;

        public ApplicationCloseCheck(IChild<IConductor> screen, Action<IDialogManager, Action<bool>> closeCheck) {
            this.screen = screen;
            this.closeCheck = closeCheck;
        }

        [Import]
        public IShell Shell { get; set; }

        public void Execute(ResultExecutionContext context) {
            var documentWorkspace = screen.Parent as IDocumentWorkspace;
            if (documentWorkspace != null)
                documentWorkspace.Edit(screen);

            closeCheck(Shell.Dialogs, result => Completed(this, new ResultCompletionEventArgs { WasCancelled = !result }));
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}