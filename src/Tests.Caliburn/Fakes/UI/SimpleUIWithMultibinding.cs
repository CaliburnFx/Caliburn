using System.Windows.Controls;
using System.Windows.Data;

namespace Tests.Caliburn.Fakes.UI
{
    public class SimpleUIWithMultibinding : UserControl
    {
        public SimpleUIWithMultibinding()
        {
            var stack = new StackPanel();
            Content = stack;

            var multibinding = new MultiBinding { Converter = new SimpleMultiValueConverter()};
            multibinding.Bindings.Add(new Binding("FirstName"));
            multibinding.Bindings.Add(new Binding("LstName")); //purposefully misspelled

            var textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, multibinding);
            stack.Children.Add(textBlock);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("Age"));
            stack.Children.Add(textBlock);
        }
    }
}
