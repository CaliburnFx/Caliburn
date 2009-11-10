namespace Caliburn.Core.IoC
{
    using System;

    /// <summary>
    /// Represents a Custom Lifetime registration.
    /// </summary>
    public class CustomLifetime : ComponentRegistrationBase
    {
        /// <summary>
        /// Gets or sets the lifetime.
        /// </summary>
        /// <value>The lifetime.</value>
        public Type Lifetime { get; set; }

        /// <summary>
        /// Gets or sets the implementation.
        /// </summary>
        /// <value>The implementation.</value>
        public Type Implementation { get; set; }
    }
}