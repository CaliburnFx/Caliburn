namespace Caliburn.ModelFramework
{
    /// <summary>
    /// A special type of <see cref="IPropertyChangeAware{T}"/> designed for property validation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPropertyValidator<T> : IPropertyChangeAware<T>
    {
        
    }
}