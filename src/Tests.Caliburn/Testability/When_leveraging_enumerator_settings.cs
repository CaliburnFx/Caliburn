using Shouldly;

namespace Tests.Caliburn.Testability
{
    using System.Linq;
    using Fakes.Model;
    using Fakes.UI;
    using global::Caliburn.Testability;
    using Xunit;


    public class When_leveraging_enumerator_settings : TestBase
    {
        [StaFact]
        public void can_exclude_content_control_templates()
        {
            var validator = Validator.For<UIBoundToCustomerWithContentControl, Customer>();

            validator.Settings.IncludeTemplates = false;

            var result = validator.Validate();

            result.Errors.Count().ShouldBe(0);
        }

        [StaFact]
        public void can_exclude_group_styles()
        {
            var validator = Validator.For<UIWithItemsControlGroupStyleTemplate, Customer>();

            validator.Settings.IncludeStyles = false;

            var result = validator.Validate();

            result.Errors.Count().ShouldBe(0);
        }

        [StaFact]
        public void can_exclude_items_control_templates()
        {
            var validator = Validator.For<UIBoundToCustomerWithItemsControl, Customer>();

            validator.Settings.IncludeTemplates = false;

            var result = validator.Validate();

            result.Errors.Count().ShouldBe(2);
        }

        [StaFact]
        public void can_exclude_styles()
        {
            var validator = Validator.For<UIBoundToCustomerWithStyle, Customer>();

            validator.Settings.IncludeStyles = false;

            var result = validator.Validate();

            result.Errors.Count().ShouldBe(0);
        }

        [StaFact]
        public void can_include_child_user_controls()
        {
            var validator = Validator.For<SimpleUIBoundToCustomer, Customer>();

            validator.Settings.TraverseUserControls = true;

            var result = validator.Validate();

            result.Errors.Count().ShouldBe(3);
        }

        [StaFact]
        public void can_include_collections()
        {
            var validator = Validator.For<UIWithBoundBrush, StopModel>();

            validator.Settings.IncludeAllDependencyObjects = true;

            var result = validator.Validate();

            result.Errors.Count().ShouldBe(1);
        }

        [StaFact]
        public void can_include_properties_with_dependency_object_values()
        {
            var validator = Validator.For<SimpleUIBoundToCustomer, Customer>();

            validator.Settings.IncludeAllDependencyObjects = true;

            var result = validator.Validate();

            result.Errors.Count().ShouldBe(3);
        }

        [StaFact]
        public void can_stop_after_first_error()
        {
            var validator = Validator.For<SimpleUIBoundToCustomer, Customer>();

            validator.Settings.StopAfterFirstError = true;

            var result = validator.Validate();

            result.Errors.Count().ShouldBe(1);
        }
    }
}
