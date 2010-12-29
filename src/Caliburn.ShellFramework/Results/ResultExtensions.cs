namespace Caliburn.ShellFramework.Results
{
    using System;
    using PresentationFramework.RoutedMessaging;

    /// <summary>
    /// Extensions methods for instances of <see cref="IResult"/>.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Configures the specified result.
        /// </summary>
        /// <typeparam name="TChild">The type of the child.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="configure">The configuration delegate.</param>
        /// <returns>Itself.</returns>
        public static IOpenResult<TChild> Configured<TChild>(this IOpenResult<TChild> result, Action<TChild> configure)
        {
            result.OnConfigure = configure;
            return result;
        }

        /// <summary>
        /// Adds custom close logic.
        /// </summary>
        /// <typeparam name="TChild">The type of the child.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="onShutdown">The close delegate.</param>
        /// <returns>Itself.</returns>
        public static IOpenResult<TChild> WhenClosing<TChild>(this IOpenResult<TChild> result, Action<TChild> onShutdown)
        {
            result.OnClose = onShutdown;
            return result;
        }
    }
}