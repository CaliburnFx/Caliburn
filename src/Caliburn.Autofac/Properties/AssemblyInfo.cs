using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyTitle("Caliburn.Autofac")]
[assembly: Guid("25e1498f-a1c2-4a2b-979e-9ca09190debe")]
[assembly: CLSCompliant(true)]

#if !NO_PARTIAL_TRUST
[assembly: AllowPartiallyTrustedCallers]
#endif