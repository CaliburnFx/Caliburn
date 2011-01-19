namespace BackgroundProcessing.Framework {
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using Caliburn.PresentationFramework.RoutedMessaging;

    public class ShowPopupResult : IResult {
        public void Execute(ResultExecutionContext context) {
            var source = (FrameworkElement)context.Message.Source.UIElement;
            var popup = source.FindName("detailsPopup") as Popup;

            popup.IsOpen = true;
            popup.CaptureMouse();
            popup.Child.MouseLeave += (o, e) => popup.IsOpen = false;

            Completed(this, new ResultCompletionEventArgs());
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}