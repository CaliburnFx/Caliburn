namespace ContactManager.Services.Interfaces
{
    using System;
    using Model;

    public interface IScheduleService
    {
        DailySchedule GetScheduleFor(DateTime date);
        void SaveAll();
        void CancelChanges();
    }
}