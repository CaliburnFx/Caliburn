namespace Caliburn.PresentationFramework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using Actions;
    using ApplicationModel;
    using Commands;
    using Core;
    using Core.Invocation;
    using Core.IoC;
    using Invocation;
    using Parsers;

    /// <summary>
    /// The configuration for the routed UI messaging mechanism.
    /// </summary>
    public class PresentationFrameworkModule : CaliburnModule
    {
#if !SILVERLIGHT
        private static readonly bool _isInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
#else
        private static readonly bool _isInDesignMode = DesignerProperties.GetIsInDesignMode(new UserControl());
#endif
        /// <summary>
        /// Gets a value indicating whether the framework is in design mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the framework is in design mode; otherwise, <c>false</c>.
        /// </value>
        public static bool IsInDesignMode
        {
            get { return _isInDesignMode; }
        }

        private Type _routedMessageControllerType;
        private Type _messsageBinderType;
        private Type _parserType;
        private IRoutedMessageController _controller;
        private Type _actionFactoryType;
        private Type _viewStrategy;
        private Type _binderType;

#if !SILVERLIGHT
        private Type _windowManagerType;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationFrameworkModule"/> class.
        /// </summary>
        /// <param name="hook">The hook.</param>
        public PresentationFrameworkModule(IConfigurationHook hook)
            : base(hook)
        {
            UsingRoutedMessageController<RoutedMessageController>();
            UsingMessageBinder<MessageBinder>();
            UsingParser<Parser>();
            UsingActionFactory<ActionFactory>();
            UsingDispatcher<DispatcherImplementation>();
            UsingViewStrategy<DefaultViewStrategy>();
            UsingBinder<DefaultBinder>();
#if !SILVERLIGHT
            UsingWindowManager<DefaultWindowManager>();
#endif
        }

        /// <summary>
        /// Customizes the routed message controller used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The routed message controller type.</typeparam>
        /// <returns>The configuration.</returns>
        public PresentationFrameworkModule UsingRoutedMessageController<T>()
            where T : IRoutedMessageController
        {
            _routedMessageControllerType = typeof(T);
            return this;
        }

        /// <summary>
        /// Customizes the method binder used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The method binder type.</typeparam>
        /// <returns>The configuration.</returns>
        public PresentationFrameworkModule UsingMessageBinder<T>()
            where T : IMessageBinder
        {
            _messsageBinderType = typeof(T);
            return this;
        }

        /// <summary>
        /// Customizes the message parser used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The message parser type.</typeparam>
        /// <returns>The configuration.</returns>
        public PresentationFrameworkModule UsingParser<T>()
            where T : IParser
        {
            _parserType = typeof(T);
            return this;
        }

        /// <summary>
        /// Customizes the dispatcher implementation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public PresentationFrameworkModule UsingDispatcher<T>()
            where T : IDispatcher
        {
            ((IConfigurationHook)this).Core.UsingDispatcher<T>();
            return this;
        }

        /// <summary>
        /// Customizes the action factory used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The action factory type.</typeparam>
        /// <returns>The configuration.</returns>
        public PresentationFrameworkModule UsingActionFactory<T>()
            where T : IActionFactory
        {
            _actionFactoryType = typeof(T);
            return this;
        }

        /// <summary>
        /// Customizes the view strategy used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The view strategy type.</typeparam>
        /// <returns>The configuration.</returns>
        public PresentationFrameworkModule UsingViewStrategy<T>()
            where T : IViewStrategy
        {
            _viewStrategy = typeof(T);
            return this;
        }

        /// <summary>
        /// Customizes the binder used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The binder type.</typeparam>
        /// <returns>The configuration.</returns>
        public PresentationFrameworkModule UsingBinder<T>()
            where T : IBinder
        {
            _binderType = typeof(T);
            return this;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Customizes the window manager used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The window manager type.</typeparam>
        /// <returns>The configuration.</returns>
        public PresentationFrameworkModule UsingWindowManager<T>()
            where T : IWindowManager
        {
            _windowManagerType = typeof(T);
            return this;
        }
#endif

        /// <summary>
        /// Gets the component information for this module.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IComponentRegistration> GetComponents()
        {
            yield return Singleton(typeof(IRoutedMessageController), _routedMessageControllerType);
            yield return Singleton(typeof(IMessageBinder), _messsageBinderType);
            yield return Singleton(typeof(IParser), _parserType);
            yield return Singleton(typeof(IActionFactory), _actionFactoryType);
            yield return Singleton(typeof(IViewStrategy), _viewStrategy);
            yield return Singleton(typeof(IBinder), _binderType);
#if !SILVERLIGHT
            yield return Singleton(typeof(IWindowManager), _windowManagerType);
#endif
        }

        /// <summary>
        /// Initializes this module.
        /// </summary>
        protected override void Initialize()
        {
            _controller = ServiceLocator.GetInstance<IRoutedMessageController>();
            SetupDefaultInteractions();
            Message.Initialize(_controller, ServiceLocator.GetInstance<IParser>());

            Actions.Action.Initialize(
                ServiceLocator.GetInstance<IRoutedMessageController>(),
                ServiceLocator.GetInstance<IActionFactory>(),
                ServiceLocator
                );

            var parser = ServiceLocator.GetInstance<IParser>();
            var binder = ServiceLocator.GetInstance<IMessageBinder>();

            parser.RegisterMessageParser("Action", new ActionMessageParser(binder));
            parser.RegisterMessageParser("ResourceCommand", new CommandMessageParser(binder, CommandSource.Resource));
            parser.RegisterMessageParser("ContainerCommand", new CommandMessageParser(binder, CommandSource.Container));
            parser.RegisterMessageParser("BoundCommand", new CommandMessageParser(binder, CommandSource.Bound));

            View.Initialize(
                ServiceLocator.GetInstance<IViewStrategy>(),
                ServiceLocator.GetInstance<IBinder>()
                );
        }

        /// <summary>
        /// Sets up the default interactions.
        /// </summary>
        protected virtual void SetupDefaultInteractions()
        {
            _controller.SetupDefaults(
#if !SILVERLIGHT
                Defaults<RichTextBox>("TextChanged", (c, o) => c.Document = (FlowDocument)o, c => c.Document),
                Defaults<Menu>("Click", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<MenuItem>("Click", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<Label>("DataContextChanged", (c, o) => c.Content = o, c => c.Content),
                Defaults<DockPanel>("Loaded", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<UniformGrid>("Loaded", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<WrapPanel>("Loaded", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<Viewbox>("Loaded", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<BulletDecorator>("Loaded", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<Slider>("ValueChanged", (c, o) => c.Value = (double)o, c => c.Value),
                Defaults<Expander>("Expanded", (c, o) => c.IsExpanded = (bool)o, c => c.IsExpanded),
                Defaults<Window>("Loaded", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<StatusBar>("Loaded", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<ToolBar>("Loaded", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<ToolBarTray>("Loaded", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<TreeView>("SelectedItemChanged", (c, o) => c.ItemsSource = (IEnumerable)o, c => c.SelectedItem),
                Defaults<TabControl>("SelectionChanged", (c, o) => c.ItemsSource = (IEnumerable)o, c => c.SelectedItem),
                Defaults<TabItem>("DataContextChanged", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<ListView>("SelectionChanged", (c, o) => c.ItemsSource = (IEnumerable)o,
                                   c =>{
                                       if(c.SelectionMode == SelectionMode.Extended ||
                                          c.SelectionMode == SelectionMode.Multiple)
                                           return c.SelectedItems;
                                       return c.SelectedItem;
                                   }),
                Defaults<ListBox>("SelectionChanged", (c, o) => c.ItemsSource = (IEnumerable)o,
                                  c =>{
                                      if(c.SelectionMode == SelectionMode.Extended ||
                                         c.SelectionMode == SelectionMode.Multiple)
                                          return c.SelectedItems;
                                      return c.SelectedItem;
                                  }),
                Defaults<ComboBox>("SelectionChanged", (c, o) => c.ItemsSource = (IEnumerable)o,
                                   c => c.IsEditable ? c.Text : c.SelectedItem),
#else
                Defaults<ListBox>("SelectionChanged", (c, o) => c.ItemsSource = (IEnumerable) o, c => c.SelectedItem),
                Defaults<ComboBox>("SelectionChanged", (c, o) => c.ItemsSource = (IEnumerable) o, c => c.SelectedItem),
#endif
                Defaults<ButtonBase>("Click", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<Button>("Click", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<ToggleButton>("Click", (c, o) => c.IsChecked = (bool)o, c => c.IsChecked),
                Defaults<RadioButton>("Click", (c, o) => c.IsChecked = (bool)o, c => c.IsChecked),
                Defaults<CheckBox>("Click", (c, o) => c.IsChecked = (bool)o, c => c.IsChecked),
                Defaults<TextBox>("TextChanged", (c, o) => c.Text = o.SafeToString(), c => c.Text),
                Defaults<PasswordBox>("PasswordChanged", (c, o) => c.Password = o.SafeToString(), c => c.Password),
                Defaults<TextBlock>("DataContextChanged", (c, o) => c.Text = o.SafeToString(), c => c.Text),
                Defaults<StackPanel>("Loaded", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<Grid>("Loaded", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<Border>("Loaded", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<ContentControl>("Loaded", (c, o) => c.DataContext = o, c => c.DataContext),
                Defaults<UserControl>("Loaded", (c, o) => c.DataContext = o, c => c.DataContext)
                );
        }

        private GenericInteractionDefaults<T> Defaults<T>(string eventName, Action<T, object> setter,
                                                          Func<T, object> getter)
        {
            return new GenericInteractionDefaults<T>(
                ServiceLocator.GetInstance<IEventHandlerFactory>(),
                eventName,
                setter,
                getter
                );
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            var other = obj as PresentationFrameworkModule;
            return other != null;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(PresentationFrameworkModule).GetHashCode();
        }
    }
}