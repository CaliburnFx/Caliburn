namespace Caliburn.Silverlight.ApplicationFramework
{
    using System;

    public class HistoryInfo
    {
        public string HistoryKey { get; private set; }
        public Type ServiceType { get; private set; }

        public HistoryInfo(string historyKey, Type serviceType)
        {
            HistoryKey = historyKey;
            ServiceType = serviceType;
        }
    }
}