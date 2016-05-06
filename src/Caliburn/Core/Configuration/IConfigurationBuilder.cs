namespace Caliburn.Core.Configuration
{
    /// <summary>
    /// Biulds Caliburn's configutation.
    /// </summary>
    public interface IConfigurationBuilder
    {
        /// <summary>
        /// Allows extension of the configuration.
        /// </summary>
        /// <value>The extensibility hook.</value>
        IModuleHook With { get; }

        /// <summary>
        /// Starts the framework.
        /// </summary>
        void Start();
    }
}