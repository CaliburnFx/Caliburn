namespace Caliburn.PresentationFramework.Conventions
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Markup;
    using Core.Logging;
    using Views;

    /// <summary>
    /// An <see cref="IViewApplicable"/> that sets a databinding on an element.
    /// </summary>
    public class ApplicableBinding : IViewApplicable
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(ApplicableBinding));

        private readonly ElementDescription elementDescription;
        private readonly DependencyProperty dependencyProperty;
        private readonly string path;
        private readonly BindingMode mode;
        private readonly bool validate;
        private readonly bool checkTemplate;
        private readonly IValueConverter converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicableBinding"/> class.
        /// </summary>
        /// <param name="elementDescription">The element description.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        /// <param name="path">The path.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="validate">Inidicates whether or not to turn on validation for the binding.</param>
        /// <param name="checkTemplate">if set to <c>true</c> [check item template].</param>
        /// <param name="converter">The value converter to apply.</param>
        public ApplicableBinding(ElementDescription elementDescription, DependencyProperty dependencyProperty, string path, 
            BindingMode mode, bool validate, bool checkTemplate, IValueConverter converter)
        {
            this.elementDescription = elementDescription;
            this.dependencyProperty = dependencyProperty;
            this.path = path;
            this.mode = mode;
            this.validate = validate;
            this.checkTemplate = checkTemplate;
            this.converter = converter;
        }

        /// <summary>
        /// Applies the behavior to the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        public void ApplyTo(DependencyObject view)
        {
            var element = view.FindName(elementDescription.Name);

            if (dependencyProperty != null && !element.HasBinding(dependencyProperty))
            {
                var binding = new Binding(path)
                {
                    Mode = mode,
                    Converter = converter
                };

                var bindableProperty = elementDescription.Convention
                    .EnsureBindableProperty(element, dependencyProperty);

                TryAddValidation(element, binding, bindableProperty);
                CheckTextBox(element, binding, bindableProperty);
                element.SetBinding(bindableProperty, binding);

                Log.Info("Applied data binding {0} to {1}.", binding, view);
            }

            if(!checkTemplate) 
                return;

            var itemsControl = (ItemsControl)element;

            if (NeedsItemTemplate(itemsControl))
            {
                itemsControl.ItemTemplate = CreateTemplate(itemsControl);
                Log.Info("Applied item template to {0}.", view);
            }
        }

        /// <summary>
        /// Tries to add validation to the binding.
        /// </summary>
        protected virtual void TryAddValidation(DependencyObject element, Binding binding, DependencyProperty dependencyProperty)
        {
            if (!validate)
                return;

#if NET || SILVERLIGHT_40
            binding.ValidatesOnDataErrors = true;
#else
            binding.ValidatesOnExceptions = true;
#endif
        }

        /// <summary>
        /// Checks if the element is a text box and uses UpdateSourceTrigger.PropertyChanged if so.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="binding">The binding.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        protected virtual void CheckTextBox(DependencyObject element, Binding binding, DependencyProperty dependencyProperty)
        {
#if !SILVERLIGHT
            if(element is TextBox && dependencyProperty == TextBox.TextProperty)
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
#else
            var textBox = element as TextBox;
            if(textBox != null && dependencyProperty == TextBox.TextProperty)
            {
                textBox.TextChanged += delegate {
                    textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                };
            }
            else
            {
                var passwordBox = element as PasswordBox;
                if (passwordBox != null && dependencyProperty == PasswordBox.PasswordProperty)
                {
                    passwordBox.PasswordChanged += delegate{
                        passwordBox.GetBindingExpression(PasswordBox.PasswordProperty).UpdateSource();
                    };
                }
            }
#endif
        }

        /// <summary>
        /// Needses the item template.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns></returns>
        protected virtual bool NeedsItemTemplate(ItemsControl control)
        {
#if !SILVERLIGHT
            return control.ItemTemplate == null
                   && control.ItemTemplateSelector == null
                   && string.IsNullOrEmpty(control.DisplayMemberPath);
#else
            return control.ItemTemplate == null && string.IsNullOrEmpty(control.DisplayMemberPath);
#endif
        }

        private const string TemplateCore =
            "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:v='clr-namespace:Caliburn.PresentationFramework.Views;assembly=Caliburn.PresentationFramework'> " +
                "<ContentControl v:View.Model=\"{Binding}\" ";

        /// <summary>
        /// Creates an item template which binds view models.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns></returns>
        protected virtual DataTemplate CreateTemplate(Control control)
        {
            var context = View.GetContext(control);
            var template = TemplateCore;

            if (context != null)
                template += "v:View.Context=\"" + context + "\"";

            template += " /></DataTemplate>";

#if SILVERLIGHT
            return (DataTemplate)XamlReader.Load(template);
#else
            return (DataTemplate)XamlReader.Parse(template);
#endif
        }
    }
}