namespace Caliburn.Core.Metadata
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// An implementation of <see cref="IMetadataContainer"/>.
    /// </summary>
    public class MetadataContainer : PropertyChangedBase, IMetadataContainer
    {
        private List<IMetadata> _metadata;

        /// <summary>
        /// Adds the metadata from the provided member to the collection.
        /// </summary>
        /// <param name="member">The member.</param>
        protected virtual void AddMetadataFrom(MemberInfo member)
        {
            member.GetCustomAttributes(true)
                .OfType<IMetadata>()
                .Apply(AddMetadata);
        }

        /// <summary>
        /// Adds metadata to the store.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public virtual void AddMetadata(IMetadata metadata)
        {
            if(_metadata == null)
                _metadata = new List<IMetadata>();

            _metadata.Add(metadata);
        }

        /// <summary>
        /// Finds the matching metadata.
        /// </summary>
        /// <typeparam name="T">The type to match.</typeparam>
        /// <returns>The matches</returns>
        public virtual IEnumerable<T> FindMetadata<T>()
            where T : IMetadata
        {
            return _metadata == null
                       ? new List<T>()
                       : _metadata.OfType<T>();
        }
    }
}