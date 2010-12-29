namespace Caliburn.ShellFramework.Resources
{
    using System.IO;

    /// <summary>
    /// A service capable of locating application resources.
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        /// Gets a resource stream.
        /// </summary>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>The stream, or null if not found.</returns>
        Stream GetStream(string relativeUri, string assemblyName);
    }
}