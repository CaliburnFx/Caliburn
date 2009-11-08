namespace ContactManager.Model
{
    using System;
    using Caliburn.ModelFramework;
    using Caliburn.PresentationFramework;
    using Caliburn.WPF.ApplicationFramework.Interrogators;
    using Interrogators;

    public class Appointment : ModelBase
    {
        private BindableCollection<Contact> _allContacts = new BindableCollection<Contact>();

        public BindableCollection<Contact> AllContacts
        {
            get { return _allContacts; }
            set { _allContacts = value; }
        }

        public static readonly IPropertyDefinition<TimeSpan> TimeProperty =
            Property<Appointment, TimeSpan>(x => x.Time)
                .IsConstrainedToUserSettings();

        public TimeSpan Time
        {
            get { return GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly IPropertyDefinition<Contact> ContactProperty =
            Property<Appointment, Contact>(x => x.Contact)
                .MustNotBeNull("You must select a contact.");

        public Contact Contact
        {
            get { return GetValue(ContactProperty); }
            set { SetValue(ContactProperty, value); }
        }

        public static readonly IPropertyDefinition<string> DescriptionProperty =
            Property<Appointment, string>(x => x.Description)
                .MustNotBeBlank("You must provide a description.");

        public string Description
        {
            get { return GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
    }
}