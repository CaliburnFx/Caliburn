namespace ContactManager.Model
{
    using System;
    using Caliburn.ModelFramework;

    public class DailySchedule : ModelBase
    {
        public static readonly IPropertyDefinition<DateTime> DateProperty =
            Property<DailySchedule, DateTime>(x => x.Date);

        public DateTime Date
        {
            get { return GetValue(DateProperty); }
            set { SetValue(DateProperty, value.Date); }
        }

        public static readonly IPropertyDefinition<ICollectionNode<Appointment>> AppointmentsProperty =
            Collection<DailySchedule, Appointment>(x => x.Appointments);

        public ICollectionNode<Appointment> Appointments
        {
            get { return GetValue(AppointmentsProperty); }
        }

        public void AddAppointment(Appointment appointment)
        {
            Appointments.Add(appointment);
        }

        public void RemoveAppointment(Appointment appointmentToRemove)
        {
            Appointments.Remove(appointmentToRemove);
        }
    }
}