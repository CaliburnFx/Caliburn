using Caliburn.PresentationFramework.RoutedMessaging;
using Microsoft.Practices.ServiceLocation;

namespace GameLibrary.Model
{
    using System;

    public class CommandResult : IResult
    {
        private readonly ICommand _command;

        public CommandResult(ICommand command)
        {
            _command = command;
        }

        public void Execute(ResultExecutionContext context)
        {
            var bus = ServiceLocator.Current.GetInstance<IBackend>();
            bus.Send(_command);
            Completed(this, new ResultCompletionEventArgs());
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { }; 
    }
}