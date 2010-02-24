namespace Caliburn.ShellFramework.Configuration
{
    using Core.Configuration;
    using Core.IoC;
    using Questions;
    using Resources;
    using Services;

    public interface IShellFrameworkServicesDescription
    {
        IConfiguredRegistration<Singleton, T> BusyService<T>() where T : IBusyService;
        IConfiguredRegistration<Singleton, T> ResourceManager<T>() where T : IResourceManager;
        IConfiguredRegistration<PerRequest, T> QuestionDialog<T>() where T : IQuestionDialog;
    }
}