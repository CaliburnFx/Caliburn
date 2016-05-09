using System;

namespace Caliburn.PresentationFramework.RoutedMessaging
{
    internal class TaskResultThenDefault<T> : IResult
    {
        private readonly TaskResult<T> taskResult;
        private readonly Func<object, DefaultResult> defaultResultFactory;

        public TaskResultThenDefault(TaskResult<T> taskResult, Func<object, DefaultResult> defaultResultFactory)
        {
            this.taskResult = taskResult;
            this.defaultResultFactory = defaultResultFactory;
        }

        public void Execute(ResultExecutionContext context)
        {

        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}