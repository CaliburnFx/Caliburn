namespace Tests.Caliburn.Testability
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Fakes.Model;
    using global::Caliburn.Testability;
    using NUnit.Framework;

    [TestFixture]
    public class When_looking_for_ambiguities
    {
        ItemsControl CreateItemsControlWithGroupStyleHeaderAmbiguity()
        {
            var control = new ItemsControl();

            control.GroupStyle.Add(
                new GroupStyle {
                    HeaderTemplate = CreateDataTemplate(),
                    HeaderTemplateSelector = CreateTemplateSelector()
                });

            return control;
        }

        ItemsControl CreateItemsControlWithGroupStyleContainerAmbiguity()
        {
            var control = new ItemsControl();

            control.GroupStyle.Add(
                new GroupStyle {
                    ContainerStyle = CreateStyle(),
                    ContainerStyleSelector = CreateStyleSelector()
                });

            return control;
        }

        ItemsControl CreateItemsControlWithHeaderAmbiguity()
        {
            return new HeaderedItemsControl {
                HeaderTemplate = CreateDataTemplate(),
                HeaderTemplateSelector = CreateTemplateSelector()
            };
        }

        ItemsControl CreateItemsControlWithGroupStyleAmbiguity()
        {
            var control = new ItemsControl();

            control.GroupStyleSelector = GroupStyleSelector;
            control.GroupStyle.Add(new GroupStyle());

            return control;
        }

        GroupStyle GroupStyleSelector(CollectionViewGroup group, int level)
        {
            return new GroupStyle();
        }

        ItemsControl CreateItemsControlWithStyleAmbiguity()
        {
            return new ItemsControl {
                ItemContainerStyle = CreateStyle(),
                ItemContainerStyleSelector = CreateStyleSelector()
            };
        }

        StyleSelector CreateStyleSelector()
        {
            return new StyleSelector();
        }

        Style CreateStyle()
        {
            return new Style();
        }

        ItemsControl CreateItemsControlWithTemplateAmbiguity()
        {
            return new ItemsControl {
                ItemTemplate = CreateDataTemplate(),
                ItemTemplateSelector = CreateTemplateSelector()
            };
        }

        HeaderedContentControl CreateHeaderedContentControl()
        {
            return new HeaderedContentControl {
                HeaderTemplate = CreateDataTemplate(),
                HeaderTemplateSelector = CreateTemplateSelector()
            };
        }

        ContentControl CreateContentControl()
        {
            return new ContentControl {
                ContentTemplate = CreateDataTemplate(),
                ContentTemplateSelector = CreateTemplateSelector()
            };
        }

        DataTemplateSelector CreateTemplateSelector()
        {
            return new DataTemplateSelector();
        }

        DataTemplate CreateDataTemplate()
        {
            var template = new DataTemplate();
            template.VisualTree = new FrameworkElementFactory(typeof(Border));
            return template;
        }

        [Test]
        public void can_find_container_style_ambiguity_in_group_styles()
        {
            var validator = Validator.For<ItemsControl, Customer>(CreateItemsControlWithGroupStyleContainerAmbiguity());
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_find_content_template_and_selector_duplication()
        {
            var validator = Validator.For<ContentControl, Customer>(CreateContentControl());
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

        [Test]
        public void can_find_headered_content_template_and_selector_duplication()
        {
            var validator = Validator.For<HeaderedContentControl, Customer>(CreateHeaderedContentControl());
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
        public void can_find_item_template_and_selector_duplication()
        {
            var validator = Validator.For<ItemsControl, Customer>(CreateItemsControlWithTemplateAmbiguity());
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }
    }
}