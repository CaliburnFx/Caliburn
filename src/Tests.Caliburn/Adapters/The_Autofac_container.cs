namespace Tests.Caliburn.Adapters
{
    using Autofac.Builder;
    using Components;
    using global::Caliburn.Autofac;
    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;

    [TestFixture]
    public class The_Autofac_container : ServiceLocatorTests
    {
        protected override IServiceLocator CreateServiceLocator()
        {
            var container = new Autofac.Container();

            var builder = new ContainerBuilder();

            builder.Register<AdvancedLogger>().As<ILogger>();
            builder.Register<SimpleLogger>().As<ILogger>().Named(typeof(SimpleLogger).FullName);
            builder.Register<AdvancedLogger>().As<ILogger>().Named(typeof(AdvancedLogger).FullName);

            builder.Build(container);

            return new AutofacAdapter(container);
        }

        public override void GetAllInstances()
        {
        }
    }
}