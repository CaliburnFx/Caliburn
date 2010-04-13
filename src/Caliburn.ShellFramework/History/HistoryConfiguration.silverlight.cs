#if SILVERLIGHT && !WP7

namespace Caliburn.ShellFramework.History
{
    using System;
    using PresentationFramework.Screens;

    public class HistoryConfiguration
    {
        public HistoryConfiguration()
        {
            StateName = "Default";
            AlterTitle = (oldTitle, screen) => screen.DisplayName;
            ScreenNotFound = delegate { };
        }

        public string StateName { get; set; }
        public IScreenConductor Host { get; set; }
        public string HistoryKey { get; set; }
        public Func<string, IScreen> DetermineScreen { get; set; }
        public Func<string, IScreen, string> AlterTitle { get; set; }
        public Action<string> ScreenNotFound { get; set; }
    }
}

#endif