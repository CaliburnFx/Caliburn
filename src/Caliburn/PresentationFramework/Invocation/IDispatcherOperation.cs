namespace Caliburn.PresentationFramework.Invocation
{
    using System;

    /// <summary>
    /// Represents an object that is used to interact with an operation that has been posted to 
    /// the System.Windows.Threading.Dispatcher queue.
    /// </summary>
    public interface IDispatcherOperation
    {
#if !SILVERLIGHT
        /// <summary>
        /// Aborts the operation.
        /// </summary>
        /// <returns></returns>
        bool Abort();

        /// <summary>
        /// Waits for the operation to complete
        /// </summary>
        void Wait();

        /// <summary>
        /// Gets the result of the operation after it has completed.
        /// </summary>
        /// <value>The result.</value>
        object Result { get; }

        /// <summary>
        /// Occurs when the operation is aborted.
        /// </summary>
        event EventHandler Aborted;

        /// <summary>
        /// Occurs when the operation has completed.
        /// </summary>
        event EventHandler Completed;
#endif
    }
}