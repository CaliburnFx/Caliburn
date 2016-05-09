using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Markup;

[assembly: AssemblyTitle("Caliburn Core Library")]
[assembly: Guid("b4d684b5-33d5-41ac-b1c6-cc26555cadfd")]

#if !SILVERLIGHT
[assembly: CLSCompliant(true)]
#endif

[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.Actions")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.Commands")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.ApplicationModel")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.ViewModels")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.Views")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.RoutedMessaging")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.RoutedMessaging.Triggers")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.Converters")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.ShellFramework.Menus")]
[assembly: XmlnsPrefix("http://www.caliburnproject.org", "cal")]

#if !NO_PARTIAL_TRUST
[assembly: AllowPartiallyTrustedCallers]
#endif