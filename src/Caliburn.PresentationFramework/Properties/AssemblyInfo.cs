using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Markup;

[assembly: AssemblyTitle("Caliburn Presentation Framework")]
[assembly: Guid("3b851787-cea3-47da-aad7-f4a52fe5ef03")]

#if !SILVERLIGHT
[assembly: CLSCompliant(true)]
#endif

#if NET || SILVERLIGHT_40
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.Actions")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.Commands")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.ApplicationModel")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.ViewModels")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.Views")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.RoutedMessaging")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.RoutedMessaging.Triggers")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.Converters")]
#endif

#if !NO_PARTIAL_TRUST
[assembly: AllowPartiallyTrustedCallers]
#endif