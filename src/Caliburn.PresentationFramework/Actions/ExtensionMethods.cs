namespace Caliburn.PresentationFramework.Actions
{
    /// <summary>
    /// Hosts extension methods related to actions.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Determines whether the action has trigger effects.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>
        /// 	<c>true</c> if has trigger effects; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasTriggerEffects(this IAction action)
        {
            return action.Filters.TriggerEffects.Length > 0;
        }
    }
}