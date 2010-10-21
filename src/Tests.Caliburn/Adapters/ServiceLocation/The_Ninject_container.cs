namespace Tests.Caliburn.Adapters.ServiceLocation
{
    using Components;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.Ninject;
    using Ninject;
    using NUnit.Framework;

    [TestFixture]
    public class The_Ninject_container : ServiceLocatorTests
    {
        protected override IServiceLocator CreateServiceLocator()
        {
            var kernel = new StandardKernel();

            kernel.Bind<ILogger>().To<AdvancedLogger>();
            kernel.Bind<ILogger>().To<SimpleLogger>().Named(typeof(SimpleLogger).FullName);
            kernel.Bind<ILogger>().To<AdvancedLogger>().Named(typeof(AdvancedLogger).FullName);

            return new NinjectAdapter(kernel);
        }

        public override void GetAllInstances() {}
    }
}