namespace Caliburn.ModelFramework
{
    /// <summary>
    /// Implemented by types capable of validating a presentation model.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IModelValidator<T>
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