using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;
using Tests.Caliburn.Fakes;

namespace Tests.Caliburn.Core
{
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Configuration;

    [TestFixture]
    public class The_resolve_markup_extension : TestBase
    {
        private IServiceLocator _container;

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
            _container = Mock<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => _container);
        }

        [Test]
        public void can_resolve_by_type()
        {
            var extension = new ResolveExtension {Type = typeof(ITestService)};

            extension.ProvideValue(null);

            _container.AssertWasCalled(x => x.GetInstance(typeof(ITestService)));
        }

        [Test]
        public void can_resolve_by_key()
        {
            var extension = new ResolveExtension { Key = "theKey" };

            extension.ProvideValue(null);

            _container.AssertWasCalled(x => x.GetInstance(null, "theKey"));
        }

        [Test]
        public void can_resolve_by_type_and_key()
        {
            var extension = new ResolveExtension
                            {
                                Type = typeof(ITestService),
                                Key = "theKey"
                            };

            extension.ProvideValue(null);

            _container.AssertWasCalled(x => x.GetInstance(typeof(ITestService), "theKey"));
        }
    }
}