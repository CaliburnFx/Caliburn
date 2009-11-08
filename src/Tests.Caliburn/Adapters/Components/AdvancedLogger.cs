using System;
using System.ComponentModel.Composition;

namespace Tests.Caliburn.Adapters.Components
{
    [Export(typeof(ILogger))]
    [Export(typeof(AdvancedLogger))]
    public class AdvancedLogger : ILogger
    {
        public void Log(string msg)
        {
            Console.WriteLine("Log: {0}", msg);
        }
    }
}