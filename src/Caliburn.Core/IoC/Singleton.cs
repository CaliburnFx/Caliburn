namespace Caliburn.Core.IoC
{
    using System;

    /// <summary>
    /// Represents a Singleton registration.
    /// </summary>
    public class Singleton : ComponentRegistrationBase
    {
        /// <summary>
        /// Gets or sets the implementation.
        /// </summary>
        /// <value>The implementation.</value>
        public Type Implementation { get; set; }
    }
}