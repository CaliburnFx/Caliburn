namespace Caliburn.Silverlight.ApplicationFramework
{
    using System;
    using System.Linq;
    using Core;
    using Core.IoC;

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
                ((ComponentRegistrationBase)decoratedType.GetAttributes<ComponentRegistrationBase>(true)
                    .FirstOrDefault()
                    .GetComponentInfo(decoratedType)).Service
                );
        }
    }
}