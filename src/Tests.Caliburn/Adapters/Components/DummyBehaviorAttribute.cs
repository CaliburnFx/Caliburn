using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Core.Behaviors;

namespace Tests.Caliburn.Adapters.Components
{
	public class DummyBehaviorAttribute: Attribute, IBehavior
	{
		public IEnumerable<Type> GetInterfaces(Type implementation)
		{
			yield break;
		}
	}
}
