namespace Tests.Caliburn.Fakes.UI
{
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public class UIWithBoundBrush : UserControl
    {
        public UIWithBoundBrush()
        {
            var rectangle = new Rectangle();
            var brush = new LinearGradientBrush();

            var stop = new GradientStop();
            var binding = new Binding("Stop12"); //not correct

            BindingOperations.SetBinding(stop, GradientStop.OffsetProperty, binding);

            brush.GradientStops.Add(stop);

            rectangle.Fill = brush;

            Content = rectangle;
        }
    }
}