namespace Tests.Caliburn.Fakes
{
    public class TestService : ITestService
    {
    }

	public class TestService<T> : ITestService<T>
	{
		public T Item { get; set; }
	}
}