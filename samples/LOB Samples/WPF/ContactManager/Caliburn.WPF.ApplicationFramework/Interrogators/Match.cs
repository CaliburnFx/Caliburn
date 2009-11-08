namespace Caliburn.WPF.ApplicationFramework.Interrogators
{
    using System.Text.RegularExpressions;
    using ModelFramework;

    public class Match : IPropertyValidator<string>
    {
        private readonly Regex _regex;

        public Match(string pattern)
            : this("{0} is not in a valid format.", pattern) {}

        public Match(string message, string pattern)
        {
            Message = message;
            _regex = new Regex(pattern);
        }

        public string Message { get; set; }

        public bool Interrogate(IProperty<string> instance)
        {
            var value = instance.Value;

            if(string.IsNullOrEmpty(value) || value.Trim().Length < 1)
            {
                AddValidationResult(instance);
                return false;
            }

            if(!_regex.IsMatch(value))
            {
                AddValidationResult(instance);
                return false;
            }

            return true;
        }

        private void AddValidationResult(IProperty<string> instance)
        {
            instance.ValidationResults.Add(
                new ValidationResult(
                    string.Format(Message, instance.Definition.Name),
                    instance
                    )
                );
        }
    }
}