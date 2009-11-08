namespace Caliburn.ModelFramework
{
    /// <summary>
    /// Implemented by types capable of visiting a models.
    /// </summary>
    public interface IModelVisitor
    {
        /// <summary>
        /// Visits an <see cref="IModel"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">The model.</param>
        void VisitModel<T>(T model) where T : IModel;

        /// <summary>
        /// Visits an <see cref="IProperty{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">The property.</param>
        void VisitProperty<T>(IProperty<T> property);

        /// <summary>
        /// Visits a <see cref="ICollectionNode{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionNode">The presentation collection.</param>
        void VisitCollection<T>(ICollectionNode<T> collectionNode);
    }
}