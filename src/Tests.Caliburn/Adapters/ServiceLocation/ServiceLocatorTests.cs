namespace Tests.Caliburn.Adapters.ServiceLocation
{
    using System.Collections;
    using System.Collections.Generic;
    using Components;
    using global::Caliburn.Core.InversionOfControl;
    using Xunit;
    using Shouldly;

    public abstract class ServiceLocatorTests
    {
        protected readonly IServiceLocator Locator;

        protected ServiceLocatorTests()
        {
            Locator = CreateServiceLocator();
        }

        protected abstract IServiceLocator CreateServiceLocator();

        [Fact]
        public void GetInstance()
        {
            var instance = Locator.GetInstance<ILogger>();
            instance.ShouldNotBeNull();
        }

        [Fact]
        public void AskingForInvalidComponentShouldReturnNull()
        {
            var result = Locator.GetInstance<IDictionary>();
            result.ShouldBeNull();
        }

        [Fact]
        public void GetNamedInstance()
        {
            var instance = Locator.GetInstance<ILogger>(typeof(AdvancedLogger).FullName);
            instance.ShouldBeOfType<AdvancedLogger>();
        }

        [Fact]
        public void GetNamedInstance2()
        {
            var instance = Locator.GetInstance<ILogger>(typeof(SimpleLogger).FullName);
            instance.ShouldBeOfType<SimpleLogger>();
        }

        [Fact]
        public virtual void GetUnknownInstance2()
        {
            var result = Locator.GetInstance<ILogger>("test");
            result.ShouldBeNull();
        }

        [Fact]
        public virtual void GetAllInstances()
        {
            var instances = Locator.GetAllInstances<ILogger>();
            IList<ILogger> list = new List<ILogger>(instances);
            list.Count.ShouldBe(2);
        }

        [Fact]
        public void GetlAllInstance_ForUnknownType_ReturnEmptyEnumerable()
        {
            var instances = Locator.GetAllInstances<IDictionary>();
            IList<IDictionary> list = new List<IDictionary>(instances);
            list.Count.ShouldBe(0);
        }

        [Fact]
        public void GenericOverload_GetInstance()
        {
            Locator.GetInstance<ILogger>().GetType().ShouldBe(Locator.GetInstance(typeof(ILogger), null).GetType());
        }

        [Fact]
        public void GenericOverload_GetInstance_WithName()
        {
            Locator.GetInstance<ILogger>(typeof(AdvancedLogger).FullName).GetType()
                .ShouldBe(Locator.GetInstance(typeof(ILogger), typeof(AdvancedLogger).FullName).GetType());
        }

        [Fact]
        public void Overload_GetInstance_NoName_And_NullName()
        {
            Locator.GetInstance<ILogger>().GetType()
                .ShouldBe(Locator.GetInstance<ILogger>(null).GetType());
        }

        [Fact]
        public virtual void GenericOverload_GetAllInstances()
        {
            var genericLoggers = new List<ILogger>(Locator.GetAllInstances<ILogger>());
            var plainLoggers = new List<object>(Locator.GetAllInstances(typeof(ILogger)));
            genericLoggers.Count.ShouldBe(plainLoggers.Count);
            for (var i = 0; i < genericLoggers.Count; i++)
            {
                genericLoggers[i].GetType()
                    .ShouldBe(plainLoggers[i].GetType());
            }
        }

        [Fact]
        public void can_resolve_IRegistry()
        {
            var sl = Locator.GetInstance<IRegistry>();
            sl.ShouldBeSameAs(Locator);
        }

        [Fact]
        public void can_resolve_IServiceLocator()
        {
            var sl = Locator.GetInstance<IServiceLocator>();
            sl.ShouldBeSameAs(Locator);
        }

        [Fact]
        public void can_resolve_IContainer()
        {
            var sl = Locator.GetInstance<IContainer>();
            sl.ShouldBeSameAs(Locator);
        }
    }
}