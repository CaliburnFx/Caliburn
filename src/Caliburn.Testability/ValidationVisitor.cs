namespace Caliburn.Testability
{
    /// <summary>
    /// An implementation of <see cref="IElementVisitor"/> that validates the elements that are visited.
    /// </summary>
    public class ValidationVisitor : IElementVisitor
    {
        private ElementEnumeratorSettings _settings;
        private ValidationResult _result;
        private bool _shouldStopVisiting;

        /// <summary>
        /// Prepares the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void Prepare(ElementEnumeratorSettings settings)
        {
            _settings = settings;
            _result = new ValidationResult();
        }

        /// <summary>
        /// Gets a value indicating whether the visitor wants the enumerator to stop visiting elements.
        /// </summary>
        /// <value><c>true</c> if visiting should stop; otherwise, <c>false</c>.</value>
        public bool ShouldStopVisiting
        {
            get { return _shouldStopVisiting; }
        }

        /// <summary>
        /// Gets the result of the validation.
        /// </summary>
        /// <value>The result.</value>
        public ValidationResult Result
        {
            get { return _result; }
        }

        /// <summary>
        /// Visits the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Visit(DependencyObjectElement element)
        {
            HandleResult(
                new DependencyObjectValidator(_settings, element)
                    .ValidateBindings()
                );
        }

        /// <summary>
        /// Visits the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Visit(StyleElement element)
        {
            HandleResult(
                new StyleValidator(_settings, element)
                    .ValidateBindings()
                );
        }

        /// <summary>
        /// Visits the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Visit(GroupStyleElement element)
        {
            HandleResult(
                new GroupStyleValidator(_settings, element)
                    .ValidateBindings()
                );
        }

        /// <summary>
        /// Visits the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Visit(DataTemplateElement element)
        {
            HandleResult(
                new DataTemplateValidator(_settings, element)
                    .ValidateBindings()
                );
        }

        /// <summary>
        /// Visits the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Visit(ControlTemplateElement element)
        {
            HandleResult(
                new ControlTemplateValidator(_settings, element)
                    .ValidateBindings()
                );
        }

        private void HandleResult(ValidationResult validationResult)
        {
            _result.Add(validationResult);

            if(_settings.StopAfterFirstError && _result.HasErrors)
                _shouldStopVisiting = true;
        }
    }
}