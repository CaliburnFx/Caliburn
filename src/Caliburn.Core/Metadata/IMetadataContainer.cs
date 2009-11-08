namespace Caliburn.Core.Metadata
{
    using System.Collections.Generic;

    /// <summary>
    /// Stores metadata.
    /// </summary>
    public interface IMetadataContainer
    {
        /// <summary>
        /// Adds metadata to the store.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        void AddMetadata(IMetadata metadata);

        /// <summary>
        /// Retrieves metadata from the store.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetMetadata<T>() where T : IMetadata;

        /// <summary>
        /// Gets the matching metadata.
        /// </summary>
        /// <typeparam name="T">The type to match.</typeparam>
        /// <returns>The matches</returns>
        IEnumerable<T> GetMatchingMetadata<T>() where T : IMetadata;
    }
}