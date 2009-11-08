namespace ContactManager.Model
{
    using Caliburn.ModelFramework;

    public class Address : ModelBase
    {
        private static readonly IPropertyDefinition<string> Street1Property =
            Property<Address, string>(x => x.Street1);

        public string Street1
        {
            get { return GetValue(Street1Property); }
            set { SetValue(Street1Property, value); }
        }

        private static readonly IPropertyDefinition<string> Street2Property =
            Property<Address, string>(x => x.Street2);

        public string Street2
        {
            get { return GetValue(Street2Property); }
            set { SetValue(Street2Property, value); }
        }

        private static readonly IPropertyDefinition<string> CityProperty =
            Property<Address, string>(x => x.City);

        public string City
        {
            get { return GetValue(CityProperty); }
            set { SetValue(CityProperty, value); }
        }

        private static readonly IPropertyDefinition<string> StateProperty =
            Property<Address, string>(x => x.State);

        public string State
        {
            get { return GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        private static readonly IPropertyDefinition<string> ZipProperty =
            Property<Address, string>(x => x.Zip);

        public string Zip
        {
            get { return GetValue(ZipProperty); }
            set { SetValue(ZipProperty, value); }
        }
    }
}