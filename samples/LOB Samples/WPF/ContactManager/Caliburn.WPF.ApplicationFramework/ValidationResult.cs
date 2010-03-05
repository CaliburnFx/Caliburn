namespace Caliburn.WPF.ApplicationFramework
{
    using System.Windows.Controls;
    using ModelFramework;

    public class ValidationResult : IValidationResult
    {
        private readonly IProperty _invalidProperty;

        public ValidationResult(string message, IProperty invalidProperty)
        {
            _invalidProperty = invalidProperty;
            Message = message;
        }

        public string Message { get; private set; }

        public void DrawAttentionTo()
        {
            var view = (Control)_invalidProperty.GetView(null);
            view.Focus();

            var box = view as TextBox;
            if(box != null) box.SelectAll();
        }
    }
}