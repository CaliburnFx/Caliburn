using System.Threading.Tasks;
using Shouldly;

namespace Tests.Caliburn.RoutedUIMessaging
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Fakes.UI;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.PresentationFramework.Conventions;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using Xunit;
    using NSubstitute;


    public class When_binding_return_values : TestBase
    {
        DefaultMethodFactory factory;
        DefaultMessageBinder binder;
        IInteractionNode handlingNode;
        IInteractionNode sourceNode;
        ControlHost host;
        IConventionManager conventionManager;

        protected override void given_the_context_of()
        {
            factory = new DefaultMethodFactory();
            conventionManager = Mock<IConventionManager>();
            binder = new DefaultMessageBinder(conventionManager);
            handlingNode = Mock<IInteractionNode>();
            sourceNode = Mock<IInteractionNode>();
            host = new ControlHost();

            sourceNode.UIElement.Returns(host);
        }

        public class FakeMessage : IRoutedMessageWithOutcome
        {
            IMethod _method;
            string _outcomePath;
            IInteractionNode _source;

            public FakeMessage(IInteractionNode source, IMethod method, string returnPath)
            {
                _source = source;
                _method = method;
                _outcomePath = returnPath;
            }

            public bool Equals(IRoutedMessage other)
            {
                throw new NotImplementedException();
            }

            public IAvailabilityEffect AvailabilityEffect
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public IInteractionNode Source
            {
                get { return _source; }
            }

            public FreezableCollection<Parameter> Parameters
            {
                get { throw new NotImplementedException(); }
            }

            public void Initialize(IInteractionNode node)
            {
                throw new NotImplementedException();
            }

            public bool RelatesTo(object potentialTarget)
            {
                return true;
            }

            public event Action Invalidated = delegate { };

            public string OutcomePath
            {
                get { return _outcomePath; }
            }

            public string DefaultOutcomeElement
            {
                get { return _method.Info.Name + "Result"; }
            }

            public IEnumerable<IRoutedMessageHandler> GetDefaultHandlers(IInteractionNode node)
            {
                yield break;
            }
        }

        public class MethodHost
        {
            public void MethodWithVoidReturn() {}

            public int Method()
            {
                return 5;
            }

            public Task MethodWithTask()
            {
                return Task.Run(() => { });
            }

            public async Task<int> MethodWithTaskOfT()
            {
                await Task.Delay(10);
                return 5;
            }
        }

        [StaFact]
        public void if_no_return_path_is_specified_look_for_special_element()
        {
            var method = factory.CreateFrom(typeof(MethodHost).GetMethod("Method"));
            var returnValue = new object();
            var defaults = Mock<IElementConvention>();

            conventionManager.GetElementConvention(typeof(TextBox))
                .Returns(defaults);

            var result = binder.CreateResult(
                new MessageProcessingOutcome(
                    returnValue,
                    method.Info.ReturnType,
                    false
                    )
                );

            result.Execute(
                new ResultExecutionContext(
                    Mock<IServiceLocator>(),
                    new FakeMessage(sourceNode, method, string.Empty),
                    handlingNode
                    )
                );
            defaults.Received().SetValue(host.MethodResult, returnValue);
        }

        [StaFact]
        public void methods_with_void_return_type_return_IResult()
        {
            var method = factory.CreateFrom(typeof(MethodHost).GetMethod("MethodWithVoidReturn"));

            var result = binder.CreateResult(
                new MessageProcessingOutcome(
                    null,
                    method.Info.ReturnType,
                    false
                    )
                );

            result.ShouldNotBeNull();
        }

        [StaFact]
        public void methods_with_task_return_type_return_TaskResult()
        {
            var method = factory.CreateFrom(typeof(MethodHost).GetMethod("MethodWithTask"));

            var result = binder.CreateResult(
                new MessageProcessingOutcome(
                    null,
                    method.Info.ReturnType,
                    false
                    )
                );

            result.ShouldNotBeNull();
            result.ShouldBeOfType<TaskResult>();
        }

        [StaFact]
        public async Task methods_with_task_of_T_uses_return_path()
        {
            var method = factory.CreateFrom(typeof(MethodHost).GetMethod("MethodWithTaskOfT"));
            var returnValue = Task.FromResult(5);

            var result = binder.CreateResult(
                new MessageProcessingOutcome(
                    returnValue,
                    method.Info.ReturnType,
                    false
                    )
                );

            await result.ExecuteAsync(
                new ResultExecutionContext(
                    Mock<IServiceLocator>(),
                    new FakeMessage(sourceNode, method, "param1.Text"),
                    handlingNode
                    )
                );

            host.Param1.Text.ShouldBe("5");
        }

        [StaFact]
        public void recognizes_this_as_self_reference()
        {
            var method = factory.CreateFrom(typeof(MethodHost).GetMethod("Method"));
            var returnValue = "5";

            var result = binder.CreateResult(
                new MessageProcessingOutcome(
                    returnValue,
                    method.Info.ReturnType,
                    false
                    )
                );

            result.Execute(
                new ResultExecutionContext(
                    Mock<IServiceLocator>(),
                    new FakeMessage(sourceNode, method, "$this.DataContext"),
                    handlingNode
                    )
                );

            host.DataContext.ShouldBe(returnValue);
        }

        [StaFact]
        public void use_return_path()
        {
            var method = factory.CreateFrom(typeof(MethodHost).GetMethod("Method"));
            var returnValue = "5";

            var result = binder.CreateResult(
                new MessageProcessingOutcome(
                    returnValue,
                    method.Info.ReturnType,
                    false
                    )
                );

            result.Execute(
                new ResultExecutionContext(
                    Mock<IServiceLocator>(),
                    new FakeMessage(sourceNode, method, "param1.Text"),
                    handlingNode
                    )
                );

            host.Param1.Text.ShouldBe(returnValue);
        }
    }
}
