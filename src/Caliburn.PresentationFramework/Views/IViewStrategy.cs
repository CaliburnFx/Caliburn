namespace Caliburn.PresentationFramework.Views
{
    using Core.Metadata;

    /// <summary>
    /// An <see cref="IMetadata"/> as <see cref="IViewLocator"/> with conditional matching.
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