using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyTitle("Caliburn.Unity")]
[assembly: Guid("29711509-8011-4910-a553-f6f3cacc1c59")]
[assembly: CLSCompliant(true)]

#if !NO_PARTIAL_TRUST
[assembly: AllowPartiallyTrustedCallers]
#endif