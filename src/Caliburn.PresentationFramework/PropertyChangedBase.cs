namespace Caliburn.PresentationFramework
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using Core;
    using Invocation;

    /// <summary>
    /// A base class that implements the infrastructure for property change notification and automatically performs UI thread marshalling.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class PropertyChangedBase : INotifyPropertyChangedEx
    {

#if !SILVERLIGHT
		[System.Runtime.Serialization.OnDeserialized]
		private void __OnDeserialized(System.Runtime.Serialization.StreamingContext context)
		{
			PropertyChanged = delegate { };
		}
#endif

		/// <summary>
        /// Occurs when a property value changes.
        /// </summary>
#if !SILVERLIGHT
		[field: NonSerialized]
#endif
		public virtual event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public virtual void NotifyOfPropertyChange(string propertyName)
        {
            Execute.OnUIThread(() => RaisePropertyChangedEventImmediately(propertyName));
        }

        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        public void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            NotifyOfPropertyChange(propertyExpression.GetMemberInfo().Name);
        }

        /// <summary>
        /// Raises the property changed event immediately.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public virtual void RaisePropertyChangedEventImmediately(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}