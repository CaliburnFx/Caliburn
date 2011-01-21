namespace DelayedValidation.Framework {
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Caliburn.PresentationFramework.Filters;
    using Caliburn.PresentationFramework.RoutedMessaging;

    public class ValidateFieldSetAttribute : Attribute, IPreProcessor {
        readonly string fieldSetName;

        public ValidateFieldSetAttribute(string fieldSetName) {
            this.fieldSetName = fieldSetName;
        }

        public int Priority { get; set; }
        public bool AffectsTriggers { get; set; }

        public bool Execute(IRoutedMessage message, IInteractionNode handlingNode, object[] parameters) {
            var valid = true;
            var elementsToValidate = FieldSet.GetChildren(handlingNode.UIElement as FrameworkElement, fieldSetName);

            foreach(var element in elementsToValidate) {
                if(!element.IsVisible || !element.IsEnabled)
                    continue;

                var propertyToValidate = FieldSet.GetPropertyToValidate(element);
                if(string.IsNullOrEmpty(propertyToValidate))
                    continue;

                var property = FieldSet.GetDependencyProperty(element.GetType(), propertyToValidate);
                var expression = BindingOperations.GetBindingExpression(element, property);
                if(expression == null)
                    continue;

                expression.UpdateSource();

                var errors = Validation.GetErrors(element);
                if(errors != null && errors.Any())
                    valid = false;
            }

            return valid;
        }
    }
}