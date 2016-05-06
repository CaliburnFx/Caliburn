#if !SILVERLIGHT

namespace Caliburn.ShellFramework.Results
{
    using System;
    using Microsoft.Win32;
    using PresentationFramework.RoutedMessaging;

    /// <summary>
    /// Shows a CommonDialog.
    /// </summary>
    public class ShowCommonDialogResult : IResult
    {
        readonly CommonDialog commonDialog;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowCommonDialogResult"/> class.
        /// </summary>
        /// <param name="commonDialog">The common dialog.</param>
        public ShowCommonDialogResult(CommonDialog commonDialog)
        {
            this.commonDialog = commonDialog;
        }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(ResultExecutionContext context)
        {
            var result = commonDialog.ShowDialog()
                .GetValueOrDefault(false);

            Completed(this, new ResultCompletionEventArgs {WasCancelled = !result});
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}

#endif