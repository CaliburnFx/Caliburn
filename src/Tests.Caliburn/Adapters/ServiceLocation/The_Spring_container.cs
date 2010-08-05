namespace Tests.Caliburn.Adapters.ServiceLocation
{
    using Components;
    using global::Caliburn.Spring;
    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using Spring.Context.Support;

    [TestFixture]
    public class The_Spring_container : ServiceLocatorTests
    {
        protected override IServiceLocator CreateServiceLocator()
        {
            var context = new GenericApplicationContext(false);

            context.ObjectFactory.RegisterSingleton(typeof(SimpleLogger).FullName, new SimpleLogger());
            context.ObjectFactory.RegisterSingleton(typeof(AdvancedLogger).FullName, new AdvancedLogger());

            return new SpringAdapter(context);
        }
    }
}