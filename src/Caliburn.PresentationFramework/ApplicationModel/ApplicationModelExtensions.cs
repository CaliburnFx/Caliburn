namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.Globalization;
    using System.Linq.Expressions;
    using Core;

    /// <summary>
    /// Hosts extension methods for application model classes.
    /// </summary>
    public static class ApplicationModelExtensions
    {
        /// <summary>
        ///   Publishes a message.
        /// </summary>
        /// <param name="events">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        public static void Publish(this IEventAggregator events, object message) {
            events.Publish(message, null);
        }

        /// <summary>
        /// Focuses the control bound to the property on the view model.
        /// </summary>
        /// <param name="inputManager">The input manager.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="property">The property.</param>
        public static void Focus<T, K>(this IInputManager inputManager, T viewModel, Expression<Func<T, K>> property)
        {
            inputManager.Focus(viewModel, property.GetMemberInfo().Name);
        }

        /// <summary>
        /// Inserts or updates a value in the state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void InsertOrUpdate<T>(this IStateManager state, string key, T value)
        {
            state.InsertOrUpdate(key, Convert.ToString(value));
        }

        /// <summary>
        /// Gets the value with the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="state">The state.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T Get<T>(this IStateManager state, string key, T defaultValue)
        {
            var value = state.Get(key);

            if (value == null) 
                return defaultValue;

            try
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.CurrentUICulture);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Shows a popup at the current mouse position.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="rootModel">The root model.</param>
        public static void ShowPopup(this IWindowManager manager, object rootModel)
        {
            manager.ShowPopup(rootModel, null, null);
        }

        /// <summary>
        /// Shows a popup at the current mouse position.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The view context.</param>
        public static void ShowPopup(this IWindowManager manager, object rootModel, object context)
        {
            manager.ShowPopup(rootModel, context, null);
        }

#if !SILVERLIGHT

        /// <summary>
        /// Shows the window for the model.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="rootModel">The root model.</param>
        public static void ShowWindow(this IWindowManager manager, object rootModel)
        {
            manager.ShowWindow(rootModel, null);
        }

        /// <summary>
        /// Shows the dialog for the model.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="rootModel">The root model.</param>
        /// <returns>The dialog result</returns>
        public static bool? ShowDialog(this IWindowManager manager, object rootModel)
        {
            return manager.ShowDialog(rootModel, null);
        }

#else
        /// <summary>
        /// Shows the dialog for the model.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="rootModel">The root model.</param>
        public static void ShowDialog(this IWindowManager manager, object rootModel)
        {
            manager.ShowDialog(rootModel, null);
        }
#endif

#if SILVERLIGHT_40
        /// <summary>
        /// Shows a toast notification for the specified model.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="rootModel">The root model.</param>
        /// <param name="durationInMilliseconds">How long the notification should appear for.</param>
        public static void ShowNotification(this IWindowManager manager, object rootModel, int durationInMilliseconds)
        {
            manager.ShowNotification(rootModel, durationInMilliseconds, null);
        }
#endif
    }
}