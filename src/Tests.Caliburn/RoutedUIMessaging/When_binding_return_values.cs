namespace Tests.Caliburn.RoutedUIMessaging
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Fakes.UI;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.Core.Threading;
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Conventions;
    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class When_binding_return_values : TestBase
    {
        private DefaultMethodFactory _factory;
        private DefaultMessageBinder _binder;
        private IInteractionNode _handlingNode;
        private IInteractionNode _sourceNode;
        private ControlHost _host;
        private IConventionManager _conventionManager;

        protected override void given_the_context_of()
        {
            _factory = new DefaultMethodFactory(new DefaultThreadPool());
            _conventionManager = Mock<IConventionManager>();
            _binder = new DefaultMessageBinder(_conventionManager);
            _handlingNode = Stub<IInteractionNode>();
            _sourceNode = Stub<IInteractionNode>();
            _host = new ControlHost();

            _sourceNode.Stub(x => x.UIElement).Return(_host).Repeat.Any();
        }

        [Test]
        public void methods_with_void_return_type_return_IResult()
        {
            var method = _factory.CreateFrom(typeof(MethodHost).GetMethod("MethodWithVoidReturn"));

            var result = _binder.CreateResult(
                new MessageProcessingOutcome(
                    null,
                    method.Info.ReturnType,
                    false
                    )
                );

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void if_no_return_path_is_specified_look_for_special_element()
        {
            var method = _factory.CreateFrom(typeof(MethodHost).GetMethod("Method"));
            var returnValue = new object();
            var defaults = Mock<IElementConvention>();

            _conventionManager.Expect(x => x.GetElementConvention(typeof(TextBox)))
                .Return(defaults);

            defaults.Expect(x => x.SetValue(_host._methodResult, returnValue));

            var result = _binder.CreateResult(
                new MessageProcessingOutcome(
                    returnValue,
                    method.Info.ReturnType,
                    false
                    )
                );

            result.Execute(
                new ResultExecutionContext(
                    Stub<IServiceLocator>(),
                    new FakeMessage(_sourceNode, method, string.Empty),
                    _handlingNode
                    )
                );
        }

        [Test]
        public void use_return_path()
        {
            var method = _factory.CreateFrom(typeof(MethodHost).GetMethod("Method"));
            var returnValue = "5";

            var result = _binder.CreateResult(
                new MessageProcessingOutcome(
                    returnValue,
                    method.Info.ReturnType,
                    false
                    )
                );

            result.Execute(
                new ResultExecutionContext(
                    Stub<IServiceLocator>(),
                    new FakeMessage(_sourceNode, method, "param1.Text"),
                    _handlingNode
                    )
                );

            Assert.That(_host._param1.Text, Is.EqualTo(returnValue));
        }

        [Test]
        public void recognizes_this_as_self_reference()
        {
            var method = _factory.CreateFrom(typeof(MethodHost).GetMethod("Method"));
            var returnValue = "5";

            var result = _binder.CreateResult(
                new MessageProcessingOutcome(
                    returnValue,
                    method.Info.ReturnType,
                    false
                    )
                );

            result.Execute(
                new ResultExecutionContext(
                    Stub<IServiceLocator>(),
                    new FakeMessage(_sourceNode, method, "$this.DataContext"),
                    _handlingNode
                    )
                );

            Assert.That(_host.DataContext, Is.EqualTo(returnValue));
        }

        public class FakeMessage : IRoutedMessageWithOutcome
        {
            private IInteractionNode _source;
            private IMethod _method;
            private string _outcomePath;

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
        }

        public class MethodHost
        {
            public void MethodWithVoidReturn() {}

            public int Method()
            {
                return 5;
            }
        }
    }
}