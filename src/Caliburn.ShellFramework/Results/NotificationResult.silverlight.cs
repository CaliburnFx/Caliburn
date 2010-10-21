#if SILVERLIGHT_40

namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Windows;
    using Core.InversionOfControl;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.ViewModels;
    using PresentationFramework.Views;

    public class NotificationResult<T> : IResult
    {
        private readonly int durationInMilliseconds;
        private bool waitForClose;
        private Func<T> viewModelLocator;

        public NotificationResult(int durationInMilliseconds)
        {
            this.durationInMilliseconds = durationInMilliseconds;
        }

        public NotificationResult(T viewModel, int durationInMilliseconds)
        {
            this.durationInMilliseconds = durationInMilliseconds;
            viewModelLocator = () => viewModel;
        }

        public NotificationResult<T> Wait()
        {
            waitForClose = true;
            return this;
        }

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

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}

#endif