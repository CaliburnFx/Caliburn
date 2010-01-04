namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Hosts extension methods for application model classes.
    /// </summary>
    public static class ExtensionMethods
    {
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

#if !SILVERLIGHT

        /// <summary>
        /// Shows the window for the model.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="rootModel">The root model.</param>
        public static void Show(this IWindowManager manager, object rootModel)
        {
            manager.Show(rootModel, null, null);
        }

        /// <summary>
        /// Shows the dialog for the model.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="rootModel">The root model.</param>
        /// <returns>The dialog result</returns>
        public static bool? ShowDialog(this IWindowManager manager, object rootModel)
        {
            return manager.ShowDialog(rootModel, null, null);
        }

        /// <summary>
        /// Shows the windwo for the model.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        public static void Show(this IWindowManager manager, object rootModel, object context)
        {
            manager.Show(rootModel, context, null);
        }

        /// <summary>
        /// Shows the dialog for the model.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <returns>The dialog result</returns>
        public static bool? ShowDialog(this IWindowManager manager, object rootModel, object context)
        {
            return manager.ShowDialog(rootModel, context, null);
        }

#endif
    }
}