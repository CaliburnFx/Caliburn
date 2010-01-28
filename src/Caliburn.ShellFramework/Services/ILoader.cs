namespace Caliburn.ShellFramework.Services
{
    public interface ILoader
    {
        void StartLoading(object viewModel, string message);
        void StopLoading(object viewModel);
    }
}