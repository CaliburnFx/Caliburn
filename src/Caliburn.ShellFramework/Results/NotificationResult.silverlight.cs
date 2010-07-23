#if SILVERLIGHT_40

namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Windows;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.ViewModels;
    using PresentationFramework.Views;

    public class NotificationResult<T> : IResult
    {
        private readonly int _durationInMilliseconds;
        private bool _waitForClose;
        private Func<T> _viewModelLocator;

        public NotificationResult(int durationInMilliseconds)
        {
            _durationInMilliseconds = durationInMilliseconds;
        }

        public NotificationResult(T viewModel, int durationInMilliseconds)
        {
            _durationInMilliseconds = durationInMilliseconds;
            _viewModelLocator = () => viewModel;
        }

        public NotificationResult<T> Wait()
        {
            _waitForClose = true;
            return this;
        }

        public void Execute(ResultExecutionContext context)
        {
            if (_viewModelLocator == null)
                _viewModelLocator = () => context.ServiceLocator.GetInstance<IViewModelFactory>().Create<T>();

            var window = new NotificationWindow();
            var viewModel = _viewModelLocator();
            var view = context.ServiceLocator.GetInstance<IViewLocator>().Locate(viewModel, window, null);

            context.ServiceLocator.GetInstance<IViewModelBinder>().Bind(viewModel, view, null);
            window.Content = (FrameworkElement)view;

            if (_waitForClose)
            {
                window.Closed += delegate{
                    Completed(this, new ResultCompletionEventArgs());
                };
            }

            window.Show(_durationInMilliseconds);

            if (!_waitForClose)
                Completed(this, new ResultCompletionEventArgs());
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}

#endif