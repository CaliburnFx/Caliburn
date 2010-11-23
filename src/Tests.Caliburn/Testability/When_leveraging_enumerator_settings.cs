using System.Linq;
using Caliburn.Testability;
using NUnit.Framework;
using Tests.Caliburn.Fakes.Model;
using Tests.Caliburn.Fakes.UI;

namespace Tests.Caliburn.Testability
{
    [TestFixture]
    public class When_leveraging_enumerator_settings : TestBase
    {
        [Test]
        public void can_stop_after_first_error()
        {
            var validator = Validator.For<SimpleUIBoundToCustomer, Customer>();

            validator.Settings.StopAfterFirstError = true;

            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_execlude_styles()
        {
            var validator = Validator.For<UIBoundToCustomerWithStyle, Customer>();

            validator.Settings.IncludeStyles = false;

            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(0));
        }

        [Test]
        public void can_exclude_group_styles()
        {
            var validator = Validator.For<UIWithItemsControlGroupStyleTemplate, Customer>();

            validator.Settings.IncludeStyles = false;

            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(0));
        }

        [Test]
        public void can_exclude_items_control_templates()
        {
            var validator = Validator.For<UIBoundToCustomerWithItemsControl, Customer>();

            validator.Settings.IncludeTemplates = false;

            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(2));
        }

        [Test]
        public void can_exclude_content_control_templates()
        {
            var validator = Validator.For<UIBoundToCustomerWithContentControl, Customer>();

            validator.Settings.IncludeTemplates = false;

            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(0));
        }

        [Test]
        public void can_include_properties_with_dependency_object_values()
        {
            var validator = Validator.For<SimpleUIBoundToCustomer, Customer>();

            validator.Settings.IncludeAllDependencyObjects = true;

            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(3));
        }

        [Test]
        public void can_include_child_user_controls()
        {
            var validator = Validator.For<SimpleUIBoundToCustomer, Customer>();

            validator.Settings.TraverseUserControls = true;

            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(3));
        }

        [Test]
        public void can_include_collections()
        {
            var validator = Validator.For<UIWithBoundBrush, StopModel>();

            validator.Settings.IncludeAllDependencyObjects = true;

            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }
    }
}