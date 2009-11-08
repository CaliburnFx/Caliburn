namespace Caliburn.WPF.ApplicationFramework.Interrogators
{
    using ModelFramework;

    public class NotBlank : IPropertyValidator<string>
    {
        public NotBlank()
            : this("{0} cannot be blank.") {}

        public NotBlank(string message)
        {
            Message = message;
        }

        public string Message { get; set; }

        public bool Interrogate(IProperty<string> instance)
        {
            var value = instance.Value;

            if(string.IsNullOrEmpty(value) || value.Trim().Length < 1)
            {
                instance.ValidationResults.Add(
                    new ValidationResult(
                        string.Format(Message, instance.Definition.Name),
                        instance
                        )
                    );
                return false;
            }

            return true;
        }
    }
}