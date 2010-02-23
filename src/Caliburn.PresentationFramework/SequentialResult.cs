namespace Caliburn.PresentationFramework
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="IResult"/> that enables execution of multiple results.
    /// </summary>
    public class SequentialResult : IResult
    {
        private readonly IEnumerator<IResult> _enumerator;
        private ResultExecutionContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialResult"/> class.
        /// </summary>
        /// <param name="children">The children.</param>
        public SequentialResult(IEnumerable<IResult> children)
        {
            _enumerator = children.GetEnumerator();
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        /// <summary>
        /// Executes the result within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(ResultExecutionContext context)
        {
            _context = context;
            ChildCompleted(null, new ResultCompletionEventArgs());
        }

        private void ChildCompleted(object sender, ResultCompletionEventArgs args)
        {
            if(args.Error != null)
            {
                OnComplete(args.Error, false);
                return;
            }

            if(args.WasCancelled)
            {
                OnComplete(null, true);
                return;
            }

            var previous = sender as IResult;

            if (previous != null)
                previous.Completed -= ChildCompleted;

            if(_enumerator.MoveNext())
            {
                try
                {
                    var next = _enumerator.Current;
                    next.Completed += ChildCompleted;
                    next.Execute(_context);
                }
                catch(Exception ex)
                {
                    OnComplete(ex, false);
                    return;
                }
            }
            else OnComplete(null, false);
        }

        private void OnComplete(Exception error, bool wasCancelled) 
        {
            Completed(this, new ResultCompletionEventArgs{ Error = error, WasCancelled = wasCancelled});
        }
    }
}