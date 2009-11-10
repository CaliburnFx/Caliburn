using Caliburn.Core;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Tests.Caliburn.Core
{
    using global::Caliburn.Core.IoC;

    [TestFixture]
    public class The_CaliburnApplication_object : TestBase
    {
        [Test]
        public void configures_the_DI_container()
        {
            var container = new SimpleContainer();

            CaliburnFramework.ConfigureCore(container);

            Assert.That(ServiceLocator.Current, Is.EqualTo(container));
        }

        [Test]
        public void configures_the_SimpleContainer_by_default()
        {
            CaliburnFramework.ConfigureCore();

            Assert.That(ServiceLocator.Current, Is.Not.Null);
            Assert.That(ServiceLocator.Current, Is.InstanceOfType(typeof(SimpleContainer)));
        }

        [Test]
        public void prepares_a_configuration_hook()
        {
            var hook = CaliburnFramework.ConfigureCore();

            Assert.That(hook, Is.Not.Null);
            Assert.That(hook, Is.InstanceOfType(typeof(CoreConfiguration)));
            Assert.That(hook, Is.InstanceOfType(typeof(IConfigurationHook)));
            Assert.That(hook.ServiceLocator, Is.Not.Null);
        }
    }
}