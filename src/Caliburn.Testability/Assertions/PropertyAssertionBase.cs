namespace Caliburn.Testability.Assertions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// A base class for assertions on properties.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="INotifyPropertyChanged"/>.</typeparam>
    public abstract class PropertyAssertionBase<T> where T : class, INotifyPropertyChanged
    {
        private readonly INotifyPropertyChanged _propertyOwner;
        private readonly IDictionary<PropertyInfo, object> _values = new Dictionary<PropertyInfo, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAssertionBase&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="propertyOwner">The property owner.</param>
        protected PropertyAssertionBase(INotifyPropertyChanged propertyOwner)
        {
            _propertyOwner = propertyOwner;
        }

        /// <summary>
        /// Gets the values to set.
        /// </summary>
        /// <value>The values.</value>
        protected IDictionary<PropertyInfo, object> Values
        {
            get { return _values; }
        }

        /// <summary>
        /// Executes the constructed set of assertions.
        /// </summary>
        public void Execute()
        {
            var candidates = GetCandidateProperties();

            AssertThatClassHasCandidateProperties(candidates);

            var results = candidates
                .ToDictionary<PropertyInfo, PropertyInfo, bool>(p => p, DoesPropertyRaiseNotification);

            var failures = (from result in results
                            where result.Value == false
                            select new {result.Key})
                .ToList();


            if(failures.Count == 0) return;

            var msg = string.Format(
                "\nThe following properties on {0} did not raise change notification:\n",
                typeof(T).Name);

            failures.ForEach(failure =>{
                var setValue = GetSetterValueForProperty(failure.Key);
                msg += string.Format("\t{0} with the value set to '{1}'\n",
                                     failure.Key.Name,
                                     setValue);
            });

            throw new Exception(msg);
        }

        /// <summary>
        /// Gets the candidate properties.
        /// </summary>
        /// <returns>An enumerable set of <see cref="PropertyInfo"/> instances.</returns>
        protected abstract IEnumerable<PropertyInfo> GetCandidateProperties();

        private static void AssertThatClassHasCandidateProperties(IEnumerable<PropertyInfo> candidates)
        {
            if(candidates.Count() > 0) return;

            var msg = string.Format(
                "\n{0} does not have any public properties with setters.\n" +
                "Asserting change notification is without meaning.",
                typeof(T).Name);
            throw new Exception(msg);
        }

        private bool DoesPropertyRaiseNotification(PropertyInfo propertyInfo)
        {
            var has_property_changed = false;

            _propertyOwner.PropertyChanged +=
                (s, e) => { if(e.PropertyName == propertyInfo.Name) has_property_changed = true; };

            var valueToSet = GetSetterValueForProperty(propertyInfo);
            propertyInfo.SetValue(_propertyOwner, valueToSet, null);

            return has_property_changed;
        }

        private object GetSetterValueForProperty(PropertyInfo propertyInfo)
        {
            return (_values.ContainsKey(propertyInfo))
                       ? _values[propertyInfo]
                       : Default(propertyInfo.PropertyType);
        }

        private static object Default(Type type)
        {
            return type.IsValueType
                       ? Activator.CreateInstance(type)
                       : null;
        }
    }
}