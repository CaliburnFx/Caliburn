namespace GameLibrary.Framework
{
    using System;
    using System.Diagnostics;
    using Caliburn.Core.Logging;

    public class DebugLog : ILog
    {
        private readonly Type _type;

        public DebugLog(Type type)
        {
            _type = type;
        }

        public void Info(string message)
        {
            Debug.WriteLine("INFO: {0} : {1}", _type.Name, message);
        }

        public void Warn(string message)
        {
            Debug.WriteLine("WARN: {0} : {1}", _type.Name, message);
        }

        public void Error(Exception exception)
        {
            Debug.WriteLine("ERROR: {0}\n{1}", _type.Name, exception);
        }
    }
}