namespace ContactManager.Model
{
    using Caliburn.ModelFramework;
    using Caliburn.Silverlight.ApplicationFramework.Interrogators;
    using Web;

    public class PhoneNumber : ModelBase
    {
        private static readonly IPropertyDefinition<string> NumberProperty =
            Property<PhoneNumber, string>(x => x.Number)
                .MustMatch(@"^\(?[1-9]\d{2}\)?([-., ])?[1-9]\d{2}([-., ])?\d{4}$", "You must provide a valid phone number with area code.");

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