namespace ContactManager.Model.Interrogators
{
    using System;
    using Caliburn.ModelFramework;
    using Caliburn.WPF.ApplicationFramework;
    using Services.Interfaces;

    public class AppointmentRange : IPropertyValidator<TimeSpan>
    {
        private readonly ISettings _settings;

        public AppointmentRange(ISettings settings)
        {
            _settings = settings;
        }

        public bool Interrogate(IProperty<TimeSpan> instance)
        {
            if(instance.Value < _settings.EarliestAppointment ||
               instance.Value > _settings.LatestAppointment)
            {
                instance.ValidationResults.Add(
                    new ValidationResult(
                        string.Format(
                            "Appointments must be made between {0} and {1}.",
                            _settings.EarliestAppointment,
                            _settings.LatestAppointment
                            ),
                        instance
                        )
                    );

                return false;
            }

            return true;
        }
    }
}