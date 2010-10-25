namespace Caliburn.PresentationFramework.Views
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Markup;
    using Core;
    using RoutedMessaging;
    using ViewModels;

    /// <summary>
    /// Hosts attached properties related to view models.
    /// </summary>
    public static class View
    {
        private static IViewLocator viewLocator;
        private static IViewModelBinder viewModelBinder;

        /// <summary>
        /// Initializes the framework with the specified view locator and view model binder.
        /// </summary>
        /// <param name="viewLocator">The view locator.</param>
        /// <param name="viewModelBinder">The view model binder.</param>
        public static void Initialize(IViewLocator viewLocator, IViewModelBinder viewModelBinder)
        {
            View.viewLocator = viewLocator;
            View.viewModelBinder = viewModelBinder;
        }


        /// <summary>
        /// Get the <see cref="IInteractionNode"/> associated with the view.
        /// </summary>
        public static Func<DependencyObject, IInteractionNode> GetInteractionNode =
            view =>{
                IInteractionNode node = null;
                if(view != null)
                    node = view.GetValue(DefaultRoutedMessageController.NodeProperty) as IInteractionNode;
                return node;
            };

        /// <summary>
        /// Gets the view instance associated with the model.
        /// </summary>
        public static Func<object, object, DependencyObject> GetViewInstanceFromModel =
            (model, context) =>{
                var viewAware = model as IViewAware;
                return viewAware != null ? viewAware.GetView(context) as DependencyObject : null;
            };

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
        /// <param name="d">The element the context is attached to.</param>
        /// <returns>The context.</returns>
        public static object GetContext(DependencyObject d)
        {
            return d.GetValue(ContextProperty);
        }

        /// <summary>
        /// Sets the context.
        /// </summary>
        /// <param name="d">The element to attach the context to.</param>
        /// <param name="value">The context.</param>
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
        /// <param name="d">The element the model is attached to.</param>
        /// <returns>The model.</returns>
        public static object GetModel(DependencyObject d)
        {
            return d.GetValue(ModelProperty);
        }

        /// <summary>
        /// Sets the model.
        /// </summary>
        /// <param name="d">The element to attach the model to.</param>
        /// <param name="value">The model.</param>
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
        /// <param name="d">The element the strategy is attached to.</param>
        /// <returns>The strategy.</returns>
        public static IViewLocator GetStrategy(DependencyObject d)
        {
            return d.GetValue(StrategyProperty) as IViewLocator;
        }

        /// <summary>
        /// Sets the <see cref="IViewLocator"/>.
        /// </summary>
        /// <param name="d">The element to attach the strategy to.</param>
        /// <param name="value">The strategy.</param>
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
        /// <param name="d">The element the property is attached to.</param>
        /// <returns>Whether or not to apply conventions.</returns>
        public static bool? GetApplyConventions(DependencyObject d)
        {
            return (bool?)d.GetValue(ApplyConventionsProperty);
        }

        /// <summary>
        /// Sets the convention application behavior.
        /// </summary>
        /// <param name="d">The element to attach the property to.</param>
        /// <param name="value">Whether or not to apply conventions.</param>
        public static void SetApplyConventions(DependencyObject d, bool? value)
        {
            d.SetValue(ApplyConventionsProperty, value);
        }

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var locator = GetStrategy(d) ?? viewLocator;

            if (locator == null)
                return;

            if(e.OldValue == e.NewValue)
                return;

            if(e.NewValue != null)
            {
                var context = GetContext(d);
                var view = locator.LocateForModel(e.NewValue, d, context);

                viewModelBinder.Bind(e.NewValue, view, context);

                SetContentProperty(d, view);
            }
            else SetContentProperty(d, e.NewValue);
        }

        private static void OnContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var locator = GetStrategy(d) ?? viewLocator;

            if (locator == null)
                return;

            if (e.OldValue == e.NewValue)
                return;

            var model = GetModel(d);
            if(model == null)
                return;

            var view = locator.LocateForModel(model, d, e.NewValue);

            viewModelBinder.Bind(model, view, e.NewValue);

            SetContentProperty(d, view);
        }

        private static void OnStrategyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            var locator = e.NewValue as IViewLocator ?? viewLocator;

            if (locator == null)
                return;

            var model = GetModel(d);
            if (model == null)
                return;

            var context = GetContext(d);
            var view = locator.LocateForModel(model, d, context);

            viewModelBinder.Bind(model, view, context);
            SetContentProperty(d, view);
        }

        private static void SetContentProperty(object targetLocation, object view)
        {
            var fe = view as FrameworkElement;
            if (fe != null && fe.Parent != null)
                SetContentPropertyCore(fe.Parent, null);

            SetContentPropertyCore(targetLocation, view);
        }

        private static void SetContentPropertyCore(object targetLocation, object view)
        {
            var type = targetLocation.GetType();
            var contentProperty = type.GetAttributes<ContentPropertyAttribute>(true)
                .FirstOrDefault() ?? new ContentPropertyAttribute("Content");

            type.GetProperty(contentProperty.Name)
                .SetValue(targetLocation, view, null);
        }
    }
}