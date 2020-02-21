using System;

namespace Tests.Caliburn.Core
{
    using Fakes;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Configuration;
    using Xunit;
    using NSubstitute;


    public class The_resolve_markup_extension : TestBase
    {
        IServiceLocator container;

        public The_resolve_markup_extension()
        {
            PresentationFrameworkConfiguration.IsInDesignMode = false;
        }

        protected override void given_the_context_of()
        {
            container = Mock<IServiceLocator>();
            IoC.Initialize(container);
        }

        [Fact]
        public void can_resolve_by_key()
        {
            var extension = new ResolveExtension {
                Key = "theKey"
            };

            extension.ProvideValue(null);

            container.Received().GetInstance(null, "theKey");
        }

        [Fact]
        public void can_resolve_by_type()
        {
            var extension = new ResolveExtension {
                Type = typeof(ITestService)
            };

            extension.ProvideValue(null);

            container.Received().GetInstance(typeof(ITestService));
        }

        [Fact]
        public void can_resolve_by_type_and_key()
        {
            var extension = new ResolveExtension {
                Type = typeof(ITestService),
                Key = "theKey"
            };

            extension.ProvideValue(null);

            container.Received().GetInstance(typeof(ITestService), "theKey");
        }

        protected override void after_each()
        {
            PresentationFrameworkConfiguration.IsInDesignMode = true;
        }
    }
}
