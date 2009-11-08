using Caliburn.Unity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Tests.Caliburn.Adapters.Components;

namespace Tests.Caliburn.Adapters
{
    [TestFixture]
    public class The_Unity_container : ServiceLocatorTests
    {
        protected override IServiceLocator CreateServiceLocator()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<ILogger, AdvancedLogger>()
                .RegisterType<ILogger, SimpleLogger>(typeof(SimpleLogger).FullName)
                .RegisterType<ILogger, AdvancedLogger>(typeof(AdvancedLogger).FullName);

            return new UnityAdapter(container);
        }

        [Test]
        public void UnityAdapter_Get_WithZeroLenName_ReturnsDefaultInstance()
        {
            Assert.AreSame(
                locator.GetInstance<ILogger>().GetType(),
                locator.GetInstance<ILogger>("").GetType()
                );
        }
    }
}