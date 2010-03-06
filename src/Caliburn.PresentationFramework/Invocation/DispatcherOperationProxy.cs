namespace Caliburn.PresentationFramework.Invocation
{
    using System;
    using System.Windows.Threading;

    /// <summary>
    /// An implemenation of <see cref="IDispatcherOperation"/>.
    /// </summary>
    public class DispatcherOperationProxy : IDispatcherOperation
    {
        private readonly DispatcherOperation _operation;

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherOperationProxy"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public DispatcherOperationProxy(DispatcherOperation operation)
        {
            _operation = operation;
        }

#if !SILVERLIGHT

        /// <summary>
        /// Aborts the operation.
        /// </summary>
        /// <returns></returns>
        public bool Abort()
        {
            return _operation.Abort();
        }

        /// <summary>
        /// Waits for the operation to complete
        /// </summary>
        public void Wait()
        {
            _operation.Wait();
        }

        /// <summary>
        /// Gets the result of the operation after it has completed.
        /// </summary>
        /// <value>The result.</value>
        public object Result
        {
            get { return _operation.Result; }
        }

        /// <summary>
        /// Occurs when the operation is aborted.
        /// </summary>
        public event EventHandler Aborted
        {
            add { _operation.Aborted += value; }
            remove { _operation.Aborted -= value; }
        }

        /// <summary>
        /// Occurs when the operation has completed.
        /// </summary>
        public event EventHandler Completed
        {
            add { _operation.Completed += value; }
            remove { _operation.Completed -= value; }
        }

#endif
    }
}