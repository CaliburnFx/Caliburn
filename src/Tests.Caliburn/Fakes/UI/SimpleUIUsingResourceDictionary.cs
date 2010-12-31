namespace Tests.Caliburn.Fakes.UI
{
    using System.Windows.Controls;
    using System.Windows.Data;

    public class SimpleUIUsingResourceDictionary : UserControl
    {
        public SimpleUIUsingResourceDictionary()
        {
            var stack = new StackPanel();
            Content = stack;

            var textBlock = new TextBlock();
            textBlock.SetResourceReference(StyleProperty, SimpleResourceDictionary.TextBlockStyleKey);
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("FirstName"));
            stack.Children.Add(textBlock);

            var local = textBlock.GetValue(StyleProperty);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("LstName")); //purposefully misspelled
            stack.Children.Add(textBlock);
        }
    }
}