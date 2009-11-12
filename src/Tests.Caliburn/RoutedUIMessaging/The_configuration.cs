namespace Tests.Caliburn.RoutedUIMessaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using global::Caliburn.Core;
    using global::Caliburn.Core.IoC;
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Parsers;
    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class The_configuration : TestBase
    {
        private IConfigurationHook _hook;
        private IServiceLocator _container;

        protected override void given_the_context_of()
        {
            _hook = Mock<IConfigurationHook>();
            _container = Mock<IServiceLocator>();
        }

        [Test]
        public void is_a_CaliburnConfiguration()
        {
            _hook.Expect(x => x.Core).Return(new CoreConfiguration(_container, delegate { })).Repeat.Any();
            var config = new PresentationFrameworkModule(_hook);
            Assert.That(config, Is.InstanceOfType(typeof(CaliburnModule)));
        }

        [Test]
        public void can_be_created_using_an_extension_method()
        {
            _hook.Expect(x => x.Core).Return(new CoreConfiguration(_container, delegate { })).Repeat.Any();
            var config = _hook.WithPresentationFramework();
        }

        [Test]
        public void when_started_configures_required_components_and_defaults()
        {
            _hook.Expect(x => x.Core).Return(new CoreConfiguration(_container, delegate { })).Repeat.Any();

            var config = new PresentationFrameworkModule(_hook);
            var infos =
                config.GetType().GetMethod("GetComponents", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(
                    config, null) as IEnumerable<IComponentRegistration>;

            var found = (from info in infos.OfType<ComponentRegistrationBase>()
                         where info.Service == typeof(IRoutedMessageController)
                         select info).FirstOrDefault();

            Assert.That(found, Is.Not.Null);

            found = (from info in infos.OfType<ComponentRegistrationBase>()
                     where info.Service == typeof(IMessageBinder)
                     select info).FirstOrDefault();

            Assert.That(found, Is.Not.Null);

            found = (from info in infos.OfType<ComponentRegistrationBase>()
                     where info.Service == typeof(IParser)
                     select info).FirstOrDefault();

            Assert.That(found, Is.Not.Null);
        }

        [Test]
        public void can_provide_a_custom_routed_message_handler()
        {
            _hook.Expect(x => x.Core).Return(new CoreConfiguration(_container, delegate { })).Repeat.Any();

            var config = new PresentationFrameworkModule(_hook)
                .UsingRoutedMessageController<FakeRoutedMessageController>();

            var infos =
                config.GetType().GetMethod("GetComponents", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(
                    config, null) as IEnumerable<IComponentRegistration>;

            var found = (from info in infos.OfType<ComponentRegistrationBase>()
                         where info.Service == typeof(IRoutedMessageController)
                         select info).FirstOrDefault();

            Assert.That(found, Is.Not.EqualTo(typeof(FakeRoutedMessageController)));
        }

        [Test]
        public void can_provide_a_custom_method_binder()
        {
            _hook.Expect(x => x.Core).Return(new CoreConfiguration(_container, delegate { })).Repeat.Any();

            var config = new PresentationFrameworkModule(_hook)
                .UsingMessageBinder<FakeMethodBinder>();

            var infos =
                config.GetType().GetMethod("GetComponents", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(
                    config, null) as IEnumerable<IComponentRegistration>;

            var found = (from info in infos.OfType<ComponentRegistrationBase>()
                         where info.Service == typeof(IMessageBinder)
                         select info).FirstOrDefault();

            Assert.That(found, Is.Not.EqualTo(typeof(FakeMethodBinder)));
        }

        [Test]
        public void can_provide_a_custom_parser()
        {
            _hook.Expect(x => x.Core).Return(new CoreConfiguration(_container, delegate { })).Repeat.Any();

            var config = new PresentationFrameworkModule(_hook)
                .UsingParser<FakeMessageParser>();

            var infos =
                config.GetType().GetMethod("GetComponents", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(
                    config, null) as IEnumerable<IComponentRegistration>;

            var found = (from info in infos.OfType<ComponentRegistrationBase>()
                         where info.Service == typeof(IParser)
                         select info).FirstOrDefault();

            Assert.That(found, Is.Not.EqualTo(typeof(FakeMessageParser)));
        }
    }

    internal class FakeMethodBinder : IMessageBinder
    {
        public bool IsSpecialValue(string potential)
        {
            throw new NotImplementedException();
        }

        public void AddValueHandler(string specialValue, Func<IInteractionNode, object, object> handler)
        {
            throw new NotImplementedException();
        }

        public object[] DetermineParameters(IRoutedMessage message, IList<RequiredParameter> requiredParameters, IInteractionNode handlingNode, object context)
        {
            throw new NotImplementedException();
        }

        public IResult CreateResult(MessageProcessingOutcome outcome)
        {
            throw new NotImplementedException();
        }
    }

    internal class FakeRoutedMessageController : IRoutedMessageController
    {
        public void AddHandler(DependencyObject uiElement, IRoutedMessageHandler handler, bool setContext)
        {
            throw new NotImplementedException();
        }

        public void AttachTrigger(DependencyObject uiElement, IMessageTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public IInteractionNode GetParent(DependencyObject uiElement)
        {
            throw new NotImplementedException();
        }

        public void SetupDefaults(params InteractionDefaults[] interactionDefaults)
        {
            throw new NotImplementedException();
        }

        public InteractionDefaults GetInteractionDefaults(Type elementType)
        {
            throw new NotImplementedException();
        }
    }

    internal class FakeMessageParser : IParser
    {
        public string MessageDelimiter { get; set; }

        public void SetDefaultMessageParser(string key)
        {
            throw new NotImplementedException();
        }

        public void RegisterTriggerParser(string key, ITriggerParser parser)
        {
            throw new NotImplementedException();
        }

        public void RegisterMessageParser(string key, IMessageParser parser)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMessageTrigger> Parse(DependencyObject target, string messageText)
        {
            throw new NotImplementedException();
        }
    }
}