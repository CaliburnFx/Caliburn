namespace Caliburn.Core.Configuration
{
    public interface IConfigurationBuilder
    {
        IModuleHook With { get; }
        void Start();
    }
}