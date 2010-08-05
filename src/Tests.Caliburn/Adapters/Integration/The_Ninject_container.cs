using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Caliburn.Ninject;


namespace Tests.Caliburn.Adapters.Integration
{
	[TestFixture]
	public class The_Ninject_container : ContainerIntegrationTestBase
	{
		protected override global::Caliburn.Core.IoC.IContainer CreateContainerAdapter()
		{
			var kernel = new Ninject.StandardKernel();
			return new NinjectAdapter(kernel);
		}
	}
}
