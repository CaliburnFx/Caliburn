namespace DelayedValidation.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using Caliburn.PresentationFramework.Filters;
    using Caliburn.PresentationFramework.RoutedMessaging;

    public class ValidateFieldSetAttribute : Attribute, IPreProcessor
    {
        readonly string _fieldSetName;

        public ValidateFieldSetAttribute(string fieldSetName)
        {
            _fieldSetName = fieldSetName;
        }

        public int Priority { get; set; }
        public bool AffectsTriggers { get; set; }

        public bool Execute(IRoutedMessage message, IInteractionNode handlingNode, object[] parameters)
        {
            var valid = true;
            var elementsToValidate = GetFieldSetChildren(handlingNode.UIElement as FrameworkElement);
            foreach(var element in elementsToValidate)
            {
                if(!element.IsVisible || !element.IsEnabled)
                    continue;
                var propertyToValidate = FieldSet.GetPropertyToValidate(element);
                if(string.IsNullOrEmpty(propertyToValidate))
                    continue;
                var fieldInfo = GetField(element.GetType(), string.Format("{0}Property", propertyToValidate));
                if(fieldInfo == null)
                    continue;
                var property = fieldInfo.GetValue(element) as DependencyProperty;
                if(property == null)
                    continue;
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

        IEnumerable<FrameworkElement> GetFieldSetChildren(FrameworkElement parent)
        {
            if(parent == null)
                yield break;
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            if(childrenCount == 0)
                yield break;
            for(var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                if(child == null)
                    continue;
                if(FieldSet.GetName(child) == _fieldSetName)
                    yield return child;
                else
                {
                    foreach(var nestedChild in GetFieldSetChildren(child))
                    {
                        yield return nestedChild;
                    }
                }
            }
        }

        static FieldInfo GetField(Type type, string fieldName)
        {
            var fieldInfo = type.GetField(fieldName);
            if(fieldInfo != null)
                return fieldInfo;
            return type.BaseType != null ? GetField(type.BaseType, fieldName) : null;
        }
    }
}