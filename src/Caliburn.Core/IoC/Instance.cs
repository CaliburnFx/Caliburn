namespace Caliburn.Core.IoC
{
    /// <summary>
    /// Represents the registration of an existing instance.
    /// </summary>
    public class Instance : ComponentRegistrationBase
    {
        /// <summary>
        /// Gets or sets the implementation.
        /// </summary>
        /// <value>The implementation.</value>
        public object Implementation { get; set; }
    }
}