using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Markup;

[assembly: AssemblyTitle("Caliburn Shell Framework")]
[assembly: Guid("9d69ac84-e32e-4586-9ecd-345d1bb73041")]

#if NET || SILVERLIGHT_40
[assembly: XmlnsDefinition("http://www.caliburnproject.org", "Caliburn.ShellFramework.Menus")]
[assembly: XmlnsPrefix("http://www.caliburnproject.org", "cal")]
#endif

#if !NO_PARTIAL_TRUST
[assembly: AllowPartiallyTrustedCallers]
#endif