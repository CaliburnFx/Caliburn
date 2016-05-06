namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System;

    /// <summary>
    /// Represents a parameter that is required for message binding.
    /// </summary>
    public class RequiredParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredParameter"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        public RequiredParameter(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; private set; }
    }
}