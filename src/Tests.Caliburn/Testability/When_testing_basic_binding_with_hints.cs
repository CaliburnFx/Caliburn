using System.Xml.XPath;
using Shouldly;

namespace Tests.Caliburn.Testability
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Fakes.Model;
    using global::Caliburn.Testability;
    using Xunit;

    
    public class When_testing_basic_binding_with_hints : TestBase
    {
        public class MyUI : UserControl
        {
            public MyUI(string bindingPath)
            {
                var stack = new StackPanel();
                Content = stack;

                var textBlock = new TextBlock();
                textBlock.SetBinding(TextBlock.TextProperty, new Binding(bindingPath));
                stack.Children.Add(textBlock);
            }
        }

        public class MyUIWithTemplate : UserControl
        {
            public MyUIWithTemplate(string rootBindingPath, string templateBindingPath)
            {
                ContentTemplate = CreateTemplate(templateBindingPath);
                SetBinding(ContentProperty, new Binding(rootBindingPath));
            }

            DataTemplate CreateTemplate(string bindingPath)
            {
                var template = new DataTemplate();

                var textBlock = new FrameworkElementFactory(typeof(TextBlock));
                textBlock.SetBinding(TextBlock.TextProperty, new Binding(bindingPath));
                template.VisualTree = textBlock;

                return template;
            }
        }

        [WpfFact]
        public void can_detect_actual_type_of_referenced_object()
        {
            var validator = Validator.For<MyUI, MyPresenter>(new MyUI("Model.MyProperty"))
                .WithHint(x => x.Model, typeof(MyModel));

            var result = validator.Validate();

            result.Errors.Count().ShouldBe(0, result.ErrorSummary);
        }

        [WpfFact]
        public void can_detect_typed_then_untyped()
        {
            var validator = Validator.For<MyUI, MyPresenter>(new MyUI("TypedModel.SubModel.MySubProperty"))
                .WithHint(x => x.TypedModel.SubModel, typeof(MySubModel));

            var result = validator.Validate();

            result.Errors.Count().ShouldBe(0, result.ErrorSummary);
        }

        [WpfFact]
        public void can_detect_untyped_then_typed()
        {
            var validator = Validator.For<MyUI, MyPresenter>(new MyUI("Model.TypedSubModel.MySubProperty"))
                .WithHint(x => x.Model, typeof(MyModel));

            var result = validator.Validate();

            result.Errors.Count().ShouldBe(0, result.ErrorSummary);
        }

        [WpfFact]
        public void can_detect_untyped_then_untyped()
        {
            var validator = Validator.For<MyUI, MyPresenter>(new MyUI("Model.SubModel.MySubProperty"))
                .WithHint(x => x.Model, typeof(MyModel))
                .WithHint("Model.SubModel", typeof(MySubModel));

            var result = validator.Validate();

            result.Errors.Count().ShouldBe(0, result.ErrorSummary);
        }

        [WpfFact]
        public void can_detect_untyped_then_untyped_in_template()
        {
            var validator = Validator.For<MyUIWithTemplate, MyPresenter>(new MyUIWithTemplate("Model",
                "SubModel.MySubProperty"))
                .WithHint(x => x.Model, typeof(MyModel))
                .WithHint("Model.SubModel", typeof(MySubModel));

            var result = validator.Validate();

            result.Errors.Count().ShouldBe(0, result.ErrorSummary);
        }

        [WpfFact]
        public void cannot_detect_actual_type_if_no_hints_given()
        {
            var validator = Validator.For<MyUI, MyPresenter>(new MyUI("Model.MyProperty"));
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(1, result.ErrorSummary);
        }
    }
}