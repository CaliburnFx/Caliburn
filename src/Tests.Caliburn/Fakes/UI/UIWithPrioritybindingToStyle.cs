namespace Tests.Caliburn.Fakes.UI
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class UIWithPrioritybindingToStyle : UserControl
    {
        public UIWithPrioritybindingToStyle()
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

            var priorityBinding = new PriorityBinding();
            priorityBinding.Bindings.Add(new Binding("FirstName"));
            priorityBinding.Bindings.Add(new Binding("LstName")); //purposefully misspelled

            style.Setters.Add(
                new Setter(
                    TextBlock.FontFamilyProperty,
                    priorityBinding
                    )
                );

            return style;
        }
    }
}