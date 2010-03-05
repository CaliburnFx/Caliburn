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
        /// Gets or sets the default dependency mode.
        /// </summary>
        public static DependencyMode DefaultDependencyMode = DependencyMode.AlwaysRecord;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyPropertyChangedAttribute"/> class.
        /// </summary>
        public NotifyPropertyChangedAttribute()
        {
            DependencyMode = DefaultDependencyMode;
        }

        /// <summary>
        /// Gets or sets the dependency mode.
        /// </summary>
        /// <value>The dependency mode.</value>
        public DependencyMode DependencyMode { get; set; }

        /// <summary>
        /// Gets the interfaces which represent this behavior.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <returns>The representative interfaces.</returns>
        public IEnumerable<Type> GetInterfaces(Type implementation)
        {
            if(!typeof(INotifyPropertyChanged).IsAssignableFrom(implementation))
                yield return typeof(INotifyPropertyChangedEx);

            yield return typeof(IProxy);
        }
    }
}