namespace Tests.Caliburn.Adapters.Components
{
    using System;
    using System.Collections.Generic;
    using global::Caliburn.Core.Behaviors;

    public class DummyBehaviorAttribute : Attribute, IBehavior
    {
        public IEnumerable<Type> GetInterfaces(Type implementation)
        {
            yield break;
        }
    }
}