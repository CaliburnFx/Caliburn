namespace Caliburn.ShellFramework.Resources
{
    using System.IO;

    public interface IResourceManager
    {
        Stream GetStream(string relativeUri, string assemblyName);
    }
}