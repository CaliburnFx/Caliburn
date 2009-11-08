namespace Caliburn.Core.Threading
{
    /// <summary>
    /// The context for the currently executing background task.
    /// </summary>
    public interface IBackgroundContext
    {
        /// <summary>
        /// Gets a value indicating whether the current task has been cancelled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if cancelled; otherwise, <c>false</c>.
        /// </value>
        bool IsCancelled { get; }

        /// <summary>
        /// Enables the current task to update its progress.
        /// </summary>
        /// <param name="percentage">The percentage.</param>
        void UpdateProgress(double percentage);
    }
}