using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Caliburn.Core.InversionOfControl;
using Castle.Windsor;
using Caliburn.Windsor;

namespace Tests.Caliburn.Adapters.Integration
{
	[TestFixture]
	public class The_Windsor_container : ContainerIntegrationTestBase
	{
		protected override global::Caliburn.Core.InversionOfControl.IContainer CreateContainerAdapter()
		{
			IWindsorContainer container = new WindsorContainer();
			return new WindsorAdapter(container);
		}
	}
}
