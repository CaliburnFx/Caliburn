namespace Tests.Caliburn.Adapters.Components
{
    using System;
    using System.ComponentModel.Composition;

    [Export(typeof(ILogger)), Export(typeof(SimpleLogger))]
    public class SimpleLogger : ILogger
    {
        public void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}