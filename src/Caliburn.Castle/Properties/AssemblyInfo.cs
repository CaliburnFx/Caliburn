using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyTitle("Caliburn.Castle")]
[assembly: Guid("d2d66755-05d1-4998-88b9-91a8b4cf9c8e")]
[assembly: CLSCompliant(true)]

#if !NO_PARTIAL_TRUST
[assembly: AllowPartiallyTrustedCallers]
#endif