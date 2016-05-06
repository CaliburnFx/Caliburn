namespace Caliburn.ShellFramework.Results
{
    using System;
    using Core.InversionOfControl;
    using PresentationFramework.RoutedMessaging;
    using Services;

    /// <summary>
    /// An <see cref="IResult"/> for marking a view model as busy.
    /// </summary>
    public class BusyResult : IResult
    {
        readonly bool isBusy;
        readonly object busyViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusyResult"/> class.
        /// </summary>
        /// <param name="isBusy">if set to <c>true</c> marked as busy.</param>
        /// <param name="busyViewModel">The view model for the busy indicator, or null for default.</param>
        public BusyResult(bool isBusy, object busyViewModel)
        {
            this.isBusy = isBusy;
            this.busyViewModel = busyViewModel;
        }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(ResultExecutionContext context)
        {
            object sourceViewModel = null;

            if(context.HandlingNode != null)
                sourceViewModel = context.HandlingNode.MessageHandler.Unwrap();

            var busyService = context.ServiceLocator.GetInstance<IBusyService>();

            if(isBusy)
                busyService.MarkAsBusy(sourceViewModel, busyViewModel);
            else busyService.MarkAsNotBusy(sourceViewModel);

            Completed(this, new ResultCompletionEventArgs());
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}