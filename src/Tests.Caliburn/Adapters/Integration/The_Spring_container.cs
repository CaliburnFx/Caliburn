using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Spring.Context.Support;
using Caliburn.Spring;


namespace Tests.Caliburn.Adapters.Integration
{
	[TestFixture]
	public class The_Spring_container : ContainerIntegrationTestBase
	{
		protected override global::Caliburn.Core.InversionOfControl.IContainer CreateContainerAdapter()
		{
			var context = new GenericApplicationContext(false);
			return new SpringAdapter(context, Spring.Objects.Factory.Config.AutoWiringMode.AutoDetect);
		}
	}
}
