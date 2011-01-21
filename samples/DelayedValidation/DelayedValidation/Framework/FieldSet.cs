namespace DelayedValidation.Framework {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;

    public class FieldSet {
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.RegisterAttached("Binding", typeof(string), typeof(FieldSet), new PropertyMetadata(OnBindingChanged));

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.RegisterAttached("Name", typeof(string), typeof(FieldSet), null);

        public static readonly DependencyProperty PropertyToValidateProperty =
            DependencyProperty.RegisterAttached("PropertyToValidate", typeof(string), typeof(FieldSet), null);

        static void OnBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if(e.NewValue == e.OldValue)
                return;

            var expression = e.NewValue as string;
            if(string.IsNullOrEmpty(expression))
                return;

            var parts = expression.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            SetName(d, parts[0]);
            SetPropertyToValidate(d, parts[1]);

            var binding = new Binding(parts[2]) { UpdateSourceTrigger = UpdateSourceTrigger.Explicit };
            binding.ValidationRules.Add(new DataErrorValidationRule { ValidatesOnTargetUpdated = false });

            BindingOperations.SetBinding(d, GetDependencyProperty(d.GetType(), parts[1]), binding);
        }

        public static DependencyProperty GetDependencyProperty(Type type, string name) {
            var dpField = type.GetField(name + "Property", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
            return (DependencyProperty)dpField.GetValue(null);
        }

        public static IEnumerable<UIElement> GetChildren(UIElement parent, string fieldSetName) {
            if(parent == null)
                yield break;
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            if(childrenCount == 0)
                yield break;
            for(var i = 0; i < childrenCount; i++) {
                var child = VisualTreeHelper.GetChild(parent, i) as UIElement;
                if(child == null)
                    continue;
                if(GetName(child) == fieldSetName)
                    yield return child;
                else {
                    foreach(var nestedChild in GetChildren(child, fieldSetName)) {
                        yield return nestedChild;
                    }
                }
            }
        }

        public static void SetBinding(DependencyObject d, string value) {
            d.SetValue(BindingProperty, value);
        }

        public static string GetBinding(DependencyObject d) {
            return d.GetValue(BindingProperty) as string;
        }

        public static string GetName(DependencyObject obj) {
            return (string)obj.GetValue(NameProperty);
        }

        public static void SetName(DependencyObject obj, string value) {
            obj.SetValue(NameProperty, value);
        }

        public static string GetPropertyToValidate(DependencyObject obj) {
            return (string)obj.GetValue(PropertyToValidateProperty);
        }

        public static void SetPropertyToValidate(DependencyObject obj, string value) {
            obj.SetValue(PropertyToValidateProperty, value);
        }
    }
}