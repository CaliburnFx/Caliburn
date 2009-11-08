namespace Tests.Caliburn.Fakes
{
    public class DependentService : IDependentService
    {
        private readonly ITestService _dependency;

        public DependentService() { }

        public DependentService(ITestService dependency)
        {
            _dependency = dependency;
        }

        public ITestService Dependency
        {
            get { return _dependency; }
        }
    }
}