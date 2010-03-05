namespace ContactManager.Model.Interrogators
{
    using System;
    using Caliburn.ModelFramework;
    using Microsoft.Practices.ServiceLocation;
    using Services.Interfaces;

    public static class InterrogatorExtensions
    {
        public static IPropertyDefinition<TimeSpan> IsConstrainedToUserSettings(this IPropertyDefinition<TimeSpan> model)
        {
            model.Metadata.Add(new AppointmentRange(ServiceLocator.Current.GetInstance<ISettings>()));
            return model;
        }
    }
}