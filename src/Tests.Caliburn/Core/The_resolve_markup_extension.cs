namespace Tests.Caliburn.Core
{
    using Fakes;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Configuration;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class The_resolve_markup_extension : TestBase
    {
        IServiceLocator container;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            PresentationFrameworkConfiguration.IsInDesignMode = false;
        }

        [TestFixtureTearDown]
        public void TearDownFixture()
        {
            PresentationFrameworkConfiguration.IsInDesignMode = true;
        }

        protected override void given_the_context_of()
        {
            container = Mock<IServiceLocator>();
            IoC.Initialize(container);
        }

        [Test]
        public void can_resolve_by_key()
        {
            var extension = new ResolveExtension {
                Key = "theKey"
            };

            extension.ProvideValue(null);

            container.AssertWasCalled(x => x.GetInstance(null, "theKey"));
        }

        [Test]
        public void can_resolve_by_type()
        {
            var extension = new ResolveExtension {
                Type = typeof(ITestService)
            };

            extension.ProvideValue(null);

            container.AssertWasCalled(x => x.GetInstance(typeof(ITestService)));
        }

        [Test]
        public void can_resolve_by_type_and_key()
        {
            var extension = new ResolveExtension {
                Type = typeof(ITestService),
                Key = "theKey"
            };

            extension.ProvideValue(null);

            container.AssertWasCalled(x => x.GetInstance(typeof(ITestService), "theKey"));
        }
    }
}