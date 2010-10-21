namespace Tests.Caliburn.Adapters.ServiceLocation
{
    using Components;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.Unity;
    using Microsoft.Practices.Unity;
    using NUnit.Framework;

    [TestFixture]
    public class The_Unity_container : ServiceLocatorTests
    {
        protected override IServiceLocator CreateServiceLocator()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, AdvancedLogger>()
                .RegisterType<ILogger, SimpleLogger>(typeof(SimpleLogger).FullName)
                .RegisterType<ILogger, AdvancedLogger>(typeof(AdvancedLogger).FullName);

            return new UnityAdapter(container);
        }

        [Test]
        public void UnityAdapter_Get_WithZeroLenName_ReturnsDefaultInstance()
        {
            Assert.AreSame(
                Locator.GetInstance<ILogger>().GetType(),
                Locator.GetInstance<ILogger>("").GetType()
                );
        }
    }
}