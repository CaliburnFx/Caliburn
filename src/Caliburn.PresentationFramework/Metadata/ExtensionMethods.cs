namespace Caliburn.PresentationFramework.Metadata
{
    using Core.Metadata;

    /// <summary>
    /// Extension methods realted to metadata.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the view from metadata.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container">The container.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static T GetView<T>(this IMetadataContainer container, object context)
        {
            var viewMetadata = container.GetMetadata<ViewMetadata>();

            return viewMetadata == null
                       ? default(T)
                       : viewMetadata.GetView<T>(context);
        }

        /// <summary>
        /// Stores the view in metadata.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="view">The view.</param>
        /// <param name="context">The context.</param>
        /// <param name="useWeakReference">Indicates whether the view should be referenced weakly.</param>
        public static void SetView(this IMetadataContainer container, object view, object context, bool useWeakReference)
        {
            var viewMetadata = container.GetMetadata<ViewMetadata>();

            if (viewMetadata == null)
            {
                viewMetadata = new ViewMetadata();
                container.AddMetadata(viewMetadata);
            }

            viewMetadata.SetView(view, context, useWeakReference);
        }
    }
}