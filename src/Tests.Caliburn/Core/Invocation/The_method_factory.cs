using Caliburn.Core.Invocation;
using Caliburn.Core.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Tests.Caliburn.Fakes;

namespace Tests.Caliburn.Core.Invocation
{
    [TestFixture]
    public class The_method_factory : TestBase
    {
        private MethodInvokeTarget _theInvokeTarget;
        private IMethodFactory _factory;
        private IThreadPool _threadPool;

        protected override void given_the_context_of()
        {
            _theInvokeTarget = new MethodInvokeTarget();
            MethodInvokeTarget.Reset();

            _threadPool = Mock<IThreadPool>();
            _factory = new MethodFactory(_threadPool);
        }

        [Test]
        public void can_create_an_instance_procedure_using_method_info()
        {
            var methodInfo = typeof(MethodInvokeTarget).GetMethod("AnInstanceProcedure");

            IMethod method = _factory.CreateFrom(methodInfo);

            Assert.That(method, Is.Not.Null);
            Assert.That(method, Is.InstanceOfType(typeof(IMethod)));
            Assert.That(method.Info, Is.EqualTo(methodInfo));

            method.Invoke(_theInvokeTarget, new object[] {});

            Assert.That(_theInvokeTarget.InstanceProcedureWasCalled);

            var task = method.CreateBackgroundTask(_theInvokeTarget, new object[] { });

            Assert.That(task, Is.Not.Null);
            Assert.That(task, Is.InstanceOfType(typeof(IBackgroundTask)));

            Assert.That(method.GetMetadata<FakeMetadata>(), Is.Not.Null);
        }

        [Test]
        public void can_create_an_instance_function_using_method_info()
        {
            var methodInfo = typeof(MethodInvokeTarget).GetMethod("AnInstanceFunction");

            IMethod method = _factory.CreateFrom(methodInfo);

            Assert.That(method, Is.Not.Null);
            Assert.That(method, Is.InstanceOfType(typeof(IMethod)));
            Assert.That(method.Info, Is.EqualTo(methodInfo));

            object result = method.Invoke(_theInvokeTarget, new object[] { });

            Assert.That(_theInvokeTarget.InstanceFunctionWasCalled);
            Assert.That(result, Is.EqualTo(MethodInvokeTarget.ReturnValue));

            var task = method.CreateBackgroundTask(_theInvokeTarget, new object[] { });

            Assert.That(task, Is.Not.Null);
            Assert.That(task, Is.InstanceOfType(typeof(IBackgroundTask)));

            Assert.That(method.GetMetadata<FakeMetadata>(), Is.Not.Null);
        }

        [Test]
        public void can_create_a_static_procedure_using_method_info()
        {
            var methodInfo = typeof(MethodInvokeTarget).GetMethod("AStaticProcedure");

            IMethod method = _factory.CreateFrom(methodInfo);

            Assert.That(method, Is.Not.Null);
            Assert.That(method, Is.InstanceOfType(typeof(IMethod)));
            Assert.That(method.Info, Is.EqualTo(methodInfo));

            method.Invoke(_theInvokeTarget, new object[] { });

            Assert.That(MethodInvokeTarget.StaticProcedureWasCalled);

            var task = method.CreateBackgroundTask(_theInvokeTarget, new object[] { });

            Assert.That(task, Is.Not.Null);
            Assert.That(task, Is.InstanceOfType(typeof(IBackgroundTask)));

            Assert.That(method.GetMetadata<FakeMetadata>(), Is.Not.Null);
        }

        [Test]
        public void can_create_a_static_function_using_method_info()
        {
            var methodInfo = typeof(MethodInvokeTarget).GetMethod("AStaticFunction");

            IMethod method = _factory.CreateFrom(methodInfo);

            Assert.That(method, Is.Not.Null);
            Assert.That(method, Is.InstanceOfType(typeof(IMethod)));
            Assert.That(method.Info, Is.EqualTo(methodInfo));

            object result = method.Invoke(_theInvokeTarget, new object[] { });

            Assert.That(MethodInvokeTarget.StaticFunctionWasCalled);
            Assert.That(result, Is.EqualTo(MethodInvokeTarget.ReturnValue));

            var task = method.CreateBackgroundTask(_theInvokeTarget, new object[] { });

            Assert.That(task, Is.Not.Null);
            Assert.That(task, Is.InstanceOfType(typeof(IBackgroundTask)));

            Assert.That(method.GetMetadata<FakeMetadata>(), Is.Not.Null);
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