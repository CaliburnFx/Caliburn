namespace GameLibrary.Framework {
    using System;
    using System.Diagnostics;
    using Caliburn.Core.Logging;

    public class DebugLog : ILog {
        readonly Type type;

        public DebugLog(Type type) {
            this.type = type;
        }

        public void Info(string message) {
            Debug.WriteLine("INFO: {0} : {1}", type.Name, message);
        }

        public void Warn(string message) {
            Debug.WriteLine("WARN: {0} : {1}", type.Name, message);
        }

        public void Error(Exception exception) {
            Debug.WriteLine("ERROR: {0}\n{1}", type.Name, exception);
        }

        public void Error(string message, Exception exception) {
            Debug.WriteLine("ERROR: {0}\n{1}\n{1}", type.Name, message, exception);
        }
    }
}