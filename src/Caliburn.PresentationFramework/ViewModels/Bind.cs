namespace Caliburn.PresentationFramework.ViewModels
{
    using System.Windows;

    /// <summary>
    /// Hosts dependency properties for binding.
    /// </summary>
    public static class Bind
    {
        private static IViewModelBinder _binder;

        /// <summary>
        /// Initializes the binder attached properties.
        /// </summary>
        /// <param name="binder">The binder.</param>
        public static void Initialize(IViewModelBinder binder)
        {
            _binder = binder;
        }

        /// <summary>
        /// Allows binding on an existing view.
        /// </summary>
        public static DependencyProperty ModelProperty =
            DependencyProperty.RegisterAttached(
                "Model",
                typeof(object),
                typeof(Bind),
                new PropertyMetadata(new PropertyChangedCallback(ModelChanged))
                );

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns></returns>
        public static object GetModel(DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(ModelProperty);
        }

        /// <summary>
        /// Sets the model.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="value">The value.</param>
        public static void SetModel(DependencyObject dependencyObject, object value)
        {
            dependencyObject.SetValue(ModelProperty, value);
        }

        private static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue == null || e.NewValue == e.OldValue)
                return;

            _binder.Bind(e.NewValue, d, null);
        }
    }
}