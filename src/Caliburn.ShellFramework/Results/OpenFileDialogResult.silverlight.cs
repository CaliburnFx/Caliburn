#if SILVERLIGHT

namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Windows.Controls;
    using PresentationFramework;

    public class OpenFileDialogResult : IResult
    {
        private readonly OpenFileDialog _dialog;

        public OpenFileDialogResult(OpenFileDialog dialog)
        {
            _dialog = dialog;
        }

        public OpenFileDialog Dialog
        {
            get { return _dialog; }
        }

        public void Execute(ResultExecutionContext context)
        {
            var result = _dialog.ShowDialog()
                .GetValueOrDefault(false);

            Completed(this, new ResultCompletionEventArgs {WasCancelled = !result});
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}

#endif