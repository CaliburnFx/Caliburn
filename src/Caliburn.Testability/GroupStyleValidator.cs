namespace Caliburn.Testability
{
    /// <summary>
    /// Validates a <see cref="GroupStyleValidator"/>.
    /// </summary>
    public class GroupStyleValidator
    {
        private readonly ElementEnumeratorSettings _settings;
        private readonly GroupStyleElement _element;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupStyleValidator"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="element">The element.</param>
        public GroupStyleValidator(ElementEnumeratorSettings settings, GroupStyleElement element)
        {
            _settings = settings;
            _element = element;
        }

        /// <summary>
        /// Validates the bindings.
        /// </summary>
        /// <returns></returns>
        public ValidationResult ValidateBindings()
        {
            var result = new ValidationResult();
            CheckAmbiguities(result);
            return result;
        }

        private void CheckAmbiguities(ValidationResult result)
        {
            if(_element.Style.ContainerStyle != null &&
               _element.Style.ContainerStyleSelector != null)
            {
                result.Add(
                    Error.StyleSelectorAmbiguity(_element, _element.Type, "ContainerStyle")
                    );
            }

            if(_element.Style.HeaderTemplate != null &&
               _element.Style.HeaderTemplateSelector != null)
            {
                result.Add(
                    Error.TemplateSelectorAmbiguity(_element, _element.Type, "HeaderTemplate")
                    );
            }
        }
    }
}