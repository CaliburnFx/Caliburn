namespace Caliburn.Core.Metadata
{
    using System.Linq;

    public static class ExtensionMethods
    {
        /// <summary>
        /// Retrieves metadata from the store.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The metadata.</returns>
        public static T GetMetadata<T>(this IMetadataContainer container)
            where T : IMetadata
        {
            return container.FindMetadata<T>().FirstOrDefault();
        }
    }
}