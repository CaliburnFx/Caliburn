namespace Tests.Caliburn.Fakes
{
    public interface IDependentService
    {
        ITestService Dependency { get; }
    }
}