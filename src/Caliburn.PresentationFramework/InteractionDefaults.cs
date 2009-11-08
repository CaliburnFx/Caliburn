namespace Caliburn.PresentationFramework
{
    using System;

    /// <summary>
    /// Enables default functionality for UI-related elements.
    /// </summary>
    public abstract class InteractionDefaults
    {
        private readonly string _defaultEventName;
        private readonly Type _elementType;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionDefaults"/> class.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="defaultEventName">Name of the default event.</param>
        protected InteractionDefaults(Type elementType, string defaultEventName)
        {
            _elementType = elementType;
            _defaultEventName = defaultEventName;
        }

        /// <summary>
        /// Gets the type of the element to which the defaults apply.
        /// </summary>
        /// <value>The type of the element.</value>
        public Type ElementType
        {
            get { return _elementType; }
        }

        /// <summary>
        /// Gets the name of the default event used for UI wireup.
        /// </summary>
        /// <value>The event name.</value>
        public string DefaultEventName
        {
            get { return _defaultEventName; }
        }

        /// <summary>
        /// Gets the default trigger.
        /// </summary>
        /// <value>The default trigger.</value>
        public abstract IMessageTrigger GetDefaultTrigger();

        /// <summary>
        /// Gets the default value from the target.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns>The value.</returns>
        public abstract object GetDefaultValue(object target);

        /// <summary>
        /// Sets the default value on the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public abstract void SetDefaultValue(object target, object value);
    }
}