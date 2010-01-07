namespace Caliburn.PresentationFramework.ViewModels
{
    public class DefaultValidationError : IValidationError
    {
        public object Instance { get; private set; }
        public string PropertyName { get; private set; }
        public string Message { get; private set; }

        public DefaultValidationError(object instance, string propertyName, string message)
        {
            Instance = instance;
            PropertyName = propertyName;
            Message = message;
        }
    }
}