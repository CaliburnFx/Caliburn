using System.Collections;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Tests.Caliburn.Adapters.Components;

namespace Tests.Caliburn.Adapters.ServiceLocation
{
    using global::Caliburn.Core.IoC;
    using NUnit.Framework.SyntaxHelpers;

    public abstract class ServiceLocatorTests
    {
        protected readonly IServiceLocator locator;

        protected ServiceLocatorTests()
        {
            locator = CreateServiceLocator();
        }

        protected abstract IServiceLocator CreateServiceLocator();

        [Test]
        public void GetInstance()
        {
            ILogger instance = locator.GetInstance<ILogger>();
            Assert.IsNotNull(instance);
        }

        [Test]
        [ExpectedException(typeof(ActivationException))]
        public void AskingForInvalidComponentShouldRaiseActivationException()
        {
            locator.GetInstance<IDictionary>();
        }

        [Test]
        public void GetNamedInstance()
        {
            ILogger instance = locator.GetInstance<ILogger>(typeof(AdvancedLogger).FullName);
            Assert.AreSame(instance.GetType(), typeof(AdvancedLogger));
        }

        [Test]
        public void GetNamedInstance2()
        {
            ILogger instance = locator.GetInstance<ILogger>(typeof(SimpleLogger).FullName);
            Assert.AreSame(instance.GetType(), typeof(SimpleLogger));
        }

        [Test]
        [ExpectedException(typeof(ActivationException))]
        public virtual void GetUnknownInstance2()
        {
            locator.GetInstance<ILogger>("test");
        }

        [Test]
        public virtual void GetAllInstances()
        {
            IEnumerable<ILogger> instances = locator.GetAllInstances<ILogger>();
            IList<ILogger> list = new List<ILogger>(instances);
            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void GetlAllInstance_ForUnknownType_ReturnEmptyEnumerable()
        {
            IEnumerable<System.Collections.IDictionary> instances = locator.GetAllInstances<IDictionary>();
            IList<IDictionary> list = new List<IDictionary>(instances);
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void GenericOverload_GetInstance()
        {
            Assert.AreEqual(
                locator.GetInstance<ILogger>().GetType(),
                locator.GetInstance(typeof(ILogger), null).GetType());
        }

        [Test]
        public void GenericOverload_GetInstance_WithName()
        {
            Assert.AreEqual(
                locator.GetInstance<ILogger>(typeof(AdvancedLogger).FullName).GetType(),
                locator.GetInstance(typeof(ILogger), typeof(AdvancedLogger).FullName).GetType()
                );
        }

        [Test]
        public void Overload_GetInstance_NoName_And_NullName()
        {
            Assert.AreEqual(
                locator.GetInstance<ILogger>().GetType(),
                locator.GetInstance<ILogger>(null).GetType());
        }

        [Test]
        public virtual void GenericOverload_GetAllInstances()
        {
            List<ILogger> genericLoggers = new List<ILogger>(locator.GetAllInstances<ILogger>());
            List<object> plainLoggers = new List<object>(locator.GetAllInstances(typeof(ILogger)));
            Assert.AreEqual(genericLoggers.Count, plainLoggers.Count);
            for (int i = 0; i < genericLoggers.Count; i++)
            {
                Assert.AreEqual(
                    genericLoggers[i].GetType(),
                    plainLoggers[i].GetType());
            }
        }

        [Test]
        public void can_resolve_IRegistry()
        {
            var sl = locator.GetInstance<IRegistry>();
            Assert.That(sl, Is.EqualTo(locator));
        }

        [Test]
        public void can_resolve_IServiceLocator()
        {
            var sl = locator.GetInstance<IServiceLocator>();
            Assert.That(sl, Is.EqualTo(locator));
        }

        [Test]
        public void can_resolve_IContainer()
        {
            var sl = locator.GetInstance<IContainer>();
            Assert.That(sl, Is.EqualTo(locator));
        }
    }
}