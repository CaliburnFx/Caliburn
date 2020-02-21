using System.Linq;
using Shouldly;

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
    using Xunit;
    using NSubstitute;


    public class When_determining_method_parameters : TestBase
    {
        DefaultMessageBinder binder;
        IInteractionNode handlingNode;
        IInteractionNode sourceNode;
        IConventionManager conventionManager;

        protected override void given_the_context_of()
        {
            conventionManager = Mock<IConventionManager>();
            binder = new DefaultMessageBinder(conventionManager);
            handlingNode = Mock<IInteractionNode>();
            sourceNode = Mock<IInteractionNode>();
        }

        [StaFact]
        public void methods_with_no_parameters_should_yield_an_empty_array()
        {
            var result = binder.DetermineParameters(
                new FakeMessage(), null, handlingNode, null
                );

            result.Length.ShouldBe(0);
        }

        [StaFact]
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

            message.Initialize(sourceNode);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("param1", typeof(string)),
                new RequiredParameter("param2", typeof(int))
            };

            var result = binder.DetermineParameters(
                message, requirements, handlingNode, null
                );

            result.Length.ShouldBe(2);
            result.Contains(param1).ShouldBeTrue();
            result.Contains(param2).ShouldBeTrue();
        }

        [StaFact]
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

            message.Initialize(sourceNode);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("param1", typeof(string)),
                new RequiredParameter("param2", typeof(int))
            };

            var result = binder.DetermineParameters(
                message, requirements, handlingNode, null
                );

            result.Length.ShouldBe(2);
            result.Contains(param1.ToString()).ShouldBeTrue();
            result.Contains(Convert.ToInt32(param2)).ShouldBeTrue();
        }

        [StaFact]
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

            message.Initialize(sourceNode);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("param1", typeof(EventArgs))
            };

            var result = binder.DetermineParameters(
                message, requirements, handlingNode, context
                );

            result.Length.ShouldBe(1);
            result.Contains(context).ShouldBeTrue();
        }

        [StaFact]
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

            message.Initialize(sourceNode);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("param1", typeof(object))
            };

            var result = binder.DetermineParameters(
                message, requirements, handlingNode, context
                );

            result.Length.ShouldBe(1);
            result.Contains(context).ShouldBeTrue();
        }

        [StaFact]
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

            sourceNode.UIElement.Returns(source);
            message.Initialize(sourceNode);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("param1", typeof(Button))
            };

            var result = binder.DetermineParameters(
                message, requirements, handlingNode, null
                );

            result.Length.ShouldBe(1);
            result.Contains(source).ShouldBeTrue();
        }

        [StaFact]
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

            sourceNode.UIElement.Returns(source);
            message.Initialize(sourceNode);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("param1", typeof(object))
            };

            var result = binder.DetermineParameters(
                message, requirements, handlingNode, null
                );

            result.Length.ShouldBe(1);
            result.Contains(source.DataContext).ShouldBeTrue();
        }

        [StaFact]
        public void should_resolve_special_parameter_value()
        {
            const string param1 = "$value";
            var source = new TextBox { Text = "the value" };

            var convention = Mock<IElementConvention>();
            conventionManager.GetElementConvention(typeof(TextBox))
                .Returns(convention);
            convention.GetValue(source).Returns(source.Text);

            var message = new FakeMessage
            {
                Parameters = new FreezableCollection<Parameter>
                {
                    new Parameter {Value = param1},
                }
            };

            sourceNode.UIElement.Returns(source);
            message.Initialize(sourceNode);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("param1", typeof(object))
            };

            var result = binder.DetermineParameters(
                message, requirements, handlingNode, null
                );

            result.Length.ShouldBe(1);
            result.Contains(source.Text).ShouldBeTrue();
        }

        [StaFact]
        public void if_none_are_provided_should_search_the_UI()
        {
            const int param1 = 56;
            const double param2 = 34.0;

            var element = new ControlHost();
            element.SetParam1(param1);
            element.SetParam2(param2);

            handlingNode.UIElement.Returns(element);

            var defaults = Mock<IElementConvention>();

            conventionManager.GetElementConvention(typeof(TextBox))
                .Returns(defaults);
            var stack = new Stack<object>();
            stack.Push(param1);
            stack.Push(param2);
            defaults.GetValue(null)
                .ReturnsForAnyArgs(param1, param2);

            var message = new FakeMessage();

            message.Initialize(sourceNode);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("param1", typeof(string)),
                new RequiredParameter("param2", typeof(int))
            };

            var result = binder.DetermineParameters(
                message, requirements, handlingNode, null
                );

            result.Length.ShouldBe(2);
            result.Contains(param1.ToString()).ShouldBeTrue();
            result.Contains(Convert.ToInt32(param2)).ShouldBeTrue();
        }

        [StaFact]
        public void if_none_are_provided_check_for_eventArgs()
        {
            var context = EventArgs.Empty;

            var message = new FakeMessage();
            message.Initialize(sourceNode);

            var element = new ControlHost();
            handlingNode.UIElement.Returns(element);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("eventArgs", typeof(EventArgs)),
            };

            var result = binder.DetermineParameters(
                message, requirements, handlingNode, context
                );

            result.Length.ShouldBe(1);
            result.Contains(context).ShouldBeTrue();
        }

        [StaFact]
        public void if_none_are_provided_check_for_special_parameter()
        {
            var context = new object();

            var message = new FakeMessage();
            message.Initialize(sourceNode);

            var element = new ControlHost();
            handlingNode.UIElement.Returns(element);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("parameter", typeof(object)),
            };

            var result = binder.DetermineParameters(
                message, requirements, handlingNode, context
                );

            result.Length.ShouldBe(1);
            result.Contains(context).ShouldBeTrue();
        }

        [StaFact]
        public void if_none_are_provided_check_for_source()
        {
            var source = new Button();

            sourceNode.UIElement.Returns(source);

            var message = new FakeMessage();
            message.Initialize(sourceNode);

            var element = new ControlHost();
            handlingNode.UIElement.Returns(element);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("source", typeof(object)),
            };

            var result = binder.DetermineParameters(
                message, requirements, handlingNode, null
                );

            result.Length.ShouldBe(1);
            result.Contains(source).ShouldBeTrue();
        }

        [StaFact]
        public void if_none_are_provided_check_for_dataContext()
        {
            var source = new Button { DataContext = new object() };

            sourceNode.UIElement.Returns(source);

            var message = new FakeMessage();
            message.Initialize(sourceNode);

            var element = new ControlHost();
            handlingNode.UIElement.Returns(element);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("datacontext", typeof(object)),
            };

            var result = binder.DetermineParameters(
                message, requirements, handlingNode, null
                );

            result.Length.ShouldBe(1);
            result.Contains(source.DataContext).ShouldBeTrue();
        }

        [StaFact]
        public void if_none_are_provided_check_for_value()
        {
            var source = new TextBox { Text = "the text" };

            sourceNode.UIElement.Returns(source);

            var defaults = Mock<IElementConvention>();

            conventionManager.GetElementConvention(typeof(TextBox))
                .Returns(defaults);

            defaults.GetValue(source).Returns(source.Text);

            var message = new FakeMessage();
            message.Initialize(sourceNode);

            var element = new ControlHost();
            handlingNode.UIElement.Returns(element);

            var requirements = new List<RequiredParameter>
            {
                new RequiredParameter("value", typeof(object)),
            };

            var result = binder.DetermineParameters(
                message, requirements, handlingNode, null
                );

            result.Length.ShouldBe(1);
            result.Contains(source.Text).ShouldBeTrue();
        }
    }
}
