namespace Caliburn.DynamicProxy.Interceptors
{
    using System;
    using System.ComponentModel;
	using Castle.DynamicProxy;
	using PresentationFramework.Behaviors;
    using PresentationFramework.Invocation;

    /// <summary>
    /// Handles <see cref="INotifyPropertyChanged"/> on classes that do not implement the interface.
	/// </summary>
#if NET
	[System.Serializable]
#endif
	public class NotifyPropertyChangedWithInterfaceInterceptor : NotifyPropertyChangedBaseInterceptor
    {
        PropertyChangedEventHandler handler = delegate { };

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyPropertyChangedWithInterfaceInterceptor"/> class.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="behavior">The behavior.</param>
        public NotifyPropertyChangedWithInterfaceInterceptor(Type implementation, NotifyPropertyChangedAttribute behavior) 
            : base(implementation, behavior) {}

        /// <summary>
        /// Indicates whether the invocation should proceed.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <returns></returns>
        protected override bool ShouldProceed(IInvocation invocation)
        {
            if (!invocation.Method.DeclaringType.Equals(typeof(INotifyPropertyChanged)))
                return true;

            if(invocation.Method.Name == "add_PropertyChanged")
                AddHandler((Delegate)invocation.Arguments[0]);
            else if(invocation.Method.Name == "remove_PropertyChanged")
                RemoveHandler((Delegate)invocation.Arguments[0]);

            return false;
        }

        private void AddHandler(Delegate @delegate)
        {
            handler = (PropertyChangedEventHandler)Delegate.Combine(handler, @delegate);
        }

        private void RemoveHandler(Delegate @delegate)
        {
            handler = (PropertyChangedEventHandler)Delegate.Remove(handler, @delegate);
        }

        /// <summary>
        /// Called to raise a property change notification.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected override void OnPropertyChanged(object sender, string propertyName)
        {
            Execute.OnUIThread(() => handler(sender, new PropertyChangedEventArgs(propertyName)));
        }
    }
}