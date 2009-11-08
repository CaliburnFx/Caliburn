namespace ContactManager.Model
{
    using System;
    using System.Collections.Generic;
    using Caliburn.ModelFramework;
    using Caliburn.Silverlight.ApplicationFramework.Interrogators;

    public class Contact : ModelBase
    {
        public Guid ID { get; set; }

        public static readonly IPropertyDefinition<string> FirstNameProperty =
            Property<Contact, string>(x => x.FirstName)
                .MustNotBeBlank("You must provide a first name.");

        public string FirstName
        {
            get { return GetValue(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        public static readonly IPropertyDefinition<string> LastNameProperty =
            Property<Contact, string>(x => x.LastName)
                .MustNotBeBlank("You must provide a last name.");

        public string LastName
        {
            get { return GetValue(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        public static readonly IPropertyDefinition<Address> AddressProperty =
            Association<Contact, Address>(x => x.Address);

        public Address Address
        {
            get { return GetValue(AddressProperty); }
        }

        public static readonly IPropertyDefinition<ICollectionNode<PhoneNumber>> NumbersProperty =
            Collection<Contact, PhoneNumber>(x => x.Numbers)
                .HasMinimumCountOf(1);

        public IList<PhoneNumber> Numbers
        {
            get { return GetValue(NumbersProperty); }
        }

        public void AddPhoneNumber(PhoneNumber number)
        {
            Numbers.Add(number);
        }

        public void RemovePhoneNumber(PhoneNumber numberToRemove)
        {
            Numbers.Remove(numberToRemove);
        }
    }
}