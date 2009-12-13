namespace ContactManager.Services
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Core.IoC;
    using Interfaces;
    using Model;

    //NOTE: When building a real application, this service would generally call out to the file system or a web service.  I have used a lazy loaded Dictionary to simplify this sample.
    //NOTE: In a real application, I might not return a PresentationModel from the service.  I might return the message and then use the presenter to convert that into the appropriate model for that UI.  This dependens on the scenario.  Here, I have returned a PresentationModel for simplicity sake.
    [Singleton(typeof(IScheduleService))]
    public class ScheduleService : IScheduleService
    {
        private readonly Dictionary<DateTime, DailySchedule> _schedules = new Dictionary<DateTime, DailySchedule>();

        public DailySchedule GetScheduleFor(DateTime date)
        {
            date = date.Date;

            DailySchedule schedule;

            if(!_schedules.TryGetValue(date, out schedule))
            {
                schedule = new DailySchedule {Date = date};
                schedule.BeginEdit();

                _schedules.Add(date, schedule);
            }

            return schedule;
        }

        public void SaveAll()
        {
            foreach(var schedule in _schedules.Values)
            {
                schedule.EndEdit();
                schedule.BeginEdit();
            }
        }

        public void CancelChanges()
        {
            foreach(var schedule in _schedules.Values)
            {
                schedule.CancelEdit();
                schedule.BeginEdit();
            }
        }
    }
}