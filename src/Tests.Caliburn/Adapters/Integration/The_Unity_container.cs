using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using Caliburn.Unity;


namespace Tests.Caliburn.Adapters.Integration
{
	[TestFixture]
	public class The_Unity_container : ContainerIntegrationTestBase
	{
		protected override global::Caliburn.Core.InversionOfControl.IContainer CreateContainerAdapter()
		{
			IUnityContainer container = new UnityContainer();
			return new UnityAdapter(container);
		}
	}
}
