namespace Tests.Caliburn.Adapters.Components
{
    using System;
    
    public class AdvancedLogger : ILogger
    {
        public void Log(string msg)
        {
            Console.WriteLine("Log: {0}", msg);
        }
    }
}