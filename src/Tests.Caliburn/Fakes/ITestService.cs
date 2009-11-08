namespace Tests.Caliburn.Fakes
{
    public interface ITestService
    {
    }

	public interface ITestService<T>
	{
		T Item { get; set; }
	}
}