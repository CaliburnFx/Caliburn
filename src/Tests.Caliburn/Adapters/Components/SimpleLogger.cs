namespace Tests.Caliburn.Adapters.Components
{
    using System;

    public class SimpleLogger : ILogger
    {
        public void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}