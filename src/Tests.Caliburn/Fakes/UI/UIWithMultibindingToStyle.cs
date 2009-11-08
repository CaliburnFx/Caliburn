using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Tests.Caliburn.Fakes.UI
{
    public class UIWithMultibindingToStyle : UserControl
    {
        public UIWithMultibindingToStyle()
        {
            var stack = new StackPanel();
            Content = stack;

            var textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("FirstName"));
            stack.Children.Add(textBlock);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("LastName"));
            textBlock.Style = CreateStyle();

            stack.Children.Add(textBlock);
        }

        private Style CreateStyle()
        {
            var style = new Style();

            var multibinding = new MultiBinding { Converter = new SimpleMultiValueConverter()};
            multibinding.Bindings.Add(new Binding("FirstName"));
            multibinding.Bindings.Add(new Binding("LstName")); //purposefully misspelled

            style.Setters.Add(
                new Setter(
                    TextBlock.FontFamilyProperty,
                    multibinding
                    )
                );

            return style;
        }
    }
}