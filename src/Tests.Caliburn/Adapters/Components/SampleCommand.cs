using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.Caliburn.Adapters.Components
{
	public class SampleCommand: ISampleCommand
	{

		[Microsoft.Practices.Unity.Dependency]
		[System.ComponentModel.Composition.Import]
		[Ninject.Inject]
		[StructureMap.Attributes.SetterProperty]
		[DummyBehavior]
		public ILogger Logger { get; set; }

		
		public void Execute()
		{
			//do something
		}

	}
}
