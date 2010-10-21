namespace Tests.Caliburn.Adapters.ServiceLocation
{
    using System.Collections;
    using System.Collections.Generic;
    using Components;
    using global::Caliburn.Core.InversionOfControl;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    public abstract class ServiceLocatorTests
    {
        protected readonly IServiceLocator Locator;

        protected ServiceLocatorTests()
        {
            Locator = CreateServiceLocator();
        }

        protected abstract IServiceLocator CreateServiceLocator();

        [Test]
        public void GetInstance()
        {
            var instance = Locator.GetInstance<ILogger>();
            Assert.IsNotNull(instance);
        }

        [Test]
        public void AskingForInvalidComponentShouldReturnNull()
        {
            var result = Locator.GetInstance<IDictionary>();
            Assert.IsNull(result);
        }

        [Test]
        public void GetNamedInstance()
        {
            var instance = Locator.GetInstance<ILogger>(typeof(AdvancedLogger).FullName);
            Assert.AreSame(instance.GetType(), typeof(AdvancedLogger));
        }

        [Test]
        public void GetNamedInstance2()
        {
            var instance = Locator.GetInstance<ILogger>(typeof(SimpleLogger).FullName);
            Assert.AreSame(instance.GetType(), typeof(SimpleLogger));
        }

        [Test]
        public virtual void GetUnknownInstance2()
        {
            var result = Locator.GetInstance<ILogger>("test");
            Assert.IsNull(result);
        }

        [Test]
        public virtual void GetAllInstances()
        {
            var instances = Locator.GetAllInstances<ILogger>();
            IList<ILogger> list = new List<ILogger>(instances);
            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void GetlAllInstance_ForUnknownType_ReturnEmptyEnumerable()
        {
            var instances = Locator.GetAllInstances<IDictionary>();
            IList<IDictionary> list = new List<IDictionary>(instances);
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void GenericOverload_GetInstance()
        {
            Assert.AreEqual(
                Locator.GetInstance<ILogger>().GetType(),
                Locator.GetInstance(typeof(ILogger), null).GetType());
        }

        [Test]
        public void GenericOverload_GetInstance_WithName()
        {
            Assert.AreEqual(
                Locator.GetInstance<ILogger>(typeof(AdvancedLogger).FullName).GetType(),
                Locator.GetInstance(typeof(ILogger), typeof(AdvancedLogger).FullName).GetType()
                );
        }

        [Test]
        public void Overload_GetInstance_NoName_And_NullName()
        {
            Assert.AreEqual(
                Locator.GetInstance<ILogger>().GetType(),
                Locator.GetInstance<ILogger>(null).GetType());
        }

        [Test]
        public virtual void GenericOverload_GetAllInstances()
        {
            var genericLoggers = new List<ILogger>(Locator.GetAllInstances<ILogger>());
            var plainLoggers = new List<object>(Locator.GetAllInstances(typeof(ILogger)));
            Assert.AreEqual(genericLoggers.Count, plainLoggers.Count);
            for(var i = 0; i < genericLoggers.Count; i++)
            {
                Assert.AreEqual(
                    genericLoggers[i].GetType(),
                    plainLoggers[i].GetType());
            }
        }

        [Test]
        public void can_resolve_IRegistry()
        {
            var sl = Locator.GetInstance<IRegistry>();
            Assert.That(sl, Is.EqualTo(Locator));
        }

        [Test]
        public void can_resolve_IServiceLocator()
        {
            var sl = Locator.GetInstance<IServiceLocator>();
            Assert.That(sl, Is.EqualTo(Locator));
        }

        [Test]
        public void can_resolve_IContainer()
        {
            var sl = Locator.GetInstance<IContainer>();
            Assert.That(sl, Is.EqualTo(Locator));
        }
    }
}