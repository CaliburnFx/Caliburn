namespace Caliburn.Silverlight.NavigationShell.Framework
{
    using System;
    using System.IO;
    using System.Net;
    using PresentationFramework;

    public class WebClientResult : IResult
    {
        private readonly Uri _uri;

        public WebClientResult(Uri uri)
        {
            _uri = uri;
        }

        public Uri Uri
        {
            get { return _uri; }
        }

        public Stream Stream { get; set; }

        public void Execute(ResultExecutionContext context)
        {
            var client = new WebClient();

            client.OpenReadCompleted += (s, e) =>
            {
                if (e.Error != null)
                    Core.Invocation.Execute.OnUIThread(
                        () => Completed(this, new ResultCompletionEventArgs { Error = e.Error }));
                else
                {
                    Stream = e.Result;
                    Core.Invocation.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
                }
            };

            client.OpenReadAsync(_uri);
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}