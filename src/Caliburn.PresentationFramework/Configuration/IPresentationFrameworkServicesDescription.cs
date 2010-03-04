namespace Caliburn.PresentationFramework.Configuration
{
    using Actions;
    using ApplicationModel;
    using Conventions;
    using Core.Configuration;
    using Core.IoC;
    using RoutedMessaging;
    using RoutedMessaging.Parsers;
    using ViewModels;
    using Views;

    /// <summary>
    /// Desscribes the services required for the presentation framework to function.
    /// </summary>
    public interface IPresentationFrameworkServicesDescription
    {
        /// <summary>
        /// Customizes the routed message controller used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The routed message controller type.</typeparam>
        IConfiguredRegistration<Singleton, T> RoutedMessageController<T>() where T : IRoutedMessageController;

        /// <summary>
        /// Customizes the method binder used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The method binder type.</typeparam>
        IConfiguredRegistration<Singleton, T> MessageBinder<T>() where T : IMessageBinder;

        /// <summary>
        /// Customizes the message parser used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The message parser type.</typeparam>
        IConfiguredRegistration<Singleton, T> Parser<T>() where T : IParser;

        /// <summary>
        /// Customizes the view model description builder.
        /// </summary>
        /// <typeparam name="T">The action factory type.</typeparam>
        IConfiguredRegistration<Singleton, T> ViewModelDescriptionFactory<T>() where T : IViewModelDescriptionFactory;

        /// <summary>
        /// Customizes the actions locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IConfiguredRegistration<Singleton, T> ActionLocator<T>() where T : IActionLocator;

        /// <summary>
        /// Customizes the view strategy used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The view strategy type.</typeparam>
        IConfiguredRegistration<Singleton, T> ViewLocator<T>() where T : IViewLocator;

        /// <summary>
        /// Customizes the binder used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The binder type.</typeparam>
        IConfiguredRegistration<Singleton, T> ViewModelBinder<T>() where T : IViewModelBinder;

        /// <summary>
        /// Custmizes the view model factory used by Caliburn.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IConfiguredRegistration<Singleton, T> ViewModelFactory<T>() where T : IViewModelFactory;

        /// <summary>
        /// Customizes the convention manager used by Caliburn.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IConfiguredRegistration<Singleton, T> ConventionManager<T>() where T : IConventionManager;

#if !SILVERLIGHT_20

        /// <summary>
        /// Customizes the validator used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The validator type.</typeparam>
        IConfiguredRegistration<Singleton, T> Validator<T>() where T : IValidator;

        /// <summary>
        /// Customizes the window manager used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The window manager type.</typeparam>
        IConfiguredRegistration<Singleton, T> WindowManager<T>() where T : IWindowManager;

        /// <summary>
        /// Customizes the input manager used by Caliburn.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IConfiguredRegistration<Singleton, T> InputManager<T>() where T : IInputManager;
#endif
    }
}