﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Caliburn.Core.IoC;

namespace Tests.Caliburn.Adapters.Integration
{
	[TestFixture]
	public class The_Simple_Container : ContainerIntegrationTestBase
	{
		protected override global::Caliburn.Core.IoC.IContainer CreateContainerAdapter()
		{
			return new SimpleContainer(true);
		}
	}
}