namespace Caliburn.PresentationFramework.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using Actions;
    using Commands;
    using Core;
    using Core.Configuration;
    using Core.IoC;
    using Invocation;
    using Microsoft.Practices.ServiceLocation;
    using Parsers;
    using Screens;
    using ViewModels;
    using Action=Actions.Action;

#if SILVERLIGHT
    using System.Windows.Controls;
#endif

    public class PresentationFrameworkConfiguration :
        ConventionalModule<PresentationFrameworkConfiguration, IPresentationFrameworkServicesDescription>
    {
        private bool _registerAllScreensWithSubjects;

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

        public void RegisterAllScreensWithSubjects()
        {
            _registerAllScreensWithSubjects = true;
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
                serviceLocator.GetInstance<IViewLocator>(),
                serviceLocator.GetInstance<IViewModelBinder>()
                );

            ScreenExtensions.Initialize(
                serviceLocator.GetInstance<IViewModelFactory>()
                );

            if (!_registerAllScreensWithSubjects)
                return;

            var matches = from assembly in serviceLocator.GetInstance<IAssemblySource>()
                          from type in assembly.GetExportedTypes()
                          let service = FindInterfaceThatCloses(type, typeof(IScreen<>))
                          where service != null
                          select new PerRequest {Service = service, Implementation = type};

            var registry = serviceLocator.GetInstance<IRegistry>();
            registry.Register(matches.OfType<IComponentRegistration>());
        }

        private static Type FindInterfaceThatCloses(Type pluggedType, Type templateType)
        {
            if(!IsConcrete(pluggedType)) 
                return null;

            if(templateType.IsInterface)
            {
                foreach(Type interfaceType in pluggedType.GetInterfaces())
                {
                    if(interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == templateType)
                    {
                        return interfaceType;
                    }
                }
            }
            else if(pluggedType.BaseType.IsGenericType &&
                    pluggedType.BaseType.GetGenericTypeDefinition() == templateType)
            {
                return pluggedType.BaseType;
            }

            return pluggedType.BaseType == typeof(object)
                       ? null
                       : FindInterfaceThatCloses(pluggedType.BaseType, templateType);
        }

        private static bool IsConcrete(Type type)
        {
            return !type.IsAbstract && !type.IsInterface;
        }
    }
}