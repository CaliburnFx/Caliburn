namespace Caliburn.WPF.ApplicationFramework.Interrogators
{
    using ModelFramework;

    public class NotNull<T> : IPropertyValidator<T>
        where T : class
    {
        public NotNull()
            : this("{0} cannot be null.") {}

        public NotNull(string message)
        {
            Message = message;
        }

        public string Message { get; set; }

        public bool Interrogate(IProperty<T> instance)
        {
            var value = instance.Value;

            if(value == null)
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