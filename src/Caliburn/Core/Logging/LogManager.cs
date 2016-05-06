namespace Caliburn.Core.Logging
{
    using System;

    /// <summary>
    /// Used to manage logging.
    /// </summary>
    public static class LogManager
    {
        static readonly ILog NullLogSingleton = new NullLog();
        static Func<Type, ILog> logLocator = type => NullLogSingleton;

        /// <summary>
        /// Initializes the system with the specified log creator.
        /// </summary>
        /// <param name="logLocator">The log locator.</param>
        public static void Initialize(Func<Type, ILog> logLocator)
        {
            LogManager.logLocator = logLocator;
        }

        /// <summary>
        /// Creates a log.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static ILog GetLog(Type type)
        {
            return logLocator(type);
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