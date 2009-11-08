namespace Caliburn.PresentationFramework.Filters
{
    using Core.Metadata;

    /// <summary>
    /// A filter.
    /// </summary>
    public interface IFilter : IMetadata
    {
        /// <summary>
        /// Gets the priority used to order filters.
        /// </summary>
        /// <remarks>Higher numbers are evaluated first.</remarks>
        /// <value>The order.</value>
        int Priority { get; }
    }
}