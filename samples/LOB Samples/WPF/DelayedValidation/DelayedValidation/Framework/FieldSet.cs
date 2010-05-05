namespace DelayedValidation.Framework
{
    using System.Windows;

    public class FieldSet
    {
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.RegisterAttached("Name", typeof(string), typeof(FieldSet), null);

        public static readonly DependencyProperty PropertyToValidateProperty =
            DependencyProperty.RegisterAttached("PropertyToValidate", typeof(string), typeof(FieldSet), null);

        public static string GetName(DependencyObject obj)
        {
            return (string)obj.GetValue(NameProperty);
        }

        public static void SetName(DependencyObject obj, string value)
        {
            obj.SetValue(NameProperty, value);
        }

        public static string GetPropertyToValidate(DependencyObject obj)
        {
            return (string)obj.GetValue(PropertyToValidateProperty);
        }

        public static void SetPropertyToValidate(DependencyObject obj, string value)
        {
            obj.SetValue(PropertyToValidateProperty, value);
        }
    }
}