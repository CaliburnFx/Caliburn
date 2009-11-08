using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyTitle("Caliburn (Unit Tests)")]
[assembly: Guid("32c17916-11a8-4dff-8aa4-eb86c0516995")]

#if !NO_PARTIAL_TRUST
[assembly: AllowPartiallyTrustedCallers]
#endif