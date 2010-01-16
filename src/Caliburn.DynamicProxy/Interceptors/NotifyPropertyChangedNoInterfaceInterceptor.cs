namespace Caliburn.DynamicProxy.Interceptors
{
    using System;
    using System.ComponentModel;
    using Core;
    using PresentationFramework.Behaviors;

    /// <summary>
    /// Handles <see cref="INotifyPropertyChanged"/> on classes that already implement the interface.
    /// </summary>
    public class NotifyPropertyChangedNoInterfaceInterceptor : NotifyPropertyChangedBaseInterceptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyPropertyChangedNoInterfaceInterceptor"/> class.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="behavior">The behavior.</param>
        public NotifyPropertyChangedNoInterfaceInterceptor(Type implementation, NotifyPropertyChangedAttribute behavior) 
            : base(implementation, behavior) {}

        /// <summary>
        /// Called to raise a property change notification.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected override void OnPropertyChanged(object sender, string propertyName)
        {
            var notifier = sender as INotifyPropertyChangedEx;
            if(notifier != null) notifier.NotifyOfPropertyChange(propertyName);
        }
    }
}