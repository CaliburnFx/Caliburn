namespace Caliburn.Core.Logging
{
    using System;

    /// <summary>
    /// A log.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Logs the message as info.
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(string message);

        /// <summary>
        /// Logs the message as a warning.
        /// </summary>
        /// <param name="message">The message.</param>
        void Warn(string message);

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void Error(Exception exception);
    }
}