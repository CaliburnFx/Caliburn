namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Markup;

    /// <summary>
    /// Hosts attached properties related to view models.
    /// </summary>
    public static class View
    {
        private static IViewStrategy _viewStrategy;
        private static IBinder _binder;

        /// <summary>
        /// Initializes the framework with the specified view strategy.
        /// </summary>
        /// <param name="viewStrategy">The view strategy.</param>
        /// <param name="binder">The default binder.</param>
        public static void Initialize(IViewStrategy viewStrategy, IBinder binder)
        {
            _viewStrategy = viewStrategy;
            _binder = binder;
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
        /// A dependency property for assigning an <see cref="IViewStrategy"/> to a UI element.
        /// </summary>
        public static readonly DependencyProperty StrategyProperty =
            DependencyProperty.RegisterAttached(
                "Strategy",
                typeof(IViewStrategy),
                typeof(View),
                new PropertyMetadata(OnStrategyChanged)
                );

        /// <summary>
        /// Gets the <see cref="IViewStrategy"/>.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>The strategy.</returns>
        public static IViewStrategy GetStrategy(DependencyObject d)
        {
            return d.GetValue(StrategyProperty) as IViewStrategy;
        }

        /// <summary>
        /// Sets the <see cref="IViewStrategy"/>.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="value">The value.</param>
        public static void SetStrategy(DependencyObject d, IViewStrategy value)
        {
            d.SetValue(StrategyProperty, value);
        }

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var actualStrategy = GetStrategy(d) ?? _viewStrategy;

            if (actualStrategy == null)
                return;

            if(e.OldValue == e.NewValue)
                return;

            if(e.NewValue != null)
            {
                var context = GetContext(d);
                var view = actualStrategy.GetView(e.NewValue, d, context);

                _binder.Bind(e.NewValue, view, context);

                SetContentProperty(d, view);
            }
            else SetContentProperty(d, e.NewValue);
        }

        private static void OnContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var actualStrategy = GetStrategy(d) ?? _viewStrategy;

            if (actualStrategy == null)
                return;

            if (e.OldValue == e.NewValue)
                return;

            var model = GetModel(d);
            if(model == null)
                return;

            var view = actualStrategy.GetView(model, d, e.NewValue);

            _binder.Bind(model, view, e.NewValue);

            SetContentProperty(d, view);
        }

        private static void OnStrategyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            var actualStrategy = e.NewValue as IViewStrategy ?? _viewStrategy;

            if (actualStrategy == null)
                return;

            var model = GetModel(d);
            if (model == null)
                return;

            var context = GetContext(d);
            var view = actualStrategy.GetView(model, d, context);

            _binder.Bind(model, view, context);
            SetContentProperty(d, view);
        }

        private static void SetContentProperty(DependencyObject d, object view)
        {
            var contentProperty = d.GetType().GetCustomAttributes(typeof(ContentPropertyAttribute), true)
                                      .OfType<ContentPropertyAttribute>().FirstOrDefault()
                                  ?? new ContentPropertyAttribute("Content");

            d.GetType().GetProperty(contentProperty.Name)
                .SetValue(d, view, null);
        }
    }
}