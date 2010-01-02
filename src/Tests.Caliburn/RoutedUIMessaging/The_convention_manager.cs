namespace Tests.Caliburn.RoutedUIMessaging
{
    using System.Windows.Controls.Primitives;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.PresentationFramework.Conventions;
    using NUnit.Framework;
    using System.Windows.Controls;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class The_convention_manager : TestBase
    {
        private IConventionManager _conventionManager;
        private IMethodFactory _methodFactory;
        private IEventHandlerFactory _eventHandlerFactory;

        protected override void given_the_context_of()
        {
            _methodFactory = Mock<IMethodFactory>();
            _eventHandlerFactory = Mock<IEventHandlerFactory>();

            _conventionManager = new DefaultConventionManager(
                _methodFactory,
                _eventHandlerFactory
                );
        }

        [Test]
        public void can_register_interaction_defaults()
        {
            var defaults = new DefaultElementConvention<TextBox>(
                Mock<IEventHandlerFactory>(),
                "TextChanged",
                TextBox.TextProperty,
                (c, v) => c.Text = v.ToString(),
                c => c.Text
                );

            _conventionManager.AddElementConvention(defaults);

            var found = _conventionManager.GetElementConvention(typeof(TextBox));

            Assert.That(found, Is.SameAs(defaults));
        }

        [Test]
        public void can_get_defaults_if_only_a_base_class_is_registered()
        {
            var defaults = new DefaultElementConvention<ButtonBase>(
                Mock<IEventHandlerFactory>(),
                "Click",
                ButtonBase.ContentProperty,
                (c, v) => c.DataContext = v,
                c => c.DataContext
                );

            _conventionManager.AddElementConvention(defaults);

            var found = _conventionManager.GetElementConvention(typeof(MyButton));

            Assert.That(found, Is.SameAs(defaults));
        }

        private class MyButton : ButtonBase
        {

        }
    }
}