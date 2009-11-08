namespace Tests.Caliburn.Fakes.ViewModel
{
    using System.Collections.Generic;
    using global::Caliburn.ModelFramework;

    public class Contact : ModelBase
    {
        public static readonly IPropertyDefinition<string> FirstNameProperty =
            Property<Contact, string>(x => x.FirstName);

        public string FirstName
        {
            get { return GetValue(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        public static readonly IPropertyDefinition<string> LastNameProperty =
            Property<Contact, string>(x => x.LastName);

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
            Collection<Contact, PhoneNumber>(x => x.Numbers);

        public IList<PhoneNumber> Numbers
        {
            get { return GetValue(NumbersProperty); }
        }
    }
}