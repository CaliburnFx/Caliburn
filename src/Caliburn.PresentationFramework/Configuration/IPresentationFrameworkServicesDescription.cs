namespace Caliburn.PresentationFramework.Configuration
{
    using Actions;
    using ApplicationModel;
    using Conventions;
    using Core.IoC;
    using PresentationFramework;
    using Parsers;
    using ViewModels;

    public interface IPresentationFrameworkServicesDescription
    {
        /// <summary>
        /// Customizes the routed message controller used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The routed message controller type.</typeparam>
        Singleton RoutedMessageController<T>() where T : IRoutedMessageController;

        /// <summary>
        /// Customizes the method binder used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The method binder type.</typeparam>
        Singleton MessageBinder<T>() where T : IMessageBinder;

        /// <summary>
        /// Customizes the message parser used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The message parser type.</typeparam>
        Singleton Parser<T>() where T : IParser;

        /// <summary>
        /// Customizes the view model description builder.
        /// </summary>
        /// <typeparam name="T">The action factory type.</typeparam>
        Singleton ViewModelDescriptionFactory<T>() where T : IViewModelDescriptionFactory;

        /// <summary>
        /// Customizes the actions locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Singleton ActionLocator<T>() where T : IActionLocator;

        /// <summary>
        /// Customizes the view strategy used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The view strategy type.</typeparam>
        Singleton ViewLocator<T>() where T : IViewLocator;

        /// <summary>
        /// Customizes the binder used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The binder type.</typeparam>
        Singleton ViewModelBinder<T>() where T : IViewModelBinder;

        /// <summary>
        /// Custmizes the view model factory used by Caliburn.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Singleton ViewModelFactory<T>() where T : IViewModelFactory;

#if !SILVERLIGHT_20

        /// <summary>
        /// Customizes the validator used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The validator type.</typeparam>
        Singleton Validator<T>() where T : IValidator;
#endif

        /// <summary>
        /// Customizes the convention manager used by Caliburn.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Singleton ConventionManager<T>() where T : IConventionManager;

#if !SILVERLIGHT_20

        /// <summary>
        /// Customizes the window manager used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The window manager type.</typeparam>
        Singleton WindowManager<T>() where T : IWindowManager;
#endif
    }
}