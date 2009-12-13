namespace Caliburn.PresentationFramework.Configuration
{
    using Actions;
    using ApplicationModel;
    using Core.IoC;
    using PresentationFramework;
    using Parsers;

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
        /// Customizes the action factory used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The action factory type.</typeparam>
        Singleton ActionFactory<T>() where T : IActionFactory;

        /// <summary>
        /// Customizes the view strategy used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The view strategy type.</typeparam>
        Singleton ViewStrategy<T>() where T : IViewStrategy;

        /// <summary>
        /// Customizes the binder used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The binder type.</typeparam>
        Singleton Binder<T>() where T : IBinder;

#if !SILVERLIGHT

        /// <summary>
        /// Customizes the window manager used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The window manager type.</typeparam>
        Singleton WindowManager<T>() where T : IWindowManager;
#endif
    }
}