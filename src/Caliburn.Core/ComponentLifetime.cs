namespace Caliburn.Core
{
    /// <summary>
    /// The lifetime of a Caliburn component.
    /// </summary>
    public enum ComponentLifetime
    {
        /// <summary>
        /// One instance per application.
        /// </summary>
        Singleton,
        /// <summary>
        /// A new instance is created on each request.
        /// </summary>
        PerRequest,
        /// <summary>
        /// A new instance is created per custom lifetime rules.
        /// </summary>
        Custom
    }
}