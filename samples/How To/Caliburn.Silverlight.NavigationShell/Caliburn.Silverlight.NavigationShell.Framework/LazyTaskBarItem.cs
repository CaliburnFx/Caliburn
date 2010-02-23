namespace Caliburn.Silverlight.NavigationShell.Framework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Media.Imaging;
    using Core;
    using Microsoft.Practices.ServiceLocation;
    using PresentationFramework;
    using PresentationFramework.Filters;
    using ShellFramework.Results;

    public class LazyTaskBarItem : ITaskBarItem
    {
        private readonly string _iconName;
        private readonly string _displayName;
        private readonly string _assemblyName;

        private IEntryPoint _actualEntryPoint;

        public LazyTaskBarItem(string displayName, string assemblyName, string iconName)
        {
            _iconName = iconName;
            _displayName = displayName;
            _assemblyName = assemblyName;
        }

        public BitmapImage Icon
        {
            get { return new BitmapImage(GetUri(_iconName)); }
        }

        public string DisplayName
        {
            get { return _displayName; }
        }

        [Rescue("OpenFailed")]
        public IEnumerable<IResult> Enter()
        {
            if(_actualEntryPoint == null)
            {
                yield return Show.Busy(new BusyScreen());

                var request = new WebClientResult(GetUri(_assemblyName));
                yield return request;

                LoadAndConfigureModule(request.Stream);

                yield return Show.NotBusy();
            }

            foreach(var result in _actualEntryPoint.Enter())
            {
                yield return result;
            }
        }

        public bool OpenFailed(Exception exception)
        {
            Show.NotBusy().Execute();
            Show.MessageBox("There was a problem downloading the module.", "Error").Execute();

            return true;
        }

        private void LoadAndConfigureModule(Stream stream) 
        {
            var part = new AssemblyPart();
            var assembly = part.Load(stream);

            _actualEntryPoint = (IEntryPoint)Activator.CreateInstance(
                                         assembly.GetExportedTypes().First(
                                             x => typeof(IEntryPoint).IsAssignableFrom(x))
                                         );

            ServiceLocator.Current.GetInstance<IAssemblySource>()
                .Add(assembly);
        }

        private static Uri GetUri(string resource)
        {
            var uri = string.Format(
                "{0}://{1}:{2}/ClientBin/Modules/{3}",
                HtmlPage.Document.DocumentUri.Scheme,
                HtmlPage.Document.DocumentUri.Host,
                HtmlPage.Document.DocumentUri.Port,
                resource
                );

            return new Uri(uri);
        }
    }
}