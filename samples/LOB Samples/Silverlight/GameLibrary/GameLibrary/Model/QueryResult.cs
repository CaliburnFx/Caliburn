namespace GameLibrary.Model
{
    using System;
    using Caliburn.PresentationFramework.RoutedMessaging;
    using Microsoft.Practices.ServiceLocation;

    public class QueryResult<TResponse> : IResult
    {
        private readonly IQuery<TResponse> _query;

        public QueryResult(IQuery<TResponse> query)
        {
            _query = query;
        }

        public TResponse Response { get; set; }

        public void Execute(ResultExecutionContext context)
        {
            ServiceLocator.Current.GetInstance<IBackend>().Send(_query, response => {
                Response = response;
                Caliburn.PresentationFramework.Invocation.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
            });
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}