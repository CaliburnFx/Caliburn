using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Caliburn.Testability;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Tests.Caliburn.Fakes.Model;

namespace Tests.Caliburn.Testability
{
    [TestFixture]
    public class When_looking_for_ambiguities
    {
        [Test]
        public void can_find_content_template_and_selector_duplication()
        {
            var validator = Validator.For<ContentControl, Customer>(CreateContentControl());
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_find_headered_content_template_and_selector_duplication()
        {
            var validator = Validator.For<HeaderedContentControl, Customer>(CreateHeaderedContentControl());
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_find_item_template_and_selector_duplication()
        {
            var validator = Validator.For<ItemsControl, Customer>(CreateItemsControlWithTemplateAmbiguity());
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_find_item_container_style_and_selector_duplication()
        {
            var validator = Validator.For<ItemsControl, Customer>(CreateItemsControlWithStyleAmbiguity());
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_find_item_group_style_and_selector_duplication()
        {
            var validator = Validator.For<ItemsControl, Customer>(CreateItemsControlWithGroupStyleAmbiguity());
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_find_headered_item_template_and_selector_duplication()
        {
            var validator = Validator.For<ItemsControl, Customer>(CreateItemsControlWithHeaderAmbiguity());
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_find_container_style_ambiguity_in_group_styles()
        {
            var validator = Validator.For<ItemsControl, Customer>(CreateItemsControlWithGroupStyleContainerAmbiguity());
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_find_header_ambiguity_in_group_styles()
        {
            var validator = Validator.For<ItemsControl, Customer>(CreateItemsControlWithGroupStyleHeaderAmbiguity());
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        private ItemsControl CreateItemsControlWithGroupStyleHeaderAmbiguity()
        {
            var control = new ItemsControl();

            control.GroupStyle.Add(
                new GroupStyle
                {
                    HeaderTemplate = CreateDataTemplate(),
                    HeaderTemplateSelector = CreateTemplateSelector()
                });

            return control;
        }

        private ItemsControl CreateItemsControlWithGroupStyleContainerAmbiguity()
        {
            var control = new ItemsControl();

            control.GroupStyle.Add(
                new GroupStyle
                {
                    ContainerStyle = CreateStyle(),
                    ContainerStyleSelector = CreateStyleSelector()
                });

            return control;
        }

        private ItemsControl CreateItemsControlWithHeaderAmbiguity()
        {
            return new HeaderedItemsControl
                   {
                       HeaderTemplate = CreateDataTemplate(),
                       HeaderTemplateSelector = CreateTemplateSelector()
                   };
        }

        private ItemsControl CreateItemsControlWithGroupStyleAmbiguity()
        {
            var control = new ItemsControl();

            control.GroupStyleSelector = GroupStyleSelector;
            control.GroupStyle.Add(new GroupStyle());

            return control;
        }

        private GroupStyle GroupStyleSelector(CollectionViewGroup group, int level)
        {
            return new GroupStyle();
        }

        private ItemsControl CreateItemsControlWithStyleAmbiguity()
        {
            return new ItemsControl
            {
                ItemContainerStyle = CreateStyle(),
                ItemContainerStyleSelector = CreateStyleSelector()
            };
        }

        private StyleSelector CreateStyleSelector()
        {
            return new StyleSelector();
        }

        private Style CreateStyle()
        {
            return new Style();
        }

        private ItemsControl CreateItemsControlWithTemplateAmbiguity()
        {
            return new ItemsControl
                   {
                       ItemTemplate = CreateDataTemplate(),
                       ItemTemplateSelector = CreateTemplateSelector()
                   };
        }

        private HeaderedContentControl CreateHeaderedContentControl()
        {
            return new HeaderedContentControl
                   {
                       HeaderTemplate = CreateDataTemplate(),
                       HeaderTemplateSelector = CreateTemplateSelector()
                   };
        }

        private ContentControl CreateContentControl()
        {
            return new ContentControl
                   {
                       ContentTemplate = CreateDataTemplate(),
                       ContentTemplateSelector = CreateTemplateSelector()
                   };
        }

        private DataTemplateSelector CreateTemplateSelector()
        {
            return new DataTemplateSelector();
        }

        private DataTemplate CreateDataTemplate()
        {
            var template = new DataTemplate();
            template.VisualTree = new FrameworkElementFactory(typeof(Border));
            return template;
        }
    }
}