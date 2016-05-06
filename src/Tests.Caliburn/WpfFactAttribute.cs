using System;
using Xunit;
using Xunit.Sdk;

namespace Tests.Caliburn
{
    [AttributeUsage(AttributeTargets.Method)]
    [XunitTestCaseDiscoverer("Tests.Caliburn.WpfFactDiscoverer", "Tests.Caliburn")]
    public class WpfFactAttribute : FactAttribute { }
}