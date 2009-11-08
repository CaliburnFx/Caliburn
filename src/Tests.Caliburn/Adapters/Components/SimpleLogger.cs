using System;
using System.ComponentModel.Composition;

namespace Tests.Caliburn.Adapters.Components
{
    [Export(typeof(ILogger))]
    [Export(typeof(SimpleLogger))]
    public class SimpleLogger : ILogger
    {
        public void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}