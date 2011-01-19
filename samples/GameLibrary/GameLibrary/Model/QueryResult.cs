﻿namespace GameLibrary.Model {
    using System;
    using System.ComponentModel.Composition;
    using Caliburn.PresentationFramework.RoutedMessaging;

    public class QueryResult<TResponse> : IResult {
        readonly IQuery<TResponse> query;

        public QueryResult(IQuery<TResponse> query) {
            this.query = query;
        }

        [Import]
        public IBackend Backend { get; set; }
        public TResponse Response { get; set; }

        public void Execute(ResultExecutionContext context) {
            Backend.Send(query, response => {
                Response = response;
                Caliburn.PresentationFramework.Invocation.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
            });
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}