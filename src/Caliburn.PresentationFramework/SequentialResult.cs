namespace Caliburn.PresentationFramework
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="IResult"/> that enables execution of multiple results.
    /// </summary>
    public class SequentialResult : IResult
    {
        private readonly IEnumerable<IResult> _children;

        private IEnumerator<IResult> _enumerator;
        private IRoutedMessageWithOutcome _message;
        private IInteractionNode _handlingNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialResult"/> class.
        /// </summary>
        /// <param name="children">The children.</param>
        public SequentialResult(IEnumerable<IResult> children)
        {
            _children = children;
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event Action<IResult, Exception> Completed = delegate { };

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public IEnumerable<IResult> Children
        {
            get { return _children; }
        }

        /// <summary>
        /// Executes the custom code.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="handlingNode">The handling node.</param>
        public void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode)
        {
            _message = message;
            _handlingNode = handlingNode;

            _enumerator = _children.GetEnumerator();

            ChildCompleted(null, null);
        }

        private void ChildCompleted(IResult previous, Exception exception)
        {
            if(exception != null)
            {
                if (exception is CancelResult)
                    OnComplete(null);
                else OnComplete(exception);

                return;
            }

            if(previous != null)
                previous.Completed -= ChildCompleted;

            if(_enumerator.MoveNext())
            {
                try
                {
                    var next = _enumerator.Current;
                    next.Completed += ChildCompleted;
                    next.Execute(_message, _handlingNode);
                }
                catch(Exception ex)
                {
                    OnComplete(ex);
                    return;
                }
            }
            else OnComplete(null);
        }

        private void OnComplete(Exception exception) 
        {
            _enumerator = null;
            _message = null;
            _handlingNode = null;

            Completed(this, exception);
        }
    }
}