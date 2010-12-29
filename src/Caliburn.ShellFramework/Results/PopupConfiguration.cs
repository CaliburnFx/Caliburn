namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using PresentationFramework;

    /// <summary>
    /// The configuration for popup location.
    /// </summary>
    public static class PopupConfiguration
    {
        /// <summary>
        /// Finds or creates a popup for the provided element.
        /// </summary>
        public static Func<UIElement, Popup> FindOrCreatePopupFor = DefaultFindOrCreatePopupFor;
        
        /// <summary>
        /// Determines the name of the popup based on it's owner's name.
        /// </summary>
        public static Func<string, string> DeterminePopupName = DefaultDeterminePopupName;

        private static Popup DefaultFindOrCreatePopupFor(UIElement target)
        {
            var targetName = target.GetName();

            if (string.IsNullOrEmpty(targetName))
            {
                var popup = new Popup();

#if !SILVERLIGHT
                popup.PlacementTarget = target;
#endif

                return popup;
            }

            var popupName = DeterminePopupName(targetName);
            return target.FindName(popupName) as Popup;
        }

        private static string DefaultDeterminePopupName(string name)
        {
            return name + "Popup";
        }
    }
}