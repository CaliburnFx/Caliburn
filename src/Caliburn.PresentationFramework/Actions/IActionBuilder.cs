namespace Caliburn.PresentationFramework.Actions
{
    /// <summary>
    /// A service responsible for building an <see cref="IAction"/>.
    /// </summary>
    public interface IActionBuilder
    {
        /// <summary>
        /// Builds an <see cref="IAction"/> using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="IAction"/>.</returns>
        IAction Build(ActionBuildingContext context);
    }
}