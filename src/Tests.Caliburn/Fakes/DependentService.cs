namespace Tests.Caliburn.Fakes
{
    public class DependentService : IDependentService
    {
        readonly ITestService dependency;

        public DependentService() { }

        public DependentService(ITestService dependency)
        {
            this.dependency = dependency;
        }

        public ITestService Dependency
        {
            get { return dependency; }
        }
    }
}