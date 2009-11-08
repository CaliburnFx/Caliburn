namespace Caliburn.PresentationFramework.Actions
{
    using System.Collections.Generic;

    /// <summary>
    /// A service capable of creating actions.
    /// </summary>
    public interface IActionFactory
    {
        /// <summary>
        /// Creates actions for the provided host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        IEnumerable<IAction> CreateFor(IActionHost host);
    }
}