using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyTitle("Caliburn Model Framework")]
[assembly: Guid("a5723c01-b843-4179-a629-ea9d6a5ae5aa")]

#if !SILVERLIGHT
[assembly: CLSCompliant(true)]
#endif

#if !NO_PARTIAL_TRUST
[assembly: AllowPartiallyTrustedCallers]
#endif