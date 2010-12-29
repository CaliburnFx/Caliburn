namespace Caliburn.ShellFramework.Results
{
    using System;
    using PresentationFramework.RoutedMessaging;

    /// <summary>
    /// An <see cref="IResult"/> that opens things.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
    public interface IOpenResult<TTarget> : IResult
    {
        /// <summary>
        /// Gets or sets the configuration delegate.
        /// </summary>
        /// <value>The on configure.</value>
        Action<TTarget> OnConfigure { get; set; }

        /// <summary>
        /// Gets or sets the close delegate.
        /// </summary>
        /// <value>The on close.</value>
        Action<TTarget> OnClose { get; set; }
    }
}