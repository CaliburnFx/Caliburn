namespace Tests.Caliburn.Adapters.Components
{
    public class SampleCommand : ISampleCommand
    {
        public ILogger Logger { get; set; }

        public void Execute()
        {
            //do something
        }
    }
}