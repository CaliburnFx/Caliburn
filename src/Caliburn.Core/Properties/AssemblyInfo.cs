using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyTitle("Caliburn Core Library")]
[assembly: Guid("b4d684b5-33d5-41ac-b1c6-cc26555cadfd")]

#if !SILVERLIGHT
[assembly: CLSCompliant(true)]
#endif

#if !NO_PARTIAL_TRUST
[assembly: AllowPartiallyTrustedCallers]
#endif