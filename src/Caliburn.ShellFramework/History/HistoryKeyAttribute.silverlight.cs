#if SILVERLIGHT && !WP7

namespace Caliburn.ShellFramework.History
{
    using System;
    using Microsoft.Practices.ServiceLocation;
    using PresentationFramework.Screens;

    public class HistoryKeyAttribute : Attribute, IHistoryKey
    {
        private readonly Type _type;
        private readonly string _value;

        public HistoryKeyAttribute(string value, Type type)
        {
            _value = value;
            _type = type;
        }

        public string Value
        {
            get { return _value; }
        }

        public IScreen GetInstance()
        {
            return (IScreen)ServiceLocator.Current.GetInstance(_type);
        }
    }
}

#endif