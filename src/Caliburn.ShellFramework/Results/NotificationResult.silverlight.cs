#if SILVERLIGHT_40

namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Windows;
    using Core.InversionOfControl;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.ViewModels;
    using PresentationFramework.Views;

    /// <summary>
    /// An <see cref="IResult"/> for showing a toast notification.
    /// </summary>
    /// <typeparam name="T">The notification view model.</typeparam>
    public class NotificationResult<T> : IResult
    {
        private readonly int durationInMilliseconds;
        private bool waitForClose;
        private Func<T> viewModelLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationResult&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="durationInMilliseconds">The duration in milliseconds.</param>
        public NotificationResult(int durationInMilliseconds)
        {
            this.durationInMilliseconds = durationInMilliseconds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationResult&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="durationInMilliseconds">The duration in milliseconds.</param>
        public NotificationResult(T viewModel, int durationInMilliseconds)
        {
            this.durationInMilliseconds = durationInMilliseconds;
            viewModelLocator = () => viewModel;
        }

        /// <summary>
        /// Causes the completion event to be raised after the notification closes.
        /// </summary>
        /// <returns></returns>
        public NotificationResult<T> Wait()
        {
            waitForClose = true;
            return this;
        }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(ResultExecutionContext context)
        {
            if (viewModelLocator == null)
                viewModelLocator = () => context.ServiceLocator.GetInstance<IViewModelFactory>().Create<T>();

            var window = new NotificationWindow();
            var viewModel = viewModelLocator();
            var view = context.ServiceLocator.GetInstance<IViewLocator>().LocateForModel(viewModel, window, null);

            context.ServiceLocator.GetInstance<IViewModelBinder>().Bind(viewModel, view, null);
            window.Content = (FrameworkElement)view;

            if (waitForClose)
            {
                window.Closed += delegate{
                    Completed(this, new ResultCompletionEventArgs());
                };
            }

            window.Show(durationInMilliseconds);

            if (!waitForClose)
                Completed(this, new ResultCompletionEventArgs());
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}

#endif