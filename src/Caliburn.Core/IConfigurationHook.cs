namespace Caliburn.Core
{
    /// <summary>
    /// Enables other Caliburn modules to hook into the core configuration.
    /// </summary>
    public interface IConfigurationHook
    {
        /// <summary>
        /// Gets the core configuration.
        /// </summary>
        /// <value>The core configuration.</value>
        CoreConfiguration Core { get; }
    }
}