using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Caliburn.Autofac;
using Autofac;

namespace Tests.Caliburn.Adapters.Integration
{
	[TestFixture]
	public class The_Autofac_container : ContainerIntegrationTestBase
	{
		protected override global::Caliburn.Core.IoC.IContainer CreateContainerAdapter()
		{
			var builder = new ContainerBuilder();
			var container = builder.Build();
			return new AutofacAdapter(container);
		}

		[Test]
		[Ignore]
		public override void can_inject_dependencies_on_public_properties() { }
	}
}
