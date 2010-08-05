using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap;
using Caliburn.StructureMap;
 

namespace Tests.Caliburn.Adapters.Integration
{
	[TestFixture]
	public class The_StructureMap_container : ContainerIntegrationTestBase
	{
		protected override global::Caliburn.Core.IoC.IContainer CreateContainerAdapter()
		{
			var registry = new Registry();
			IContainer container = new Container(registry);

			return new StructureMapAdapter(container);
		}
	}
}
