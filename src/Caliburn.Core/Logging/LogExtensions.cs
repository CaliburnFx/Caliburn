namespace Caliburn.Core.Logging
{
    /// <summary>
    /// Extension methods related to logging.
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Logs the message as info.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="format">The message.</param>
        /// <param name="args">The args.</param>
        public static void Info(this ILog log, string format, params object[] args)
        {
            log.Info(string.Format(format, args));
        }
    }
}