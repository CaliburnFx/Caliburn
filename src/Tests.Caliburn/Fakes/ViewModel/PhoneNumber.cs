namespace Tests.Caliburn.Fakes.ViewModel
{
    using global::Caliburn.ModelFramework;

    public class PhoneNumber : ModelBase
    {
        private static readonly IPropertyDefinition<string> NumberProperty =
            Property<PhoneNumber, string>(x => x.Number);

        public string Number
        {
            get { return GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        private static readonly IPropertyDefinition<PhoneNumberType> TypeProperty =
            Property<PhoneNumber, PhoneNumberType>(x => x.Type);

        public PhoneNumberType Type
        {
            get { return GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
    }
}