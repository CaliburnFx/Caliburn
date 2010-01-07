namespace Caliburn.PresentationFramework.ViewModels
{
    public interface IValidationError
    {
        object Instance { get; }
        string PropertyName { get; }
        string Message { get; }
    }
}