namespace Caliburn.Testability
{
    /// <summary>
    /// Validates a <see cref="GroupStyleValidator"/>.
    /// </summary>
    public class GroupStyleValidator
    {
        readonly ElementEnumeratorSettings settings;
        readonly GroupStyleElement element;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupStyleValidator"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="element">The element.</param>
        public GroupStyleValidator(ElementEnumeratorSettings settings, GroupStyleElement element)
        {
            this.settings = settings;
            this.element = element;
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

        void CheckAmbiguities(ValidationResult result)
        {
            if(element.Style.ContainerStyle != null &&
               element.Style.ContainerStyleSelector != null)
            {
                result.Add(
                    Error.StyleSelectorAmbiguity(element, element.Type, "ContainerStyle")
                    );
            }

            if(element.Style.HeaderTemplate != null &&
               element.Style.HeaderTemplateSelector != null)
            {
                result.Add(
                    Error.TemplateSelectorAmbiguity(element, element.Type, "HeaderTemplate")
                    );
            }
        }
    }
}