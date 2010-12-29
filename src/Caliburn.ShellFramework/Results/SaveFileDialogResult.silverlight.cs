#if SILVERLIGHT

namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Windows.Controls;
    using PresentationFramework;
    using PresentationFramework.RoutedMessaging;

    /// <summary>
    /// Shows a SaveFileDialog.
    /// </summary>
    public class SaveFileDialogResult : IResult
    {
        private readonly SaveFileDialog _dialog;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileDialogResult"/> class.
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        public SaveFileDialogResult(SaveFileDialog dialog)
        {
            _dialog = dialog;
        }

        /// <summary>
        /// Gets the dialog.
        /// </summary>
        /// <value>The dialog.</value>
        public SaveFileDialog Dialog
        {
            get { return _dialog; }
        }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(ResultExecutionContext context)
        {
            var result = _dialog.ShowDialog()
                .GetValueOrDefault(false);

            Completed(this, new ResultCompletionEventArgs{ WasCancelled = !result});
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}

#endif