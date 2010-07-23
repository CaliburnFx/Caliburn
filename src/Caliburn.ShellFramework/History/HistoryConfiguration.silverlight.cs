#if SILVERLIGHT

namespace Caliburn.ShellFramework.History
{
    using System;
    using PresentationFramework.Screens;

    public class HistoryConfiguration
    {
        public HistoryConfiguration()
        {
            StateName = "Default";
            ItemNotFound = delegate { };
            AlterTitle = (oldTitle, item) => item.DetermineDisplayName();
        }

        public string StateName { get; set; }
        public IConductor Conductor { get; set; }
        public string HistoryKey { get; set; }
        public Func<string, object> DetermineItem { get; set; }
        public Func<string, object, string> AlterTitle { get; set; }
        public Action<string> ItemNotFound { get; set; }
    }
}

#endif