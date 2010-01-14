namespace Caliburn.PresentationFramework.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Core.Behaviors;

    /// <summary>
    /// Applies a behavior which implements <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    public class NotifyPropertyChangedAttribute : Attribute, IBehavior
    {
        /// <summary>
        /// Gets the interfaces which represent this behavior.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <returns>The representative interfaces.</returns>
        public IEnumerable<Type> GetInterfaces(Type implementation)
        {
            if(!typeof(INotifyPropertyChanged).IsAssignableFrom(implementation))
                yield return typeof(INotifyPropertyChanged);

            yield return typeof(IProxy);
        }
    }
}