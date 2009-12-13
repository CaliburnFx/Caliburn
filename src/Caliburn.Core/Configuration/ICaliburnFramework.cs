namespace Caliburn.Core.Configuration
{
    public interface ICaliburnFramework
    {
        void AddModule(IModule module);
        void Start();
    }
}