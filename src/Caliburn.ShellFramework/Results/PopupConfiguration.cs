namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using PresentationFramework;

    public static class PopupConfiguration
    {
        public static Func<UIElement, Popup> FindOrCreatePopupFor = DefaultFindOrCreatePopupFor;
        public static Func<string, string> DeterminePopupName = DefaultDeterminePopupName;

        private static Popup DefaultFindOrCreatePopupFor(UIElement target)
        {
            var tartetName = target.GetName();

            if (string.IsNullOrEmpty(tartetName))
            {
                var popup = new Popup();

#if !SILVERLIGHT
                popup.PlacementTarget = target;
#endif

                return popup;
            }

            var popupName = DeterminePopupName(tartetName);
            return target.FindName(popupName) as Popup;
        }

        private static string DefaultDeterminePopupName(string name)
        {
            return name + "Popup";
        }
    }
}