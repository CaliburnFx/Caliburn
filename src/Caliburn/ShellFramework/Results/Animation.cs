namespace Caliburn.ShellFramework.Results
{
    using PresentationFramework.RoutedMessaging;

    /// <summary>
    /// Factory for animation-related <see cref="IResult"/> instances.
    /// </summary>
    public static class Animation
    {
        /// <summary>
        /// Creates a Begin animation result.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The result.</returns>
        public static AnimationResult Begin(string key)
        {
            return new AnimationResult(key, AnimationResult.AnimationAction.Begin);
        }

        /// <summary>
        /// Creates a Pause animation result.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The result.</returns>
        public static AnimationResult Pause(string key)
        {
            return new AnimationResult(key, AnimationResult.AnimationAction.Pause);
        }

        /// <summary>
        /// Creates a result animation result.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The result.</returns>
        public static AnimationResult Resume(string key)
        {
            return new AnimationResult(key, AnimationResult.AnimationAction.Resume);
        }

        /// <summary>
        /// Creates a stop animation result.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The result.</returns>
        public static AnimationResult Stop(string key)
        {
            return new AnimationResult(key, AnimationResult.AnimationAction.Stop);
        }
    }
}