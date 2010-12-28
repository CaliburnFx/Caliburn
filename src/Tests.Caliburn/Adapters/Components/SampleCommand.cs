using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.Caliburn.Adapters.Components
{
	using System.ComponentModel.Composition;

	public class SampleCommand: ISampleCommand
	{

		[Microsoft.Practices.Unity.Dependency]
		[Ninject.Inject]
		[StructureMap.Attributes.SetterProperty]
		[DummyBehavior]
		[Import]
		public ILogger Logger { get; set; }

		
		public void Execute()
		{
			//do something
		}

	}
}
