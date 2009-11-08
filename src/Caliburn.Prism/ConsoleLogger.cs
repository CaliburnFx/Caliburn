namespace Caliburn.Prism
{
    using System;
    using System.Globalization;
    using System.IO;
    using Microsoft.Practices.Composite.Logging;

    /// <summary>
    /// A simple, default implementation of <see cref="ILoggerFacade"/>.
    /// </summary>
    public class ConsoleLogger : ILoggerFacade, IDisposable
    {
        private readonly TextWriter _writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogger"/> class.
        /// </summary>
        public ConsoleLogger()
        {
            _writer = Console.Out;
        }

        /// <summary>
        /// Write a new log entry with the specified category and priority.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category of the entry.</param>
        /// <param name="priority">The priority of the entry.</param>
        public void Log(string message, Category category, Priority priority)
        {
            string messageToLog = String.Format(
                CultureInfo.InvariantCulture,
                "{1}: {2}. Priority: {3}. Timestamp:{0:u}.",
                DateTime.Now,
                category.ToString().ToUpper(CultureInfo.InvariantCulture),
                message,
                priority);

            _writer.WriteLine(messageToLog);
        }

        /// <summary>
        /// Disposes the associated <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="disposing">When <see langword="true"/>, disposes the associated <see cref="TextWriter"/>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(_writer != null)
                {
                    _writer.Dispose();
                }
            }
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        /// <remarks>Calls <see cref="Dispose(bool)"/></remarks>.
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}