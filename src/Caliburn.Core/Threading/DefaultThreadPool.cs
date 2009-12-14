namespace Caliburn.Core.Threading
{
    using System.Threading;

    /// <summary>
    /// An implementation of <see cref="IThreadPool"/>.
    /// </summary>
    public class DefaultThreadPool : IThreadPool
    {
        /// <summary>
        /// Queues the user work item.
        /// </summary>
        /// <param name="callback">The work callback.</param>
        /// <param name="userState">The user state.</param>
        /// <returns></returns>
        public bool QueueUserWorkItem(WaitCallback callback, object userState)
        {
            return ThreadPool.QueueUserWorkItem(callback, userState);
        }
    }
}