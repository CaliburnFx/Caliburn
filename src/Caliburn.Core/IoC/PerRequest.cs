namespace Caliburn.Core.IoC
{
    using System;

    /// <summary>
    /// Represents a PerRequest registration.
    /// </summary>
    public class PerRequest : ComponentRegistrationBase
    {
        /// <summary>
        /// Gets or sets the implementation.
        /// </summary>
        /// <value>The implementation.</value>
        public Type Implementation { get; set; }
    }
}