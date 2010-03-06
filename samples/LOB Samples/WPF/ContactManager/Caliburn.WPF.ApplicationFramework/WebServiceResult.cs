namespace Caliburn.WPF.ApplicationFramework
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Core;
    using Core.Invocation;
    using Microsoft.Practices.ServiceLocation;
    using PresentationFramework.RoutedMessaging;

    public class WebServiceResult<T, K> : IResult
        where T : new()
        where K : EventArgs
    {
        private readonly Action<K> _callback;
        private readonly Expression<Action<T>> _serviceCall;

        public WebServiceResult(Expression<Action<T>> serviceCall)
        {
            _serviceCall = serviceCall;
        }

        public WebServiceResult(Expression<Action<T>> serviceCall, Action<K> callback)
        {
            _serviceCall = serviceCall;
            _callback = callback;
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        public void Execute(ResultExecutionContext context)
        {
            context.ServiceLocator.GetInstance<ILoadScreen>().StartLoading();
            //if you would rather disable the control that caused the service to be called, you could do this:
            //ChangeAvailability(message, false);

            var lambda = (LambdaExpression)_serviceCall;
            var methodCall = (MethodCallExpression)lambda.Body;
            var eventName = methodCall.Method.Name.Replace("Async", "Completed");

            var service = new T();

            EventHelper.WireEvent(
                service, 
                service.GetType().GetEvent(eventName),
                OnEvent
                );

            _serviceCall.Compile()(service);
        }

        private void OnEvent(object s, EventArgs e)
        {
            ServiceLocator.Current.GetInstance<ILoadScreen>().StopLoading();
            //or re-enable the control that caused the service to be called:
            //ChangeAvailability(message, true);

            if (_callback != null)
                _callback((K)e);

            Completed(this, new ResultCompletionEventArgs());
        }

        private void ChangeAvailability(IRoutedMessage message, bool isAvailable)
        {
            (from trigger in message.Source.Triggers
             where trigger.Message == message
             select trigger).Apply(x => x.UpdateAvailabilty(isAvailable));
        }
    }
}