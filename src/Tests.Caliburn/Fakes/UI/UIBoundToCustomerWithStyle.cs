namespace Tests.Caliburn.Fakes.UI
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class UIBoundToCustomerWithStyle : UserControl
    {
        public UIBoundToCustomerWithStyle()
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

        Style CreateStyle()
        {
            var style = new Style();

            style.Setters.Add(
                new Setter(
                    TextBlock.FontFamilyProperty,
                    new Binding("FontFamily") //property does not exist
                    )
                );

            return style;
        }
    }
}