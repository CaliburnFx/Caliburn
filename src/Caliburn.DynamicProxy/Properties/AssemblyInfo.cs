using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyTitle("Caliburn.DynamicProxy")]
[assembly: Guid("6425f90c-d984-46d2-a714-4225d438dba1")]
[assembly: CLSCompliant(true)]

#if !NO_PARTIAL_TRUST
[assembly: AllowPartiallyTrustedCallers]
#endif