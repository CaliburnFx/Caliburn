#if !SILVERLIGHT

namespace Caliburn.ShellFramework.Results
{
    using System;
    using Microsoft.Win32;
    using PresentationFramework.RoutedMessaging;

    public class ShowCommonDialogResult : IResult
    {
        private readonly CommonDialog _commonDialog;

        public ShowCommonDialogResult(CommonDialog commonDialog)
        {
            _commonDialog = commonDialog;
        }

        public void Execute(ResultExecutionContext context)
        {
            var result = _commonDialog.ShowDialog()
                .GetValueOrDefault(false);

            Completed(this, new ResultCompletionEventArgs {WasCancelled = !result});
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}

#endif