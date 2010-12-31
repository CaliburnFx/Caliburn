namespace Caliburn.Testability.Extensions
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using Assertions;

    /// <summary>
    /// Hosts extension methods used to make assertions about property notification.
    /// </summary>
    public static class PropertyAssertionExtensions
    {
        /// <summary>
        /// Sets up an assertion about a single property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="propertyOwner">The property owner.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public static SinglePropertyAssertion<T, K> AssertThatProperty<T, K>(this T propertyOwner, Expression<Func<T, K>> property)
            where T : class, INotifyPropertyChanged
        {
            return new SinglePropertyAssertion<T, K>(propertyOwner, property);
        }

        /// <summary>
        /// Makes an assertion for all properties of an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyOwner">The property owner.</param>
        /// <returns></returns>
        public static AllPropertiesAssertion<T> AssertThatAllProperties<T>(this T propertyOwner)
            where T : class, INotifyPropertyChanged
        {
            return new AllPropertiesAssertion<T>(propertyOwner);
        }

        /// <summary>
        /// Gets the property info.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<T, K>(this Expression<Func<T, K>> property)
        {
            var memberExpression = (MemberExpression)property.Body;
            return (PropertyInfo)memberExpression.Member;
        }


        /// <summary>
        /// Sets up an assertion for a given property with the expectation that change notification will be raised 
        /// under certain conditions.
        /// </summary>
        /// <typeparam name="T">The type of the property owner.</typeparam>
        /// <typeparam name="K">The type of the property.</typeparam>
        /// <param name="propertyOwner">The property owner.</param>
        /// <param name="property">The property of interest.</param>
        /// <returns></returns>
        public static PropertyHasChangedAssertion<T, K> AssertThatChangeNotificationIsRaisedBy<T, K>(this T propertyOwner, Expression<Func<T, K>> property)
            where T : INotifyPropertyChanged
        {
            return new PropertyHasChangedAssertion<T, K>(propertyOwner, property);
        }
    }
}