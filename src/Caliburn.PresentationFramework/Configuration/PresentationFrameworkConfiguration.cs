namespace Caliburn.PresentationFramework.Configuration
{
    using System.ComponentModel;
    using System.Windows;
    using Actions;
    using ApplicationModel;
    using Commands;
    using Core.Configuration;
    using Invocation;
    using Microsoft.Practices.ServiceLocation;
    using Parsers;
    using ViewModels;
    using Action=Actions.Action;

#if SILVERLIGHT
    using System.Windows.Controls;
#endif

    public class PresentationFrameworkConfiguration :
        ConventionalModule<PresentationFrameworkConfiguration, IPresentationFrameworkServicesDescription>
    {
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
            var controller = serviceLocator.GetInstance<IRoutedMessageController>();
            var messageBinder = serviceLocator.GetInstance<IMessageBinder>();
            var parser = serviceLocator.GetInstance<IParser>();

            parser.RegisterMessageParser("Action", new ActionMessageParser(messageBinder));
            parser.RegisterMessageParser("ResourceCommand", new CommandMessageParser(messageBinder, CommandSource.Resource));
            parser.RegisterMessageParser("ContainerCommand", new CommandMessageParser(messageBinder, CommandSource.Container));
            parser.RegisterMessageParser("BoundCommand", new CommandMessageParser(messageBinder, CommandSource.Bound));

            Message.Initialize(
                controller,
                parser
                );

            Action.Initialize(
                controller,
                serviceLocator.GetInstance<IViewModelDescriptionFactory>(),
                serviceLocator
                );

            View.Initialize(
                serviceLocator.GetInstance<IViewStrategy>(),
                serviceLocator.GetInstance<IBinder>()
                );
        }
    }
}