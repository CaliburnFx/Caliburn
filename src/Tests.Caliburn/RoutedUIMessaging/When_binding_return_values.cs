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
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
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
            handlingNode = Stub<IInteractionNode>();
            sourceNode = Stub<IInteractionNode>();
            host = new ControlHost();

            sourceNode.Stub(x => x.UIElement).Return(host).Repeat.Any();
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
        }

        [Test]
        public void if_no_return_path_is_specified_look_for_special_element()
        {
            var method = factory.CreateFrom(typeof(MethodHost).GetMethod("Method"));
            var returnValue = new object();
            var defaults = Mock<IElementConvention>();

            conventionManager.Expect(x => x.GetElementConvention(typeof(TextBox)))
                .Return(defaults);

            defaults.Expect(x => x.SetValue(host._methodResult, returnValue));

            var result = binder.CreateResult(
                new MessageProcessingOutcome(
                    returnValue,
                    method.Info.ReturnType,
                    false
                    )
                );

            result.Execute(
                new ResultExecutionContext(
                    Stub<IServiceLocator>(),
                    new FakeMessage(sourceNode, method, string.Empty),
                    handlingNode
                    )
                );
        }

        [Test]
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

            Assert.That(result, Is.Not.Null);
        }

        [Test]
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
                    Stub<IServiceLocator>(),
                    new FakeMessage(sourceNode, method, "$this.DataContext"),
                    handlingNode
                    )
                );

            Assert.That(host.DataContext, Is.EqualTo(returnValue));
        }

        [Test]
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
                    Stub<IServiceLocator>(),
                    new FakeMessage(sourceNode, method, "param1.Text"),
                    handlingNode
                    )
                );

            Assert.That(host._param1.Text, Is.EqualTo(returnValue));
        }
    }
}