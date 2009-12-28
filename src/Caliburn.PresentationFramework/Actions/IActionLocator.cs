namespace Caliburn.PresentationFramework.Actions
{
    using System.Collections.Generic;

    /// <summary>
    /// A service responsible for locating actions.
    /// </summary>
    public interface IActionLocator
    {
        /// <summary>
        /// Locates actions using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Discovered actions.</returns>
        IEnumerable<IAction> Locate(ActionLocationContext context);
    }
}