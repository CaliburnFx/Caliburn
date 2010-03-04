#if SILVERLIGHT

namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Windows.Controls;
    using PresentationFramework;
    using PresentationFramework.RoutedMessaging;

    public class SaveFileDialogResult : IResult
    {
        private readonly SaveFileDialog _dialog;

        public SaveFileDialogResult(SaveFileDialog dialog)
        {
            _dialog = dialog;
        }

        public SaveFileDialog Dialog
        {
            get { return _dialog; }
        }

        public void Execute(ResultExecutionContext context)
        {
            var result = _dialog.ShowDialog()
                .GetValueOrDefault(false);

            Completed(this, new ResultCompletionEventArgs{ WasCancelled = !result});
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}

#endif