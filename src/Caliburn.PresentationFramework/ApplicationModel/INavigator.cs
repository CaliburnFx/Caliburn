namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;

    /// <summary>
    /// A <see cref="IPresenterManager"/> that also has basic navigation abilities.
    /// </summary>
    public interface INavigator : IPresenterManager
    {
        /// <summary>
        /// Gets a value indicating whether this instance can navigate back.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can go back; otherwise, <c>false</c>.
        /// </value>
        bool CanGoBack { get; }

        /// <summary>
        /// Gets a value indicating whether this instance can navigate forward.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can go forward; otherwise, <c>false</c>.
        /// </value>
        bool CanGoForward { get; }

        /// <summary>
        /// Navigates forward.
        /// </summary>
        void Forward(Action<bool> completed);

        /// <summary>
        /// Navigates back.
        /// </summary>
        void Back(Action<bool> completed);

        /// <summary>
        /// Navigates using the specified action.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="completed">Called when navigation is completed.</param>
        void Navigate(Action<Action<bool>> function, Action<bool> completed);

        /// <summary>
        /// Clears the navigation history.
        /// </summary>
        void ClearHistory();
    }
}