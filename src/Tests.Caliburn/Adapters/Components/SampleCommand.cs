namespace Tests.Caliburn.Adapters.Components
{
    using System.ComponentModel.Composition;
    public class SampleCommand : ISampleCommand
    {
        [DummyBehavior, Import]
        public ILogger Logger { get; set; }

        public void Execute()
        {
            //do something
        }
    }
}