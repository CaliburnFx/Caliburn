namespace AvailabilityEffects
{
    using System.Windows;
    using Caliburn.PresentationFramework.RoutedMessaging;

    //Note: Implement IAvailabilityEffect to create custom effects.
    public class OpacityEffect : IAvailabilityEffect
    {
        //Note: Update the UI based on the availability.
        public void ApplyTo(DependencyObject target, bool isAvailable)
        {
            var uiElement = target as UIElement;

            if(uiElement == null) return;

            uiElement.Opacity = isAvailable ? 1 : .5;
        }
    }
}