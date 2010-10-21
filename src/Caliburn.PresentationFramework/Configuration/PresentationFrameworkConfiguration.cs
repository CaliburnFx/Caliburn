namespace Caliburn.PresentationFramework.Configuration
{
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using Actions;
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

#if SILVERLIGHT
    using System.Windows.Controls;
#endif

    /// <summary>
    /// The presenation framework module.
    /// </summary>
    public class PresentationFrameworkConfiguration :
        ConventionalModule<PresentationFrameworkConfiguration, IPresentationFrameworkServicesDescription>
    {
        private bool registerItemsWithSubjects;
        private bool nameInstances;

        private static bool? isInDesignMode;

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
                    if (Application.Current != null)
                    {
                        var app = Application.Current.ToString();

                        if(app == "System.Windows.Application" || app == "Microsoft.Expression.Blend.BlendApplication")
                            isInDesignMode = true;
                        else isInDesignMode = false;
                    }
                    else isInDesignMode = true;
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
            EnumerableResults.Initialize(serviceLocator);

            if (!registerItemsWithSubjects)
                return;

            var registry = serviceLocator.GetInstance<IRegistry>();
            var assemblySource = serviceLocator.GetInstance<IAssemblySource>();

            assemblySource.Apply(x => RegisterItemsWithSubjects(registry, x));
            assemblySource.AssemblyAdded += assembly => RegisterItemsWithSubjects(registry, assembly);
        }

        private void RegisterItemsWithSubjects(IRegistry registry, Assembly assembly)
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