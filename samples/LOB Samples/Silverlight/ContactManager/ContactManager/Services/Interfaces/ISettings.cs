namespace ContactManager.Services.Interfaces
{
    using System;
    using System.ComponentModel;

    public interface ISettings : INotifyPropertyChanged
    {
        TimeSpan EarliestAppointment { get; set; }
        TimeSpan LatestAppointment { get; set; }

        void Save();
        void Load();
    }
}