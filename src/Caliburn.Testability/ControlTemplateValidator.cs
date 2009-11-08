namespace Caliburn.Testability
{
    /// <summary>
    /// Validates a <see cref="ControlTemplateElement"/>.
    /// </summary>
    public class ControlTemplateValidator : DependencyObjectValidator
    {
        private readonly ControlTemplateElement _element;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlTemplateValidator"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="element">The element.</param>
        public ControlTemplateValidator(ElementEnumeratorSettings settings, ControlTemplateElement element)
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
                var triggerValidator = new TriggerValidator(_element.Type, _element, _element.ControlTemplate.Triggers);
                result.Add(triggerValidator.ValidateBindings());
            }

            return result;
        }
    }
}