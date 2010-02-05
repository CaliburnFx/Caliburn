namespace Caliburn.ShellFramework.Resources
{
    using System.IO;
    using System.Reflection;
    using System.Windows.Markup;
    using System.Windows.Media.Imaging;

#if SILVERLIGHT
    using System.Windows.Documents;
#endif

    public static class ResourceExtensions
    {
        public static string GetExecutingAssemblyName()
        {
            return Assembly.GetExecutingAssembly().GetAssemblyName();
        }

        public static string GetAssemblyName(this Assembly assembly)
        {
            string name = assembly.FullName;
            return name.Substring(0, name.IndexOf(','));
        }

        public static Stream GetStream(this IResourceManager resourceManager, string relativeUri)
        {
            return resourceManager.GetStream(relativeUri, GetExecutingAssemblyName());
        }

        public static BitmapImage GetBitmap(this IResourceManager resourceManager, string relativeUri, string assemblyName)
        {
            var stream = resourceManager.GetStream(relativeUri, assemblyName);
            if (stream == null) return null;

            using (stream)
            {
                var bmp = new BitmapImage();

#if SILVERLIGHT
                bmp.SetSource(stream);
#else
                bmp.BeginInit();
                bmp.StreamSource = stream;
                bmp.EndInit();
                bmp.Freeze();
#endif

                return bmp;
            }
        }

        public static BitmapImage GetBitmap(this IResourceManager resourceManager, string relativeUri)
        {
            return resourceManager.GetBitmap(relativeUri, GetExecutingAssemblyName());
        }

        public static string GetString(this IResourceManager resourceManager, string relativeUri, string assemblyName)
        {
            var stream = resourceManager.GetStream(relativeUri, assemblyName);
            if (stream == null) return null;

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string GetString(this IResourceManager resourceManager, string relativeUri)
        {
            return resourceManager.GetString(relativeUri, GetExecutingAssemblyName());
        }

#if SILVERLIGHT

        public static FontSource GetFontSource(this IResourceManager resourceManager, string relativeUri, string assemblyName)
        {
            var stream = resourceManager.GetStream(relativeUri, assemblyName);
            if (stream == null) return null;

            using (stream)
            {
                return new FontSource(stream);
            }
        }

        public static FontSource GetFontSource(this IResourceManager resourceManager, string relativeUri)
        {
            return resourceManager.GetFontSource(relativeUri, GetExecutingAssemblyName());
        }

#endif

        public static object GetXamlObject(this IResourceManager resourceManager, string relativeUri, string assemblyName)
        {
#if SILVERLIGHT
            string str = resourceManager.GetString(relativeUri, assemblyName);
            if(string.IsNullOrEmpty(str)) return null;

            return XamlReader.Load(str);
#else
            var stream = resourceManager.GetStream(relativeUri, assemblyName);
            if (stream == null) return null;

            using(stream)
            {
                return XamlReader.Load(stream);
            }
#endif
        }

        public static object GetXamlObject(this IResourceManager resourceManager, string relativeUri)
        {
            return resourceManager.GetXamlObject(relativeUri, GetExecutingAssemblyName());
        }
    }
}