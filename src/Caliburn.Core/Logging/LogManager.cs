namespace Caliburn.Core.Logging
{
    using System;

    /// <summary>
    /// Used to manage logging.
    /// </summary>
    public static class LogManager
    {
        private static readonly ILog _nullLog = new NullLog();
        private static Func<Type, ILog> _logLocator = type => _nullLog;

        /// <summary>
        /// Initializes the system with the specified log creator.
        /// </summary>
        /// <param name="logLocator">The log locator.</param>
        public static void Initialize(Func<Type, ILog> logLocator)
        {
            _logLocator = logLocator;
        }

        /// <summary>
        /// Creates a log.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static ILog GetLog(Type type)
        {
            return _logLocator(type);
        }

        private class NullLog : ILog 
        {
            public void Info(string message) {}
            public void Warn(string message) {}
            public void Error(Exception exception) {}
            public void Error(string message, Exception exception) {}
        }
    }
}