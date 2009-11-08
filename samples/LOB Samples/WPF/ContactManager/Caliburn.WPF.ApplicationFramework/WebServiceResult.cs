namespace Caliburn.WPF.ApplicationFramework
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Core;
    using Core.Invocation;
    using Microsoft.Practices.ServiceLocation;
    using PresentationFramework;

    public class WebServiceResult<T, K> : IResult
        where T : new()
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

        public event Action<IResult, Exception> Completed = delegate { };

        public void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode)
        {
            ServiceLocator.Current.GetInstance<ILoadScreen>().StartLoading();
            //if you would rather disable the control that caused the service to be called, you could do this:
            //ChangeAvailability(message, false);

            var factory = ServiceLocator.Current.GetInstance<IEventHandlerFactory>();

            var lambda = (LambdaExpression)_serviceCall;
            var methodCall = (MethodCallExpression)lambda.Body;
            var eventName = methodCall.Method.Name.Replace("Async", "Completed");

            var service = new T();
            var handler = factory.Wire(service, eventName);

            handler.SetActualHandler(ActualHandler);

            _serviceCall.Compile()(service);
        }

        private void ActualHandler(object[] parameters)
        {
            ServiceLocator.Current.GetInstance<ILoadScreen>().StopLoading();
            //or re-enable the control that caused the service to be called:
            //ChangeAvailability(message, true);

            if(_callback != null)
                _callback((K)parameters[1]);

            Completed(this, null);
        }

        private void ChangeAvailability(IRoutedMessage message, bool isAvailable)
        {
            (from trigger in message.Source.Triggers
             where trigger.Message == message
             select trigger).Apply(x => x.UpdateAvailabilty(isAvailable));
        }
    }
}