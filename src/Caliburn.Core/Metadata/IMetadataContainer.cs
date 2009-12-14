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
        /// Finds the matching metadata.
        /// </summary>
        /// <typeparam name="T">The type to match.</typeparam>
        /// <returns>The matches</returns>
        IEnumerable<T> FindMetadata<T>() where T : IMetadata;
    }
}