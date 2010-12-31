namespace Caliburn.PresentationFramework.ViewModels
{
    using System.Windows;

    /// <summary>
    /// Hosts dependency properties for binding.
    /// </summary>
    public static class Bind
    {
        static IViewModelBinder binder;

        /// <summary>
        /// Initializes the binder attached properties.
        /// </summary>
        /// <param name="binder">The binder.</param>
        public static void Initialize(IViewModelBinder binder)
        {
            Bind.binder = binder;
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
        /// Gets the model to bind to.
        /// </summary>
        /// <param name="dependencyObject">The dependency object to bind to.</param>
        /// <returns>The model.</returns>
        public static object GetModel(DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(ModelProperty);
        }

        /// <summary>
        /// Sets the model to bind to.
        /// </summary>
        /// <param name="dependencyObject">The dependency object to bind to.</param>
        /// <param name="value">The model.</param>
        public static void SetModel(DependencyObject dependencyObject, object value)
        {
            dependencyObject.SetValue(ModelProperty, value);
        }

        static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue == null || e.NewValue == e.OldValue)
                return;

            binder.Bind(e.NewValue, d, null);
        }
    }
}