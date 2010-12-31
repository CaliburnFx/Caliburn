namespace Caliburn.Testability
{
    /// <summary>
    /// Validates a <see cref="DataTemplateElement"/>.
    /// </summary>
    public class DataTemplateValidator : DependencyObjectValidator
    {
        readonly DataTemplateElement element;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTemplateValidator"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="element">The element.</param>
        public DataTemplateValidator(ElementEnumeratorSettings settings, DataTemplateElement element)
            : base(settings, element)
        {
            this.element = element;
        }

        /// <summary>
        /// Validates the bindings.
        /// </summary>
        /// <returns>The result of the validation process.</returns>
        public override ValidationResult ValidateBindings()
        {
            var result = base.ValidateBindings();

            if(element.Type != null)
            {
                var triggerValidator = new TriggerValidator(element.Type, element, element.DataTemplate.Triggers);
                result.Add(triggerValidator.ValidateBindings());
            }

            return result;
        }
    }
}