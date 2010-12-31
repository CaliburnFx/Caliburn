namespace Caliburn.Testability
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Validates a <see cref="StyleElement"/>.
    /// </summary>
    public class StyleValidator
    {
        readonly ElementEnumeratorSettings settings;
        readonly StyleElement element;

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleValidator"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="element">The element.</param>
        public StyleValidator(ElementEnumeratorSettings settings, StyleElement element)
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

            foreach(var info in GetBindings())
            {
                var validatedProperty = element.Type.ValidateAgainst(element, info.Property, info.Binding);
                result.Add(validatedProperty);
            }

            var triggerValidator = new TriggerValidator(element.Type, element, element.Style.Triggers);
            result.Add(triggerValidator.ValidateBindings());

            return result;
        }

        private IEnumerable<BindingInfo> GetBindings()
        {
            foreach(var setterBase in element.Style.Setters)
            {
                var setter = setterBase as Setter;
                if(setter == null) continue;

                var bindingBase = setter.Value as BindingBase;

                foreach(var actualBinding in bindingBase.GetActualBindings())
                {
                    yield return new BindingInfo(actualBinding, setter.Property);
                }
            }
        }
    }
}