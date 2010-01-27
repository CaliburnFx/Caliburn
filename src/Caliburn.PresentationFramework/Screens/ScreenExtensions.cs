using System;

namespace Caliburn.PresentationFramework.Screens
{
    using System.Linq;
    using ViewModels;

    /// <summary>
    /// Hosts extension methods for <see cref="IScreen"/> classes.
    /// </summary>
    public static class ScreenExtensions
    {
        private static IViewModelFactory _viewModelFactory;

        /// <summary>
        /// Initializes the extensions with the specified view model factory.
        /// </summary>
        /// <param name="viewModelFactory">The view model factory.</param>
        public static void Initialize(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        /// <summary>
        /// Opens the specified screen.
        /// </summary>
        /// <param name="collection">The screen collection.</param>
        /// <param name="subject">The subject.</param>
        public static void OpenScreen(this IScreenCollection collection, IScreenSubject subject)
        {
            collection.OpenScreen(subject, delegate { });
        }

        /// <summary>
        /// Opens the specified screen.
        /// </summary>
        /// <param name="collection">The screen collection.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="completed">Completion callback.</param>
        public static void OpenScreen(this IScreenCollection collection, IScreenSubject subject, Action<bool> completed)
        {
            var found = collection.Screens.FirstOrDefault(subject.Matches);

            if(found != null)
                collection.OpenScreen(found, completed);
            else subject.CreateScreen(_viewModelFactory, screen => collection.OpenScreen(screen, completed));
        }

        /// <summary>
        /// Opens the specified screen.
        /// </summary>
        /// <param name="collection">The screen collection.</param>
        /// <param name="screen">The screen.</param>
        public static void OpenScreen<T>(this IScreenCollection<T> collection, T screen)
            where T : class, IScreen
        {
            collection.OpenScreen(screen, isSuccess => { });
        }

        /// <summary>
        /// Shuts down the specified screen.
        /// </summary>
        /// <param name="collection">The screen collection owning the screen to shutdown.</param>
        /// <param name="screen">The screen.</param>
        public static void ShutdownScreen<T>(this IScreenCollection<T> collection, T screen)
            where T : class, IScreen
        {
            collection.ShutdownScreen(screen, isSuccess => { });
        }

        /// <summary>
        /// Shuts down the specified screen.
        /// </summary>
        /// <param name="collection">The screen collection owning the screen to shutdown.</param>
        /// <param name="screen">The screen.</param>
        public static void ShutdownScreen(this IScreenCollection collection, IScreen screen)
        {
            collection.ShutdownScreen(screen, isSuccess => { });
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