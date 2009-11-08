namespace BackgroundProcessing
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using Caliburn.PresentationFramework;

    public class ShowPopupResult : IResult
    {
        public void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode)
        {
            var source = (FrameworkElement)message.Source.UIElement;
            var popup = source.FindName("detailsPopup") as Popup;

            popup.IsOpen = true;
            popup.CaptureMouse();
            popup.Child.MouseLeave += (o, e) => popup.IsOpen = false;

            Completed(this, null);
        }

        public event Action<IResult, Exception> Completed = delegate { };
    }
}