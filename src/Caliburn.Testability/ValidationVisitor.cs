namespace Caliburn.Testability
{
    /// <summary>
    /// An implementation of <see cref="IElementVisitor"/> that validates the elements that are visited.
    /// </summary>
    public class ValidationVisitor : IElementVisitor
    {
        ElementEnumeratorSettings settings;
        ValidationResult result;
        bool shouldStopVisiting;

        /// <summary>
        /// Prepares the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void Prepare(ElementEnumeratorSettings settings)
        {
            this.settings = settings;
            result = new ValidationResult();
        }

        /// <summary>
        /// Gets a value indicating whether the visitor wants the enumerator to stop visiting elements.
        /// </summary>
        /// <value><c>true</c> if visiting should stop; otherwise, <c>false</c>.</value>
        public bool ShouldStopVisiting
        {
            get { return shouldStopVisiting; }
        }

        /// <summary>
        /// Gets the result of the validation.
        /// </summary>
        /// <value>The result.</value>
        public ValidationResult Result
        {
            get { return result; }
        }

        /// <summary>
        /// Visits the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Visit(DependencyObjectElement element)
        {
            HandleResult(
                new DependencyObjectValidator(settings, element)
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
                new StyleValidator(settings, element)
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
                new GroupStyleValidator(settings, element)
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
                new DataTemplateValidator(settings, element)
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
                new ControlTemplateValidator(settings, element)
                    .ValidateBindings()
                );
        }

        void HandleResult(ValidationResult validationResult)
        {
            result.Add(validationResult);

            if(settings.StopAfterFirstError && result.HasErrors)
                shouldStopVisiting = true;
        }
    }
}