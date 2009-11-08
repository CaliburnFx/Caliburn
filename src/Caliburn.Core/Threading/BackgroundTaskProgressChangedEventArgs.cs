namespace Caliburn.Core.Threading
{
    using System;

    /// <summary>
    /// An <see cref="EventArgs"/> used to provide details of a <see cref="IBackgroundTask"/> progress change event.
    /// </summary>
    public class BackgroundTaskProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets original state provided by the user.
        /// </summary>
        /// <value>The state of the user.</value>
        public object UserState { get; private set; }

        /// <summary>
        /// Gets the percentage of work complete.
        /// </summary>
        /// <value>The percentage.</value>
        public double Percentage { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundTaskProgressChangedEventArgs"/> class.
        /// </summary>
        /// <param name="userState">State of the user.</param>
        /// <param name="percentage">The percentage.</param>
        public BackgroundTaskProgressChangedEventArgs(object userState, double percentage)
        {
            UserState = userState;
            Percentage = percentage;
        }
    }
}