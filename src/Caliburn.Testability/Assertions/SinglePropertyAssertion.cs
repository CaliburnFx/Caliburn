namespace Caliburn.Testability.Assertions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using Extensions;

    /// <summary>
    /// Builds up a set of assertions for a single property of an object.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="INotifyPropertyChanged"/>.</typeparam>
    /// <typeparam name="K">The type of the property the assertion is applied to.</typeparam>
    public class SinglePropertyAssertion<T, K> : PropertyAssertionBase<T>
        where T : class, INotifyPropertyChanged
    {
        private readonly Expression<Func<T, K>> _property;

        /// <summary>
        /// Initializes a new instance of the <see cref="SinglePropertyAssertion&lt;T, K&gt;"/> class.
        /// </summary>
        /// <param name="propertyOwner">The property owner.</param>
        /// <param name="property">The property.</param>
        public SinglePropertyAssertion(INotifyPropertyChanged propertyOwner, Expression<Func<T, K>> property)
            : base(propertyOwner)
        {
            _property = property;
        }

        /// <summary>
        /// Gets the candidate properties.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<PropertyInfo> GetCandidateProperties()
        {
            return new[] {_property.GetPropertyInfo()};
        }

        /// <summary>
        /// Raises the change notification.
        /// </summary>
        public void RaisesChangeNotification()
        {
            Execute();
        }
    }
}