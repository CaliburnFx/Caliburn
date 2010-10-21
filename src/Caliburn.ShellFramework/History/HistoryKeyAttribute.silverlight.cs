#if SILVERLIGHT

namespace Caliburn.ShellFramework.History
{
    using System;
    using Core.InversionOfControl;

    public class HistoryKeyAttribute : Attribute, IHistoryKey
    {
        private readonly Type type;
        private readonly string value;

        public HistoryKeyAttribute(string value, Type type)
        {
            this.value = value;
            this.type = type;
        }

        public string Value
        {
            get { return value; }
        }

        public object GetInstance()
        {
            return IoC.GetInstance(type, null);
        }
    }
}

#endif