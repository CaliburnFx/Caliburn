using Caliburn.Core.Invocation;
using NUnit.Framework;
using Tests.Caliburn.Fakes;

namespace Tests.Caliburn.Core.Invocation
{
    [TestFixture]
    public class The_method_factory : TestBase
    {
        private MethodInvokeTarget _theInvokeTarget;
        private IMethodFactory _factory;

        protected override void given_the_context_of()
        {
            _theInvokeTarget = new MethodInvokeTarget();
            MethodInvokeTarget.Reset();

            _factory = new DefaultMethodFactory();
        }

        [Test]
        public void can_create_an_instance_procedure_using_method_info()
        {
            var methodInfo = typeof(MethodInvokeTarget).GetMethod("AnInstanceProcedure");

            IMethod method = _factory.CreateFrom(methodInfo);

            Assert.That(method, Is.Not.Null);
            Assert.That(method, Is.InstanceOf<IMethod>());
            Assert.That(method.Info, Is.EqualTo(methodInfo));

            method.Invoke(_theInvokeTarget, new object[] {});

            Assert.That(_theInvokeTarget.InstanceProcedureWasCalled);

            var task = method.CreateBackgroundTask(_theInvokeTarget, new object[] { });

            Assert.That(task, Is.Not.Null);
            Assert.That(task, Is.InstanceOf<IBackgroundTask>());
        }

        [Test]
        public void can_create_an_instance_function_using_method_info()
        {
            var methodInfo = typeof(MethodInvokeTarget).GetMethod("AnInstanceFunction");

            IMethod method = _factory.CreateFrom(methodInfo);

            Assert.That(method, Is.Not.Null);
            Assert.That(method, Is.InstanceOf<IMethod>());
            Assert.That(method.Info, Is.EqualTo(methodInfo));

            object result = method.Invoke(_theInvokeTarget, new object[] { });

            Assert.That(_theInvokeTarget.InstanceFunctionWasCalled);
            Assert.That(result, Is.EqualTo(MethodInvokeTarget.ReturnValue));

            var task = method.CreateBackgroundTask(_theInvokeTarget, new object[] { });

            Assert.That(task, Is.Not.Null);
            Assert.That(task, Is.InstanceOf<IBackgroundTask>());
        }

        [Test]
        public void can_create_a_static_procedure_using_method_info()
        {
            var methodInfo = typeof(MethodInvokeTarget).GetMethod("AStaticProcedure");

            IMethod method = _factory.CreateFrom(methodInfo);

            Assert.That(method, Is.Not.Null);
            Assert.That(method, Is.InstanceOf<IMethod>());
            Assert.That(method.Info, Is.EqualTo(methodInfo));

            method.Invoke(_theInvokeTarget, new object[] { });

            Assert.That(MethodInvokeTarget.StaticProcedureWasCalled);

            var task = method.CreateBackgroundTask(_theInvokeTarget, new object[] { });

            Assert.That(task, Is.Not.Null);
            Assert.That(task, Is.InstanceOf<IBackgroundTask>());
        }

        [Test]
        public void can_create_a_static_function_using_method_info()
        {
            var methodInfo = typeof(MethodInvokeTarget).GetMethod("AStaticFunction");

            IMethod method = _factory.CreateFrom(methodInfo);

            Assert.That(method, Is.Not.Null);
            Assert.That(method, Is.InstanceOf<IMethod>());
            Assert.That(method.Info, Is.EqualTo(methodInfo));

            object result = method.Invoke(_theInvokeTarget, new object[] { });

            Assert.That(MethodInvokeTarget.StaticFunctionWasCalled);
            Assert.That(result, Is.EqualTo(MethodInvokeTarget.ReturnValue));

            var task = method.CreateBackgroundTask(_theInvokeTarget, new object[] { });

            Assert.That(task, Is.Not.Null);
            Assert.That(task, Is.InstanceOf<IBackgroundTask>());
        }

        [Test]
        public void caches_method_instances()
        {
            var methodInfo = typeof(MethodInvokeTarget).GetMethod("AnInstanceProcedure");
            IMethod method = _factory.CreateFrom(methodInfo);

            var methodInfo2 = typeof(MethodInvokeTarget).GetMethod("AnInstanceProcedure");
            IMethod method2 = _factory.CreateFrom(methodInfo2);

            Assert.That(method, Is.SameAs(method2));
        }
    }
}