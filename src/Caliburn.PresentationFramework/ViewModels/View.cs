namespace Caliburn.PresentationFramework.ViewModels
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Markup;
    using Core;

    /// <summary>
    /// Hosts attached properties related to view models.
    /// </summary>
    public static class View
    {
        private static IViewLocator _viewLocator;
        private static IViewModelBinder _viewModelBinder;

        /// <summary>
        /// Initializes the framework with the specified view locator and view model binder.
        /// </summary>
        /// <param name="viewLocator">The view locator.</param>
        /// <param name="viewModelBinder">The view model binder.</param>
        public static void Initialize(IViewLocator viewLocator, IViewModelBinder viewModelBinder)
        {
            _viewLocator = viewLocator;
            _viewModelBinder = viewModelBinder;
        }

	/// <summary>
        /// A dependency property for assigning a context to a particular portion of the UI.
        /// </summary>
        public static readonly DependencyProperty ContextProperty =
            DependencyProperty.RegisterAttached(
                "Context",
                typeof(object),
                typeof(View),
                new PropertyMetadata(OnContextChanged)
                );

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static object GetContext(DependencyObject d)
        {
            return d.GetValue(ContextProperty);
        }

        /// <summary>
        /// Sets the context.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="value">The value.</param>
        public static void SetContext(DependencyObject d, object value)
        {
            d.SetValue(ContextProperty, value);
        }

        /// <summary>
        /// A dependency property for attaching a model to the UI.
        /// </summary>
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.RegisterAttached(
                "Model",
                typeof(object),
                typeof(View),
                new PropertyMetadata(OnModelChanged)
                );

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static object GetModel(DependencyObject d)
        {
            return d.GetValue(ModelProperty);
        }

        /// <summary>
        /// Sets the model.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="value">The value.</param>
        public static void SetModel(DependencyObject d, object value)
        {
            d.SetValue(ModelProperty, value);
        }


        /// <summary>
        /// A dependency property for assigning an <see cref="IViewLocator"/> to a UI element.
        /// </summary>
        public static readonly DependencyProperty StrategyProperty =
            DependencyProperty.RegisterAttached(
                "Strategy",
                typeof(IViewLocator),
                typeof(View),
                new PropertyMetadata(OnStrategyChanged)
                );

        /// <summary>
        /// Gets the <see cref="IViewLocator"/>.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>The strategy.</returns>
        public static IViewLocator GetStrategy(DependencyObject d)
        {
            return d.GetValue(StrategyProperty) as IViewLocator;
        }

        /// <summary>
        /// Sets the <see cref="IViewLocator"/>.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="value">The value.</param>
        public static void SetStrategy(DependencyObject d, IViewLocator value)
        {
            d.SetValue(StrategyProperty, value);
        }

        /// <summary>
        /// A dependency property which allows the override of convention application behavior.
        /// </summary>
        public static readonly DependencyProperty ApplyConventionsProperty =
            DependencyProperty.RegisterAttached(
                "ApplyConventions",
                typeof(bool?),
                typeof(View),
                null
                );

        /// <summary>
        /// Gets the convention application behavior.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>The strategy.</returns>
        public static bool? GetApplyConventions(DependencyObject d)
        {
            return (bool?)d.GetValue(ApplyConventionsProperty);
        }

        /// <summary>
        /// Sets the convention application behavior.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="value">The value.</param>
        public static void SetApplyConventions(DependencyObject d, bool? value)
        {
            d.SetValue(ApplyConventionsProperty, value);
        }

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var locator = GetStrategy(d) ?? _viewLocator;

            if (locator == null)
                return;

            if(e.OldValue == e.NewValue)
                return;

            if(e.NewValue != null)
            {
                var context = GetContext(d);
                var view = locator.Locate(e.NewValue, d, context);

                _viewModelBinder.Bind(e.NewValue, view, context, true);

                SetContentProperty(d, view);
            }
            else SetContentProperty(d, e.NewValue);
        }

        private static void OnContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var locator = GetStrategy(d) ?? _viewLocator;

            if (locator == null)
                return;

            if (e.OldValue == e.NewValue)
                return;

            var model = GetModel(d);
            if(model == null)
                return;

            var view = locator.Locate(model, d, e.NewValue);

            _viewModelBinder.Bind(model, view, e.NewValue, true);

            SetContentProperty(d, view);
        }

        private static void OnStrategyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            var locator = e.NewValue as IViewLocator ?? _viewLocator;

            if (locator == null)
                return;

            var model = GetModel(d);
            if (model == null)
                return;

            var context = GetContext(d);
            var view = locator.Locate(model, d, context);

            _viewModelBinder.Bind(model, view, context, true);
            SetContentProperty(d, view);
        }

        private static void SetContentProperty(DependencyObject d, object view)
        {
            var type = d.GetType();
            var contentProperty = type.GetAttributes<ContentPropertyAttribute>(true)
                                      .FirstOrDefault() ?? new ContentPropertyAttribute("Content");

            type.GetProperty(contentProperty.Name)
                .SetValue(d, view, null);
        }
    }
}