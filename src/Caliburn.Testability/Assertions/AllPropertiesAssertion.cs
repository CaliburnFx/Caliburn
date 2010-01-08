namespace Caliburn.Testability.Assertions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Extensions;

    /// <summary>
    /// Builds up a set of assertions for all of the properties of an object.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="INotifyPropertyChanged"/>.</typeparam>
    public class AllPropertiesAssertion<T> : PropertyAssertionBase<T> where T : class, INotifyPropertyChanged
    {
        private readonly IList<string> ignored = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AllPropertiesAssertion&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="propertyOwner">The property owner.</param>
        public AllPropertiesAssertion(INotifyPropertyChanged propertyOwner) : base(propertyOwner) { }

        /// <summary>
        /// Ignore a property when makeing assertions on this object.
        /// </summary>
        /// <typeparam name="K">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public AllPropertiesAssertion<T> Ignoring<K>(Expression<Func<T, K>> property)
        {
            ignored.Add(property.GetPropertyInfo().Name);
            return this;
        }

        /// <summary>
        /// Gets the candidate properties.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<PropertyInfo> GetCandidateProperties()
        {
            return from property in typeof(T).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance)
                   where !ignored.Contains(property.Name)
                         && property.CanWrite
                   select property;
        }

        /// <summary>
        /// Uses the specified value to set the property during the process of change notification asssertion.
        /// </summary>
        /// <typeparam name="K">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="valueToSet">The value to set.</param>
        /// <returns></returns>
        public AllPropertiesAssertion<T> SetValue<K>(Expression<Func<T, K>> property, K valueToSet)
        {
            Values[property.GetPropertyInfo()] = valueToSet;
            return this;
        }

        /// <summary>
        /// Raises the change notification.
        /// </summary>
        public void RaiseChangeNotification()
        {
            Execute();
        }
    }
}