namespace Tests.Caliburn.Adapters.Components
{
    using System.ComponentModel.Composition;
    using Microsoft.Practices.Unity;
    using Ninject;
    using StructureMap.Attributes;

    public class SampleCommand : ISampleCommand
    {
        [Dependency, Inject, SetterProperty, DummyBehavior, Import]
        public ILogger Logger { get; set; }

        public void Execute()
        {
            //do something
        }
    }
}