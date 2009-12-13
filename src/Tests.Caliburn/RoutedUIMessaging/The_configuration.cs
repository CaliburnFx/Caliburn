namespace Tests.Caliburn.RoutedUIMessaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using global::Caliburn.Core.Configuration;
    using global::Caliburn.Core.IoC;
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Actions;
    using global::Caliburn.PresentationFramework.ApplicationModel;
    using global::Caliburn.PresentationFramework.Configuration;
    using global::Caliburn.PresentationFramework.Parsers;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class The_configuration : TestBase
    {
        private PresentationFrameworkConfiguration _config;
        private IModule _module;

        protected override void given_the_context_of()
        {
            _config = ConventionalModule<PresentationFrameworkConfiguration, IPresentationFrameworkServicesDescription>.Instance;
            _module = _config;
        }

        [Test]
        public void when_started_configures_required_components_and_defaults()
        {
            var registrations = _module.GetComponents();

            var found = (from reg in registrations.OfType<Singleton>()
                         where reg.Service == typeof(IRoutedMessageController)
                         select reg).FirstOrDefault();

            Assert.That(found, Is.Not.Null);
            Assert.That(found.Implementation, Is.Not.Null);

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IMessageBinder)
                     select reg).FirstOrDefault();

            Assert.That(found, Is.Not.Null);
            Assert.That(found.Implementation, Is.Not.Null);

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IParser)
                     select reg).FirstOrDefault();

            Assert.That(found, Is.Not.Null);
            Assert.That(found.Implementation, Is.Not.Null);

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IActionFactory)
                     select reg).FirstOrDefault();

            Assert.That(found, Is.Not.Null);
            Assert.That(found.Implementation, Is.Not.Null);

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IViewStrategy)
                     select reg).FirstOrDefault();

            Assert.That(found, Is.Not.Null);
            Assert.That(found.Implementation, Is.Not.Null);

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IBinder)
                     select reg).FirstOrDefault();

            Assert.That(found, Is.Not.Null);
            Assert.That(found.Implementation, Is.Not.Null);

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IWindowManager)
                     select reg).FirstOrDefault();

            Assert.That(found, Is.Not.Null);
            Assert.That(found.Implementation, Is.Not.Null);
        }

        [Test]
        public void can_provide_a_custom_routed_message_handler()
        {
            _config.Using(x => x.RoutedMessageController<FakeRoutedMessageController>());

            var registrations = _module.GetComponents();

            var found = (from reg in registrations.OfType<Singleton>()
                         where reg.Service == typeof(IRoutedMessageController)
                         select reg).FirstOrDefault();

            Assert.That(found.Implementation, Is.EqualTo(typeof(FakeRoutedMessageController)));
        }

        [Test]
        public void can_provide_a_custom_method_binder()
        {
            _config.Using(x => x.MessageBinder<FakeMessageBinder>());

            var registrations = _module.GetComponents();

            var found = (from reg in registrations.OfType<Singleton>()
                         where reg.Service == typeof(IMessageBinder)
                         select reg).FirstOrDefault();

            Assert.That(found.Implementation, Is.EqualTo(typeof(FakeMessageBinder)));
        }

        [Test]
        public void can_provide_a_custom_parser()
        {
            _config.Using(x => x.Parser<FakeMessageParser>());

            var registrations = _module.GetComponents();

            var found = (from reg in registrations.OfType<Singleton>()
                         where reg.Service == typeof(IParser)
                         select reg).FirstOrDefault();

            Assert.That(found.Implementation, Is.EqualTo(typeof(FakeMessageParser)));
        }
    }

    internal class FakeMessageBinder : IMessageBinder
    {
        public bool IsSpecialValue(string potential)
        {
            throw new NotImplementedException();
        }

        public void AddValueHandler(string specialValue, Func<IInteractionNode, object, object> handler)
        {
            throw new NotImplementedException();
        }

        public object[] DetermineParameters(IRoutedMessage message, IList<RequiredParameter> requiredParameters,
                                            IInteractionNode handlingNode, object context)
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