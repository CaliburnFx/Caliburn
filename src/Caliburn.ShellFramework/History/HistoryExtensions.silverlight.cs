#if SILVERLIGHT

namespace Caliburn.ShellFramework.History
{
    using System;
    using System.Linq;
    using Core;

    /// <summary>
    /// Extension methods for history coordination.
    /// </summary>
    public static class HistoryExtensions
    {
        /// <summary>
        /// Gets the history key value from an instance.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <returns>The key, or null if not found.</returns>
        public static string GetHistoryValue(this object screen)
        {
            var keyAttribute = screen.GetHistoryKey();
            return keyAttribute != null ? keyAttribute.Value : null;
        }

        /// <summary>
        /// Gets the history key for an instance.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <returns>The key, or null if not found.</returns>
        public static IHistoryKey GetHistoryKey(this object screen)
        {
            return screen == null ? null : screen.GetType().GetHistoryKey();
        }

        /// <summary>
        /// Gets the history key for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The key, or null if not found.</returns>
        public static IHistoryKey GetHistoryKey(this Type type)
        {
            return type.GetAttributes<IHistoryKey>(false)
                .FirstOrDefault();
        }
    }
}

#endif