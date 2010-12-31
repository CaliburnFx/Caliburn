namespace Tests.Caliburn.Fakes.UI
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class UIWithHierarchicalPath : UserControl
    {
        public UIWithHierarchicalPath()
        {
            var grid = new Grid();
            Content = grid;

            var label = new Label();
            label.SetBinding(ContentProperty, new Binding("Items/Description"));
            grid.Children.Add(label);

            var stack = new StackPanel();
            stack.SetBinding(DataContextProperty, new Binding("Items"));
            grid.Children.Add(stack);

            label = new Label();
            label.SetBinding(ContentProperty, new Binding("/Description"));
            stack.Children.Add(label);

            var button = new Button();
            button.SetBinding(ContentProperty, new Binding("/"));
            button.Template = CreateTemplate();
            stack.Children.Add(button);
        }

        ControlTemplate CreateTemplate()
        {
            var template = new ControlTemplate();

            var label = new FrameworkElementFactory(typeof(Label));
            label.SetBinding(ContentProperty, new Binding("Description"));
            template.VisualTree = label;

            return template;
        }
    }
}