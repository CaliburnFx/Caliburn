namespace Tests.Caliburn.Fakes.UI
{
    using System.Windows.Controls;
    using System.Windows.Data;

    public class SimpleUIWithPriorityBinding : UserControl
    {
        public SimpleUIWithPriorityBinding()
        {
            var stack = new StackPanel();
            Content = stack;

            var priorityBinding = new PriorityBinding();
            priorityBinding.Bindings.Add(new Binding("FirstName"));
            priorityBinding.Bindings.Add(new Binding("LstName")); //purposefully misspelled

            var textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, priorityBinding);
            stack.Children.Add(textBlock);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("Age"));
            stack.Children.Add(textBlock);
        }
    }
}