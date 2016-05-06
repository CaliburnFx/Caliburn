#if SILVERLIGHT

namespace Caliburn.ShellFramework.History
{
    using System;
    using Core.InversionOfControl;

    /// <summary>
    /// An attribute that serves as an <see cref="IHistoryKey"/>.
    /// </summary>
    public class HistoryKeyAttribute : Attribute, IHistoryKey
    {
        private readonly Type type;
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryKeyAttribute"/> class.
        /// </summary>
        /// <param name="value">The key's value.</param>
        /// <param name="type">The view model type.</param>
        public HistoryKeyAttribute(string value, Type type)
        {
            this.value = value;
            this.type = type;
        }

        /// <summary>
        /// Gets the key's value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get { return value; }
        }

        /// <summary>
        /// Gets the view model instance.
        /// </summary>
        /// <returns></returns>
        public object GetInstance()
        {
            return IoC.GetInstance(type, null);
        }
    }
}

#endif