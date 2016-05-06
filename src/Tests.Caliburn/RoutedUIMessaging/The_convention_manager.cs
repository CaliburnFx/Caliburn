using Shouldly;

namespace Tests.Caliburn.RoutedUIMessaging
{
    using System.Windows.Controls.Primitives;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.PresentationFramework.Conventions;
    using Xunit;
    using System.Windows.Controls;

    
    public class The_convention_manager : TestBase
    {
        IConventionManager conventionManager;
        IMethodFactory methodFactory;

        protected override void given_the_context_of()
        {
            methodFactory = Mock<IMethodFactory>();

            conventionManager = new DefaultConventionManager(
                methodFactory
                );
        }

        [Fact]
        public void can_register_interaction_defaults()
        {
            var defaults = new DefaultElementConvention<TextBox>(
                "TextChanged",
                TextBox.TextProperty,
                (c, v) => c.Text = v.ToString(),
                c => c.Text,
                null
                );

            conventionManager.AddElementConvention(defaults);

            var found = conventionManager.GetElementConvention(typeof(TextBox));

            found.ShouldBeSameAs(defaults);
        }

        [Fact]
        public void can_get_defaults_if_only_a_base_class_is_registered()
        {
            var defaults = new DefaultElementConvention<ButtonBase>(
                "Click",
                ButtonBase.ContentProperty,
                (c, v) => c.DataContext = v,
                c => c.DataContext,
                null
                );

            conventionManager.AddElementConvention(defaults);

            var found = conventionManager.GetElementConvention(typeof(MyButton));

            found.ShouldBeSameAs(defaults);
        }

        private class MyButton : ButtonBase
        {

        }
    }
}