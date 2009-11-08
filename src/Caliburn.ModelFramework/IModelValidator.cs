namespace Caliburn.ModelFramework
{
    using Core.Metadata;

    /// <summary>
    /// Implemented by types capable of validating a presentation model.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IModelValidator<T> : IMetadata
        where T : IModel
    {
        /// <summary>
        /// Interrogates the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        bool Interrogate(T model);
    }
}