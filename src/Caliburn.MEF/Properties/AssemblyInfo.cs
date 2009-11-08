using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyTitle("Caliburn.MEF")]
[assembly: Guid("8e2b4fe0-d1a1-47b9-8e43-91c0235e38dd")]
[assembly: CLSCompliant(true)]

#if !NO_PARTIAL_TRUST
[assembly: AllowPartiallyTrustedCallers]
#endif