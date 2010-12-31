namespace Caliburn.Testability.Assertions
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using Extensions;

    /// <summary>
    /// Builds up an assertion that a given property raises change notification when the 
    /// a given condition is executed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K">The type of the property.</typeparam>
    public class PropertyHasChangedAssertion<T, K>
        where T : INotifyPropertyChanged
    {
        readonly T owner;
        readonly Expression<Func<T, K>> property;
        bool isValidAssertion;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyHasChangedAssertion&lt;T, K&gt;"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="property">The property.</param>
        public PropertyHasChangedAssertion(T owner, Expression<Func<T, K>> property)
        {
            this.owner = owner;
            this.property = property;
        }

        /// <summary>
        /// Affects the property.
        /// </summary>
        /// <param name="affectProperty">The affect property.</param>
        public void When(Action affectProperty)
        {
            isValidAssertion = true;
            bool notification_was_raised = false;

            PropertyInfo property_info = property.GetPropertyInfo();

            owner.PropertyChanged += (s, e) =>{
                if(e.PropertyName == property_info.Name)
                    notification_was_raised = true;
            };

            affectProperty();

            if(!notification_was_raised)
            {
                string msg = "The property " + property_info.Name + " did not raise change notification.";
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="PropertyHasChangedAssertion&lt;T, K&gt;"/> is reclaimed by garbage collection.
        /// </summary>
        ~PropertyHasChangedAssertion()
        {
            if(!isValidAssertion)
                throw new Exception(
                    "No context was provided to test the notification, use When(Action affectProperty) to provide a context.");
        }
    }
}