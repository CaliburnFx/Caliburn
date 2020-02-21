namespace Caliburn.PresentationFramework.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using Actions;
    using Behaviors;
    using Commands;
    using Conventions;
    using Core;
    using Core.Configuration;
    using Core.InversionOfControl;
    using Core.Validation;
    using Invocation;
    using RoutedMessaging;
    using RoutedMessaging.Parsers;
    using Screens;
    using ViewModels;
    using Views;
    using Action=Actions.Action;

    /// <summary>
    /// The presenation framework module.
    /// </summary>
    public class PresentationFrameworkConfiguration :
        ConventionalModule<PresentationFrameworkConfiguration, IPresentationFrameworkServicesDescription>
    {
        bool registerItemsWithSubjects;
        bool nameInstances;

        static bool? isInDesignMode;
        Func<IEnumerator<IResult>, IResult> parentEnumeratorFactory = results => new SequentialResult(results);

        /// <summary>
        /// Gets a value indicating whether the framework is in design mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the framework is in design mode; otherwise, <c>false</c>.
        /// </value>
        public static bool IsInDesignMode
        {
            get
            {
                if (isInDesignMode == null)
                {
                    var prop = DesignerProperties.IsInDesignModeProperty;
                    isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;

                    if (!isInDesignMode.GetValueOrDefault(false) && Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.Ordinal))
                        isInDesignMode = true;
                }

                return isInDesignMode.GetValueOrDefault(false);
            }
            set { isInDesignMode = value; }
        }

        /// <summary>
        /// Searches the <see cref="IAssemblySource"/> and registers all screens which concretely implement <see cref="IScreen{T}"/> using their closed interface type.
        /// </summary>
        public PresentationFrameworkConfiguration RegisterAllItemsWithSubjects()
        {
            return RegisterAllItemsWithSubjects(false);
        }

        /// <summary>
        /// Searches the <see cref="IAssemblySource"/> and registers all screens which concretely implement <see cref="IScreen{T}"/> using their closed interface type.
        /// </summary>
        public PresentationFrameworkConfiguration RegisterAllItemsWithSubjects(bool nameInstances)
        {
            registerItemsWithSubjects = true;
            this.nameInstances = nameInstances;
            return this;
        }

        /// <summary>
        /// Customizes the enumerator factory that is used for coroutines.
        /// </summary>
        /// <param name="parentEnumeratorFactory">The parent enumerator factory.</param>
        /// <returns>The configuration.</returns>
        public PresentationFrameworkConfiguration UseEnumeratorFactory(Func<IEnumerator<IResult>, IResult> parentEnumeratorFactory)
        {
            this.parentEnumeratorFactory = parentEnumeratorFactory;
            return this;
        }

        /// <summary>
        /// Customizes the logic used to select elements for convention application.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <returns>The configuration.</returns>
        public PresentationFrameworkConfiguration SelectElementsWith(Func<IConventionManager, DependencyObject, IEnumerable<ElementDescription>> selector)
        {
            ConventionExtensions.SelectElementsToInspect = selector;
            return this;
        }

        /// <summary>
        /// Customize the suffix that is removed from the class name when registering commands by Key with the container.
        /// </summary>
        public PresentationFrameworkConfiguration UseCommandNameSuffix(string suffix)
        {
            CommandAttribute.CommandNameSuffix = suffix;
            return this;
        }

        /// <summary>
        /// Sets the default dependency mode for INPC proxies.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>The configuration.</returns>
        public PresentationFrameworkConfiguration SetDefaultDependencyMode(DependencyMode mode)
        {
            NotifyPropertyChangedAttribute.DefaultDependencyMode = mode;
            return this;
        }

        /// <summary>
        /// Initializes the core.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public override void Initialize(IServiceLocator serviceLocator)
        {
            Execute.Initialize(serviceLocator.GetInstance<IDispatcher>());

            base.Initialize(serviceLocator);

            var controller = serviceLocator.GetInstance<IRoutedMessageController>();
            var messageBinder = serviceLocator.GetInstance<IMessageBinder>();
            var parser = serviceLocator.GetInstance<IParser>();
            var conventionManager = serviceLocator.GetInstance<IConventionManager>();
            var viewModelBinder = serviceLocator.GetInstance<IViewModelBinder>();
            var viewModelDescriptionFactory = serviceLocator.GetInstance<IViewModelDescriptionFactory>();

            parser.RegisterMessageParser("Action", new ActionMessageParser(conventionManager, messageBinder));
            parser.RegisterMessageParser("ResourceCommand", new CommandMessageParser(conventionManager, messageBinder, CommandSource.Resource));
            parser.RegisterMessageParser("ContainerCommand", new CommandMessageParser(conventionManager, messageBinder, CommandSource.Container));
            parser.RegisterMessageParser("BoundCommand", new CommandMessageParser(conventionManager, messageBinder, CommandSource.Bound));

            Message.Initialize(
                controller,
                parser
                );

            Action.Initialize(
                controller,
                viewModelDescriptionFactory,
                serviceLocator
                );

            View.Initialize(
                serviceLocator.GetInstance<IViewLocator>(),
                viewModelBinder
                );

            ScreenExtensions.Initialize(
                serviceLocator.GetInstance<IViewModelFactory>()
                );

            ViewConventionBase.Initialize(
                messageBinder,
                viewModelDescriptionFactory,
                serviceLocator.GetInstance<IValidator>()
                );

            Bind.Initialize(viewModelBinder);
            Coroutine.Initialize(
                serviceLocator,
                serviceLocator.GetInstance<IBuilder>(),
                parentEnumeratorFactory
                );

            if (!registerItemsWithSubjects)
                return;

            var registry = serviceLocator.GetInstance<IRegistry>();
            var assemblySource = serviceLocator.GetInstance<IAssemblySource>();

            assemblySource.Apply(x => RegisterItemsWithSubjects(registry, x));
            assemblySource.AssemblyAdded += assembly => RegisterItemsWithSubjects(registry, assembly);
        }

        void RegisterItemsWithSubjects(IRegistry registry, Assembly assembly)
        {
            var matches = from type in CoreExtensions.GetInspectableTypes(assembly)
                          let service = type.FindInterfaceThatCloses(typeof(IHaveSubject<>))
                          where service != null
                          select new PerRequest
                          {
                              Service = service,
                              Implementation = type,
                              Name = nameInstances ? type.AssemblyQualifiedName : null
                          };

            registry.Register(matches.OfType<IComponentRegistration>());
        }
    }
}
