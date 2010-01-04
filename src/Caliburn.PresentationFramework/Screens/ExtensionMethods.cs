using System;

namespace Caliburn.PresentationFramework.Screens
{
    /// <summary>
    /// Hosts extension methods for <see cref="IScreen"/> classes.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Opens the specified screen.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="screen">The screen.</param>
        public static void OpenScreen<T>(this IScreenHost<T> host, T screen)
            where T : class, IScreen
        {
            host.OpenScreen(screen, isSuccess => { });
        }

        /// <summary>
        /// Shuts down the specified screen.
        /// </summary>
        /// <param name="host">The manager.</param>
        /// <param name="screen">The presenter.</param>
        public static void ShutdownScreen<T>(this IScreenHost<T> host, T screen)
            where T : class, IScreen
        {
            host.ShutdownScreen(screen, isSuccess => { });
        }

        /// <summary>
        /// Shuts down the active screen.
        /// </summary>
        public static void ShutdownActiveScreen(this IScreenConductor screenConductor)
        {
            screenConductor.ShutdownActiveScreen(isSuccess => { });
        }

        /// <summary>
        /// Navigates back.
        /// </summary>
        public static void Back(this INavigator navigator)
        {
            navigator.Back(isSuccess => { });
        }

        /// <summary>
        /// Navigates forward.
        /// </summary>
        public static void Forward(this INavigator navigator)
        {
            navigator.Forward(isSuccess => { });
        }

        /// <summary>
        /// Navigates using the specified action.
        /// </summary>
        /// <param name="navigator">The navigator.</param>
        /// <param name="function">The function.</param>
        public static void Navigate(this INavigator navigator, Action<Action<bool>> function)
        {
            navigator.Navigate(function, isSuccess => { });
        }
    }
}