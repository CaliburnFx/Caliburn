namespace Caliburn.PresentationFramework.Configuration
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using Actions;
    using ApplicationModel;
    using Commands;
    using Core.Configuration;
    using Core.Invocation;
    using Invocation;
    using Microsoft.Practices.ServiceLocation;
    using Parsers;
    using ViewModels;
    using Action=Actions.Action;

    public class PresentationFrameworkConfiguration :
        ConventionalModule<PresentationFrameworkConfiguration, IPresentationFrameworkServicesDescription>
    {
        private IEventHandlerFactory _eventHandlerFactory;
        private IRoutedMessageController _controller;

#if !SILVERLIGHT
        private static readonly bool _isInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
#else
        private static readonly bool _isInDesignMode = DesignerProperties.GetIsInDesignMode(new UserControl());
#endif

        public PresentationFrameworkConfiguration()
        {
            CaliburnModule<CoreConfiguration>
                .Instance
                .Using(x => x.Dispatcher<DispatcherImplementation>());
        }

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

        protected override void InitializeCore(IServiceLocator serviceLocator)
        {
            _controller = serviceLocator.GetInstance<IRoutedMessageController>();
            _eventHandlerFactory = serviceLocator.GetInstance<IEventHandlerFactory>();

            var messageBinder = serviceLocator.GetInstance<IMessageBinder>();
            var parser = serviceLocator.GetInstance<IParser>();

            parser.RegisterMessageParser("Action", new ActionMessageParser(messageBinder));
            parser.RegisterMessageParser("ResourceCommand", new CommandMessageParser(messageBinder, CommandSource.Resource));
            parser.RegisterMessageParser("ContainerCommand", new CommandMessageParser(messageBinder, CommandSource.Container));
            parser.RegisterMessageParser("BoundCommand", new CommandMessageParser(messageBinder, CommandSource.Bound));

            SetupDefaultInteractions();

            Message.Initialize(
                _controller,
                parser
                );

            Action.Initialize(
                _controller,
                serviceLocator.GetInstance<IViewModelDescriptionBuilder>(),
                serviceLocator
                );

            View.Initialize(
                serviceLocator.GetInstance<IViewStrategy>(),
                serviceLocator.GetInstance<IBinder>()
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
                                   c =>
                                   {
                                       if (c.SelectionMode == SelectionMode.Extended ||
                                          c.SelectionMode == SelectionMode.Multiple)
                                           return c.SelectedItems;
                                       return c.SelectedItem;
                                   }),
                Defaults<ListBox>("SelectionChanged", (c, o) => c.ItemsSource = (IEnumerable)o,
                                  c =>
                                  {
                                      if (c.SelectionMode == SelectionMode.Extended ||
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

        private GenericInteractionDefaults<T> Defaults<T>(string eventName, Action<T, object> setter, Func<T, object> getter)
        {
            return new GenericInteractionDefaults<T>(
                _eventHandlerFactory,
                eventName,
                setter,
                getter
                );
        }
    }
}