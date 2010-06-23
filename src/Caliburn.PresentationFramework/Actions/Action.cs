namespace Caliburn.PresentationFramework.Actions
{
    using System.Windows;
    using Microsoft.Practices.ServiceLocation;
    using RoutedMessaging;
    using ViewModels;

    /// <summary>
    /// A host for action related attached properties.
    /// </summary>
    public static class Action
    {
        private static IRoutedMessageController _controller;
        private static IViewModelDescriptionFactory _viewModelDescriptionFactory;
        private static IServiceLocator _serviceLocator;

        /// <summary>
        /// A property definition representing the target of an <see cref="ActionMessage"/>.  
        /// The DataContext of the element will be set to this instance.
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.RegisterAttached(
                "Target",
                typeof(object),
                typeof(Action),
                new PropertyMetadata(OnTargetChanged)
                );

        /// <summary>
        /// A property definition representing the target of an <see cref="ActionMessage"/>.  
        /// The DataContext of the element is not set to this instance.
        /// </summary>
        public static readonly DependencyProperty TargetWithoutContextProperty =
            DependencyProperty.RegisterAttached(
                "TargetWithoutContext",
                typeof(object),
                typeof(Action),
                new PropertyMetadata(OnTargetWithoutContextChanged)
                );

        /// <summary>
        /// Initializes property host.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="viewModelDescriptionFactory"></param>
        /// <param name="serviceLocator">The service locator.</param>
        public static void Initialize(IRoutedMessageController controller, IViewModelDescriptionFactory viewModelDescriptionFactory,
                                      IServiceLocator serviceLocator)
        {
            _controller = controller;
            _viewModelDescriptionFactory = viewModelDescriptionFactory;
            _serviceLocator = serviceLocator;
        }

        /// <summary>
        /// Sets the target of the <see cref="ActionMessage"/>.
        /// </summary>
        /// <param name="d">The element to attach the target to.</param>
        /// <param name="target">The target for instances of <see cref="ActionMessage"/>.</param>
        public static void SetTarget(DependencyObject d, object target)
        {
            d.SetValue(TargetProperty, target);
        }

        /// <summary>
        /// Gets the target for instances of <see cref="ActionMessage"/>.
        /// </summary>
        /// <param name="d">The element to which the target is attached.</param>
        /// <returns>The target for instances of <see cref="ActionMessage"/>.</returns>
        public static object GetTarget(DependencyObject d)
        {
            return d.GetValue(TargetProperty);
        }

        /// <summary>
        /// Sets the target of the <see cref="ActionMessage"/>.
        /// </summary>
        /// <param name="d">The element to attach the target to.</param>
        /// <param name="target">The target for instances of <see cref="ActionMessage"/>.</param>
        /// <remarks>The DataContext will not be set.</remarks>
        public static void SetTargetWithoutContext(DependencyObject d, object target)
        {
            d.SetValue(TargetWithoutContextProperty, target);
        }

        /// <summary>
        /// Gets the target for instances of <see cref="ActionMessage"/>.
        /// </summary>
        /// <param name="d">The element to which the target is attached.</param>
        /// <returns>The target for instances of <see cref="ActionMessage"/></returns>
        public static object GetTargetWithoutContext(DependencyObject d)
        {
            return d.GetValue(TargetWithoutContextProperty);
        }

        private static void OnTargetWithoutContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetTargetCore(e, d, false);
        }

        private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetTargetCore(e, d, true);
        }

        private static void SetTargetCore(DependencyPropertyChangedEventArgs e, DependencyObject d, bool setContext)
        {
            if(_controller == null) return;

            if(e.NewValue != e.OldValue && e.NewValue != null)
            {
                var target = e.NewValue;

#if SILVERLIGHT
                var containerKey = e.NewValue as string;

                if(containerKey != null)
                    target = _serviceLocator.GetInstance(null, containerKey);
#endif

                var handler = new ActionMessageHandler(
                    _viewModelDescriptionFactory.Create(target.GetType()),
                    target
                    );

                _controller.AddHandler(d, handler, setContext);
            }
        }
    }
}