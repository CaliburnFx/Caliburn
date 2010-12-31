namespace Tests.Caliburn.Fakes.UI
{
    using System;
    using System.Windows;

    public class FakeElement : UIElement
    {
        public static readonly string EventName = "Click";

        public static RoutedEvent RoutedEvent = EventManager.RegisterRoutedEvent(
            "Routed",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(FakeElement)
            );

        public event EventHandler Click;

        public void RaiseClick()
        {
            if(Click != null)
                Click(this, EventArgs.Empty);
        }
    }
}