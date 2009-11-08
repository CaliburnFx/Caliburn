namespace Caliburn.ModelFramework
{
    using System;

    /// <summary>
    /// A repository for instances of <see cref="IModelDefinition"/>.
    /// </summary>
    public interface IModelRepository
    {
        /// <summary>
        /// Clears the repository.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds the model to the repository.
        /// </summary>
        /// <param name="type">The instance type to which the definition applies.</param>
        /// <param name="modelDefinition">The model definition.</param>
        void AddModel(Type type, IModelDefinition modelDefinition);

        /// <summary>
        /// Gets the model for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IModelDefinition GetModelFor(Type type);
    }
}