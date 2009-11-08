namespace Caliburn.ModelFramework
{
    using System.Collections.Generic;

    /// <summary>
    /// Hosts extension methods related to presentation models.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Validates the specified modelNode.
        /// </summary>
        /// <param name="modelNode">The model.</param>
        /// <returns></returns>
        public static IList<IValidationResult> Validate(this IModelNode modelNode)
        {
            var visitor = new ValidationVisitor();
            modelNode.Accept(visitor);
            return visitor.Result;
        }

        /// <summary>
        /// Adds a property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="definition">The definition.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static IPropertyDefinition<T> AddProperty<T>(this IModelDefinition definition, string propertyName)
        {
            return definition.AddProperty(propertyName, () => default(T));
        }

        /// <summary>
        /// Adds an association property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="definition">The definition.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static IPropertyDefinition<T> AddAssociation<T>(this IModelDefinition definition, string propertyName)
            where T : new()
        {
            return definition.AddProperty(propertyName, () => new T());
        }

        /// <summary>
        /// Adds a collection property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="definition">The definition.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static IPropertyDefinition<ICollectionNode<T>> AddCollection<T>(this IModelDefinition definition,
                                                                               string propertyName)
        {
            return definition.AddProperty<ICollectionNode<T>>(propertyName, () => new CollectionNode<T>());
        }
    }
}