namespace ContactManager.Services
{
    using System;
    using Caliburn.Core.IoC;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.WPF.ApplicationFramework;
    using Interfaces;

    [Singleton(typeof(ISettings))]
    public class Settings : SettingsBase, ISettings
    {
        public TimeSpan EarliestAppointment
        {
            get
            {
                var milliseconds = this.Get("EarliestAppointment", TimeSpan.FromHours(9).TotalMilliseconds);
                return TimeSpan.FromMilliseconds(milliseconds);
            }
            set
            {
                InsertOrUpdate("EarliestAppointment", value.TotalMilliseconds.ToString());
                NotifyOfPropertyChange("EarliestAppointment");
            }
        }

        public TimeSpan LatestAppointment
        {
            get
            {
                var milliseconds = this.Get("LatestAppointment", TimeSpan.FromHours(17).TotalMilliseconds);
                return TimeSpan.FromMilliseconds(milliseconds);
            }
            set
            {
                InsertOrUpdate("LatestAppointment", value.TotalMilliseconds.ToString());
                NotifyOfPropertyChange("LatestAppointment");
            }
        }
    }
}