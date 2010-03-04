namespace Tests.Caliburn.RoutedUIMessaging
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Fakes;
    using Fakes.UI;
    using global::Caliburn.PresentationFramework.Conventions;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class When_determining_method_parameters : TestBase
    {
        private DefaultMessageBinder _binder;
        private IInteractionNode _handlingNode;
        private IInteractionNode _sourceNode;
        private IConventionManager _conventionManager;

        protected override void given_the_context_of()
        {
            _conventionManager = Mock<IConventionManager>();
            _binder = new DefaultMessageBinder(_conventionManager);
            _handlingNode = Stub<IInteractionNode>();
            _sourceNode = Stub<IInteractionNode>();
        }

        [Test]
        public void methods_with_no_parameters_should_yield_an_empty_array()
        {
            var result = _binder.DetermineParameters(
                new FakeMessage(), null, _handlingNode, null
                );

            Assert.That(result, Has.Length(0));
        }

        [Test]
        public void methods_with_parameters_equal_to_those_provided_should_yield_provided()
        {
            const string param1 = "a string";
            const int param2 = 56;

            var message = new FakeMessage
            {
                Parameters = new FreezableCollection<Parameter>
                {
                    new Parameter {Value = param1},
                    new Parameter {Value = param2}
                }
            };

            message.Initialize(_sourceNode);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("param1", typeof(string)),
                new RequiredParameter("param2", typeof(int))
            };

            var result = _binder.DetermineParameters(
                message, requirements, _handlingNode, null
                );

            Assert.That(result, Has.Length(2));
            Assert.That(result, Has.Member(param1));
            Assert.That(result, Has.Member(param2));
        }

        [Test]
        public void parameters_should_be_coerced_to_the_proper_type()
        {
            const int param1 = 56;
            const double param2 = 34.0;

            var message = new FakeMessage
            {
                Parameters = new FreezableCollection<Parameter>
                {
                    new Parameter {Value = param1},
                    new Parameter {Value = param2}
                }
            };

            message.Initialize(_sourceNode);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("param1", typeof(string)),
                new RequiredParameter("param2", typeof(int))
            };

            var result = _binder.DetermineParameters(
                message, requirements, _handlingNode, null
                );

            Assert.That(result, Has.Length(2));
            Assert.That(result, Has.Member(param1.ToString()));
            Assert.That(result, Has.Member(Convert.ToInt32(param2)));
        }

        [Test]
        public void should_resolve_special_parameter_eventArgs()
        {
            const string param1 = "$eventArgs";
            var context = EventArgs.Empty;

            var message = new FakeMessage
            {
                Parameters = new FreezableCollection<Parameter>
                {
                    new Parameter {Value = param1},
                }
            };

            message.Initialize(_sourceNode);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("param1", typeof(EventArgs))
            };

            var result = _binder.DetermineParameters(
                message, requirements, _handlingNode, context
                );

            Assert.That(result, Has.Length(1));
            Assert.That(result, Has.Member(context));
        }

        [Test]
        public void should_resolve_special_parameter_parameter()
        {
            const string param1 = "$parameter";
            var context = new object();

            var message = new FakeMessage
            {
                Parameters = new FreezableCollection<Parameter>
                {
                    new Parameter {Value = param1},
                }
            };

            message.Initialize(_sourceNode);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("param1", typeof(object))
            };

            var result = _binder.DetermineParameters(
                message, requirements, _handlingNode, context
                );

            Assert.That(result, Has.Length(1));
            Assert.That(result, Has.Member(context));
        }

        [Test]
        public void should_resolve_special_parameter_source()
        {
            const string param1 = "$source";
            var source = new Button();

            var message = new FakeMessage
            {
                Parameters = new FreezableCollection<Parameter>
                {
                    new Parameter {Value = param1},
                }
            };

            _sourceNode.Stub(x => x.UIElement).Return(source);
            message.Initialize(_sourceNode);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("param1", typeof(Button))
            };

            var result = _binder.DetermineParameters(
                message, requirements, _handlingNode, null
                );

            Assert.That(result, Has.Length(1));
            Assert.That(result, Has.Member(source));
        }

        [Test]
        public void should_resolve_special_parameter_dataContext()
        {
            const string param1 = "$dataContext";
            var source = new Button { DataContext = new object() };

            var message = new FakeMessage
            {
                Parameters = new FreezableCollection<Parameter>
                {
                    new Parameter {Value = param1},
                }
            };

            _sourceNode.Stub(x => x.UIElement).Return(source);
            message.Initialize(_sourceNode);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("param1", typeof(object))
            };

            var result = _binder.DetermineParameters(
                message, requirements, _handlingNode, null
                );

            Assert.That(result, Has.Length(1));
            Assert.That(result, Has.Member(source.DataContext));
        }

        [Test]
        public void should_resolve_special_parameter_value()
        {
            const string param1 = "$value";
            var source = new TextBox { Text = "the value" };

            var convention = Mock<IElementConvention>();
            _conventionManager.Expect(x => x.GetElementConvention(typeof(TextBox)))
                .Return(convention);
            convention.Expect(x => x.GetValue(source)).Return(source.Text);

            var message = new FakeMessage
            {
                Parameters = new FreezableCollection<Parameter>
                {
                    new Parameter {Value = param1},
                }
            };

            _sourceNode.Stub(x => x.UIElement).Return(source);
            message.Initialize(_sourceNode);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("param1", typeof(object))
            };

            var result = _binder.DetermineParameters(
                message, requirements, _handlingNode, null
                );

            Assert.That(result, Has.Length(1));
            Assert.That(result, Has.Member(source.Text));
        }

        [Test]
        public void if_none_are_provided_should_search_the_UI()
        {
            const int param1 = 56;
            const double param2 = 34.0;

            var element = new ControlHost();
            element.SetParam1(param1);
            element.SetParam2(param2);

            _handlingNode.Stub(x => x.UIElement).Return(element).Repeat.Twice();

            var defaults = Mock<IElementConvention>();

            _conventionManager.Expect(x => x.GetElementConvention(typeof(TextBox)))
                .Return(defaults).Repeat.Twice();

            defaults.Expect(x => x.GetValue(Arg<DependencyObject>.Is.Anything)).Return(param1);
            defaults.Expect(x => x.GetValue(Arg<DependencyObject>.Is.Anything)).Return(param2);

            var message = new FakeMessage();

            message.Initialize(_sourceNode);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("param1", typeof(string)),
                new RequiredParameter("param2", typeof(int))
            };

            var result = _binder.DetermineParameters(
                message, requirements, _handlingNode, null
                );

            Assert.That(result, Has.Length(2));
            Assert.That(result, Has.Member(param1.ToString()));
            Assert.That(result, Has.Member(Convert.ToInt32(param2)));
        }

        [Test]
        public void if_none_are_provided_check_for_eventArgs()
        {
            var context = EventArgs.Empty;

            var message = new FakeMessage();
            message.Initialize(_sourceNode);

            var element = new ControlHost();
            _handlingNode.Stub(x => x.UIElement).Return(element).Repeat.Twice();

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("eventArgs", typeof(EventArgs)),
            };

            var result = _binder.DetermineParameters(
                message, requirements, _handlingNode, context
                );

            Assert.That(result, Has.Length(1));
            Assert.That(result, Has.Member(context));
        }

        [Test]
        public void if_none_are_provided_check_for_special_parameter()
        {
            var context = new object();

            var message = new FakeMessage();
            message.Initialize(_sourceNode);

            var element = new ControlHost();
            _handlingNode.Stub(x => x.UIElement).Return(element).Repeat.Twice();

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("parameter", typeof(object)),
            };

            var result = _binder.DetermineParameters(
                message, requirements, _handlingNode, context
                );

            Assert.That(result, Has.Length(1));
            Assert.That(result, Has.Member(context));
        }

        [Test]
        public void if_none_are_provided_check_for_source()
        {
            var source = new Button();

            _sourceNode.Stub(x => x.UIElement).Return(source);

            var message = new FakeMessage();
            message.Initialize(_sourceNode);

            var element = new ControlHost();
            _handlingNode.Stub(x => x.UIElement).Return(element).Repeat.Twice();

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("source", typeof(object)),
            };

            var result = _binder.DetermineParameters(
                message, requirements, _handlingNode, null
                );

            Assert.That(result, Has.Length(1));
            Assert.That(result, Has.Member(source));
        }

        [Test]
        public void if_none_are_provided_check_for_dataContext()
        {
            var source = new Button { DataContext = new object() };

            _sourceNode.Stub(x => x.UIElement).Return(source);

            var message = new FakeMessage();
            message.Initialize(_sourceNode);

            var element = new ControlHost();
            _handlingNode.Stub(x => x.UIElement).Return(element).Repeat.Twice();

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("datacontext", typeof(object)),
            };

            var result = _binder.DetermineParameters(
                message, requirements, _handlingNode, null
                );

            Assert.That(result, Has.Length(1));
            Assert.That(result, Has.Member(source.DataContext));
        }

        [Test]
        public void if_none_are_provided_check_for_value()
        {
            var source = new TextBox { Text = "the text" };

            _sourceNode.Stub(x => x.UIElement).Return(source);

            var defaults = Stub<IElementConvention>();

            _conventionManager.Expect(x => x.GetElementConvention(typeof(TextBox)))
                .Return(defaults);

            defaults.Expect(x => x.GetValue(source)).Return(source.Text);

            var message = new FakeMessage();
            message.Initialize(_sourceNode);

            var element = new ControlHost();
            _handlingNode.Stub(x => x.UIElement).Return(element).Repeat.Twice();

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("value", typeof(object)),
            };

            var result = _binder.DetermineParameters(
                message, requirements, _handlingNode, null
                ); 

            Assert.That(result, Has.Length(1));
            Assert.That(result, Has.Member(source.Text));
        }
    }
}