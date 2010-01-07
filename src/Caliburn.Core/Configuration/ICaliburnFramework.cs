namespace Caliburn.Core.Configuration
{
    /// <summary>
    /// Represents the Caliburn framework.
    /// </summary>
    public interface ICaliburnFramework
    {
        /// <summary>
        /// Adds a module to the famework.
        /// </summary>
        /// <param name="module">The module.</param>
        void AddModule(IModule module);

        /// <summary>
        /// Starts the framework.
        /// </summary>
        void Start();
    }
}