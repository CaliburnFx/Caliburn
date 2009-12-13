namespace Tests.Caliburn.Core
{
    using global::Caliburn.Core.Configuration;
    using global::Caliburn.Core.IoC;
    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class The_CaliburnApplication_object : TestBase
    {
        [Test]
        public void configures_the_DI_container()
        {
            var container = new SimpleContainer();

            CaliburnFramework.Configure(container);

            Assert.That(ServiceLocator.Current, Is.EqualTo(container));
        }

        [Test]
        public void configures_the_SimpleContainer_by_default()
        {
            CaliburnFramework.Configure();

            Assert.That(ServiceLocator.Current, Is.Not.Null);
            Assert.That(ServiceLocator.Current, Is.InstanceOfType(typeof(SimpleContainer)));
        }
    }
}