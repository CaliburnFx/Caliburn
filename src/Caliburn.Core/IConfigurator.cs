namespace Caliburn.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Implemented by a class that can configure the componets with a container.
    /// </summary>
    public interface IConfigurator
    {
        /// <summary>
        /// Configures the container with the provided components.
        /// </summary>
        /// <param name="components">The components.</param>
        void ConfigureWith(IEnumerable<ComponentInfo> components);
    }
}