using Shouldly;

namespace Tests.Caliburn.Core
{
    using System;
    using System.Linq;
    using System.Reflection;
    using global::Caliburn.Core;
    using global::Caliburn.Core.Configuration;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.Core.InversionOfControl;
    using Xunit;

    
    public class The_core_configuration : TestBase
    {
        CoreConfiguration config;
        IModule module;

        protected override void given_the_context_of()
        {
            config = ConventionalModule<CoreConfiguration, ICoreServicesDescription>.Instance;
            module = config;
        }

        [Fact]
        public void when_started_configures_required_components_and_children()
        {
            var registrations = module.GetComponents();

            var found = (from reg in registrations.OfType<Singleton>()
                         where reg.Service == typeof(IMethodFactory)
                         select reg).FirstOrDefault();

            found.ShouldNotBeNull();
            found.Implementation.ShouldNotBeNull();

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IAssemblySource)
                     select reg).FirstOrDefault();

            found.ShouldNotBeNull();
            found.Implementation.ShouldNotBeNull();
        }

        [Fact]
        public void can_provide_a_custom_method_factory()
        {
            config.Using(x => x.MethodFactory<FakeMethodFactory>());

            var registrations = module.GetComponents();

            var found = (from reg in registrations.OfType<Singleton>()
                         where reg.Service == typeof(IMethodFactory)
                         select reg).FirstOrDefault();

            found.Implementation.ShouldBe(typeof(FakeMethodFactory));
        }

        class FakeMethodFactory : IMethodFactory
        {
            public IMethod CreateFrom(MethodInfo methodInfo)
            {
                throw new NotImplementedException();
            }
        }
    }
}