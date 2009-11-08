using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyTitle("Caliburn Test Library")]
[assembly: Guid("8ad1d314-c977-421b-af73-5d5e258d0696")]

#if !SILVERLIGHT
[assembly: CLSCompliant(true)]
#endif

#if !NO_PARTIAL_TRUST
[assembly: AllowPartiallyTrustedCallers]
#endif