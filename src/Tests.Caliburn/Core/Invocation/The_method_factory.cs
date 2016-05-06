using Caliburn.Core.Invocation;
using Shouldly;
using Xunit;
using Tests.Caliburn.Fakes;

namespace Tests.Caliburn.Core.Invocation
{
    
    public class The_method_factory : TestBase
    {
        MethodInvokeTarget theInvokeTarget;
        IMethodFactory factory;

        protected override void given_the_context_of()
        {
            theInvokeTarget = new MethodInvokeTarget();
            MethodInvokeTarget.Reset();

            factory = new DefaultMethodFactory();
        }

        [Fact]
        public void can_create_an_instance_procedure_using_method_info()
        {
            var methodInfo = typeof(MethodInvokeTarget).GetMethod("AnInstanceProcedure");

            IMethod method = factory.CreateFrom(methodInfo);

            method.ShouldNotBeNull();
            method.ShouldBeAssignableTo<IMethod>();
            method.Info.ShouldBe(methodInfo);

            method.Invoke(theInvokeTarget, new object[] {});

            theInvokeTarget.InstanceProcedureWasCalled.ShouldBeTrue();

            var task = method.CreateBackgroundTask(theInvokeTarget, new object[] { });

            task.ShouldNotBeNull();
            task.ShouldBeAssignableTo<IBackgroundTask>();
        }

        [Fact]
        public void can_create_an_instance_function_using_method_info()
        {
            var methodInfo = typeof(MethodInvokeTarget).GetMethod("AnInstanceFunction");

            IMethod method = factory.CreateFrom(methodInfo);

            method.ShouldNotBeNull();
            method.ShouldBeAssignableTo<IMethod>();
            method.Info.ShouldBe(methodInfo);

            object result = method.Invoke(theInvokeTarget, new object[] { });

            theInvokeTarget.InstanceFunctionWasCalled.ShouldBeTrue();
            result.ShouldBe(MethodInvokeTarget.ReturnValue);

            var task = method.CreateBackgroundTask(theInvokeTarget, new object[] { });

            task.ShouldNotBeNull();
            task.ShouldBeAssignableTo<IBackgroundTask>();
        }

        [Fact]
        public void can_create_a_static_procedure_using_method_info()
        {
            var methodInfo = typeof(MethodInvokeTarget).GetMethod("AStaticProcedure");

            IMethod method = factory.CreateFrom(methodInfo);

            method.ShouldNotBeNull();
            method.ShouldBeAssignableTo<IMethod>();
            method.Info.ShouldBe(methodInfo);

            method.Invoke(theInvokeTarget, new object[] { });

            MethodInvokeTarget.StaticProcedureWasCalled.ShouldBeTrue();

            var task = method.CreateBackgroundTask(theInvokeTarget, new object[] { });

            task.ShouldNotBeNull();
            task.ShouldBeAssignableTo<IBackgroundTask>();
        }

        [Fact]
        public void can_create_a_static_function_using_method_info()
        {
            var methodInfo = typeof(MethodInvokeTarget).GetMethod("AStaticFunction");

            IMethod method = factory.CreateFrom(methodInfo);

            method.ShouldNotBeNull();
            method.ShouldBeAssignableTo<IMethod>();
            method.Info.ShouldBe(methodInfo);

            object result = method.Invoke(theInvokeTarget, new object[] { });

            MethodInvokeTarget.StaticFunctionWasCalled.ShouldBeTrue();
            result.ShouldBe(MethodInvokeTarget.ReturnValue);

            var task = method.CreateBackgroundTask(theInvokeTarget, new object[] { });

            task.ShouldNotBeNull();
            task.ShouldBeAssignableTo<IBackgroundTask>();
        }

        [Fact]
        public void caches_method_instances()
        {
            var methodInfo = typeof(MethodInvokeTarget).GetMethod("AnInstanceProcedure");
            IMethod method = factory.CreateFrom(methodInfo);

            var methodInfo2 = typeof(MethodInvokeTarget).GetMethod("AnInstanceProcedure");
            IMethod method2 = factory.CreateFrom(methodInfo2);

            method.ShouldBeSameAs(method2);
        }
    }
}