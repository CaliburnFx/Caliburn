#if SILVERLIGHT_40

namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Windows;
    using Core.InversionOfControl;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.ViewModels;
    using PresentationFramework.Views;

    /// <summary>
    /// An <see cref="IResult"/> for showing a toast notification.
    /// </summary>
    /// <typeparam name="T">The notification view model.</typeparam>
    public class NotificationResult<T> : IResult
    {
        readonly int durationInMilliseconds;
        readonly Func<ResultExecutionContext, T> locateViewModel =
            c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<T>();

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
            locateViewModel = c => viewModel;
        }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(ResultExecutionContext context)
        {
            var viewModel = locateViewModel(context);
            var windowManager = context.ServiceLocator.GetInstance<IWindowManager>();

            windowManager.ShowNotification(viewModel, durationInMilliseconds);
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}

#endif