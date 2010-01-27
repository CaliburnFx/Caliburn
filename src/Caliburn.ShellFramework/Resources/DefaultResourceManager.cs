namespace Caliburn.ShellFramework.Resources
{
    using System;
    using System.IO;
    using System.Windows;

    public class DefaultResourceManager : IResourceManager
    {
        public Stream GetStream(string relativeUri, string assemblyName)
        {
            try
            {
                var resource = Application.GetResourceStream(new Uri(assemblyName + ";component/" + relativeUri, UriKind.Relative))
                               ?? Application.GetResourceStream(new Uri(relativeUri, UriKind.Relative));

                return (resource != null)
                           ? resource.Stream
                           : null;
            }
            catch
            {
                return null;
            }
        }
    }
}