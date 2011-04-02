namespace Caliburn.PresentationFramework.RoutedMessaging {
    using System;
    using System.Collections.Generic;
    using Core.InversionOfControl;
    using Core.Logging;

    /// <summary>
    ///   An implementation of <see cref = "IResult" /> that enables sequential execution of multiple results.
    /// </summary>
    public class SequentialResult : IResult {
        static readonly ILog Log = LogManager.GetLog(typeof(SequentialResult));

        readonly IEnumerator<IResult> enumerator;
        ResultExecutionContext context;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "SequentialResult" /> class.
        /// </summary>
        /// <param name = "enumerator">The enumerator.</param>
        public SequentialResult(IEnumerator<IResult> enumerator) {
            this.enumerator = enumerator;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "SequentialResult" /> class.
        /// </summary>
        /// <param name = "results">The results.</param>
        public SequentialResult(IEnumerable<IResult> results)
            : this(results.GetEnumerator()) {}

        /// <summary>
        ///   Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        /// <summary>
        ///   Executes the result using the specified context.
        /// </summary>
        /// <param name = "context">The context.</param>
        public void Execute(ResultExecutionContext context) {
            this.context = context;
            ChildCompleted(null, new ResultCompletionEventArgs());
        }

        void ChildCompleted(object sender, ResultCompletionEventArgs args) {
            if(args.Error != null) {
                Log.Error(args.Error);
                OnComplete(args.Error, false);
                return;
            }

            if(args.WasCancelled) {
                Log.Info("Result enumeration cancelled.");
                OnComplete(null, true);
                return;
            }

            var previous = sender as IResult;

            if(previous != null)
                previous.Completed -= ChildCompleted;

            var moveNextSucceeded = false;
            try {
                moveNextSucceeded = enumerator.MoveNext();
            }
            catch(Exception ex) {
                Log.Error(ex);
                OnComplete(ex, false);
                return;
            }

            if(moveNextSucceeded) {
                try {
                    var next = enumerator.Current;
                    IoC.BuildUp(next);
                    next.Completed += ChildCompleted;
                    next.Execute(context);
                }
                catch(Exception ex) {
                    Log.Error(ex);
                    OnComplete(ex, false);
                    return;
                }
            }
            else OnComplete(null, false);
        }

        void OnComplete(Exception error, bool wasCancelled) {
            enumerator.Dispose();
            Log.Info("Result enumeration complete.");
            Completed(this, new ResultCompletionEventArgs { Error = error, WasCancelled = wasCancelled });
        }
    }
}