namespace Caliburn.Silverlight.ApplicationFramework
{
    using System;
    using System.Linq;
    using Core.Metadata;

    public class HistoryKeyAttribute : Attribute
    {
        private readonly string _historyKey;

        public HistoryKeyAttribute(string historyKey)
        {
            _historyKey = historyKey;
        }

        public HistoryInfo CreateHistoryInfo(Type decoratedType)
        {
            return new HistoryInfo(
                _historyKey,
                decoratedType.GetCustomAttributes(typeof(RegisterAttribute), true)
                    .Cast<RegisterAttribute>()
                    .FirstOrDefault()
                    .GetComponentInfo(decoratedType).Service
                );
        }
    }
}