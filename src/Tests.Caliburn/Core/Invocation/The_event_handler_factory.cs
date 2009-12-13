using Caliburn.Core.Invocation;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Tests.Caliburn.Fakes;

namespace Tests.Caliburn.Core.Invocation
{
    [TestFixture]
    public class The_event_handler_factory : TestBase
    {
        private MethodInvokeTarget _theInvokeTarget;
        private IEventHandlerFactory _factory;

        protected override void given_the_context_of()
        {
            _factory = new DefaultEventHandlerFactory();
            _theInvokeTarget = new MethodInvokeTarget();
            MethodInvokeTarget.Reset();
        }

        [Test]
        public void can_wire_an_event_from_event_info()
        {
            var info = typeof(MethodInvokeTarget).GetEvent("AnEvent");
            var handler = _factory.Wire(_theInvokeTarget, info);

            Assert.That(handler, Is.Not.Null);
            Assert.That(handler, Is.InstanceOfType(typeof(IEventHandler)));

            bool theHandlerWasCalled = false;
            handler.SetActualHandler(delegate { theHandlerWasCalled = true; });

            _theInvokeTarget.FireAnEvent();

            Assert.That(theHandlerWasCalled);
        }

        [Test]
        public void can_wire_an_event_based_on_name()
        {
            var handler = _factory.Wire(_theInvokeTarget, "AnEvent");

            Assert.That(handler, Is.Not.Null);
            Assert.That(handler, Is.InstanceOfType(typeof(IEventHandler)));

            bool theHandlerWasCalled = false;
            handler.SetActualHandler(delegate { theHandlerWasCalled = true; });

            _theInvokeTarget.FireAnEvent();

            Assert.That(theHandlerWasCalled);
        }
    }
}