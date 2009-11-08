namespace Caliburn.Core.Threading
{
    using System;

    /// <summary>
    /// An <see cref="EventArgs"/> used to provide details of a <see cref="IBackgroundTask"/> complete event.
    /// </summary>
    public class BackgroundTaskCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the result of the underlying function.
        /// </summary>
        /// <value>The result.</value>
        public object Result { get; private set; }

        /// <summary>
        /// Gets the previously supplied user state.
        /// </summary>
        /// <value>The state of the user.</value>
        public object UserState { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IBackgroundTask"/> was cancelled.
        /// </summary>
        /// <value><c>true</c> if cancelled; otherwise, <c>false</c>.</value>
        public bool Cancelled { get; private set; }

        /// <summary>
        /// Gets an exception thrown by the <see cref="IBackgroundTask"/> if one occurred.
        /// </summary>
        /// <value>The error.</value>
        public Exception Error { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundTaskCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="userState">State of the user.</param>
        /// <param name="cancelled">if set to <c>true</c> [cancelled].</param>
        /// <param name="error">The error.</param>
        public BackgroundTaskCompletedEventArgs(object result, object userState, bool cancelled, Exception error)
        {
            Result = result;
            UserState = userState;
            Cancelled = cancelled;
            Error = error;
        }
    }
}