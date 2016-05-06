namespace Caliburn.ShellFramework.Configuration
{
    using Core.Configuration;
    using Core.InversionOfControl;
    using Questions;
    using Resources;
    using Services;

    /// <summary>
    /// Describes the services required for the shell framework to function.
    /// </summary>
    public interface IShellFrameworkServicesDescription
    {
        /// <summary>
        /// Customizes the busy service.
        /// </summary>
        /// <typeparam name="T">The busy service type.</typeparam>
        /// <returns>The configuration.</returns>
        IConfiguredRegistration<Singleton, T> BusyService<T>() where T : IBusyService;

        /// <summary>
        /// Customizes the resource manager.
        /// </summary>
        /// <typeparam name="T">The resource manager type.</typeparam>
        /// <returns>The configuration.</returns>
        IConfiguredRegistration<Singleton, T> ResourceManager<T>() where T : IResourceManager;

        /// <summary>
        /// Customizes the question dialog.
        /// </summary>
        /// <typeparam name="T">The question dialog type.</typeparam>
        /// <returns>The configuration.</returns>
        IConfiguredRegistration<PerRequest, T> QuestionDialog<T>() where T : IQuestionDialog;
    }
}