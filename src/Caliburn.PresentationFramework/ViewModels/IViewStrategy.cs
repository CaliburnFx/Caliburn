namespace Caliburn.PresentationFramework.ViewModels
{
    using Core.Metadata;

    /// <summary>
    /// An <see cref="IViewLocator"/> as <see cref="IMetadata"/> with conditional matching.
    /// </summary>
    public interface IViewStrategy : IViewLocator, IMetadata
    {
        /// <summary>
        /// Determines whether this strategy applies in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>true if it matches the context; false otherwise</returns>
        bool Matches(object context);
    }
}