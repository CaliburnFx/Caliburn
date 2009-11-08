namespace Caliburn.Testability
{
    /// <summary>
    /// Validates a <see cref="DataTemplateElement"/>.
    /// </summary>
    public class DataTemplateValidator : DependencyObjectValidator
    {
        private readonly DataTemplateElement _element;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTemplateValidator"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="element">The element.</param>
        public DataTemplateValidator(ElementEnumeratorSettings settings, DataTemplateElement element)
            : base(settings, element)
        {
            _element = element;
        }

        /// <summary>
        /// Validates the bindings.
        /// </summary>
        /// <returns>The result of the validation process.</returns>
        public override ValidationResult ValidateBindings()
        {
            var result = base.ValidateBindings();

            if(_element.Type != null)
            {
                var triggerValidator = new TriggerValidator(_element.Type, _element, _element.DataTemplate.Triggers);
                result.Add(triggerValidator.ValidateBindings());
            }

            return result;
        }
    }
}