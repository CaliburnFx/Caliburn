using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Markup;

[assembly: AssemblyTitle("Caliburn Presentation Framework")]
[assembly: Guid("3b851787-cea3-47da-aad7-f4a52fe5ef03")]

#if !SILVERLIGHT
[assembly: CLSCompliant(true)]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.Actions")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.Triggers")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.Commands")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.ApplicationModel")]
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.PresentationFramework.ViewModels")]
#endif

#if !NO_PARTIAL_TRUST
[assembly: AllowPartiallyTrustedCallers]
#endif