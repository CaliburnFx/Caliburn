using System.Windows.Controls;
using System.Windows.Data;
using Tests.Caliburn.Fakes.UI;

namespace Tests.Caliburn.Fakes.UI
{
    public class SimpleUIUsingResourceDictionary : UserControl
    {
        public SimpleUIUsingResourceDictionary()
        {
            var stack = new StackPanel();
            Content = stack;

            var textBlock = new TextBlock();
            textBlock.SetResourceReference(TextBlock.StyleProperty, SimpleResourceDictionary.TextBlockStyleKey);
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("FirstName"));
            stack.Children.Add(textBlock);

            var local = textBlock.GetValue(TextBlock.StyleProperty);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("LstName")); //purposefully misspelled
            stack.Children.Add(textBlock);
        }
    }
}