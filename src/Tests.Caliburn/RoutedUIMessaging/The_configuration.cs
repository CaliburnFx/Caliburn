using Shouldly;

namespace Tests.Caliburn.RoutedUIMessaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using global::Caliburn.Core.Configuration;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.PresentationFramework.Actions;
    using global::Caliburn.PresentationFramework.ApplicationModel;
    using global::Caliburn.PresentationFramework.Configuration;
    using global::Caliburn.PresentationFramework.Conventions;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using global::Caliburn.PresentationFramework.RoutedMessaging.Parsers;
    using global::Caliburn.PresentationFramework.ViewModels;
    using global::Caliburn.PresentationFramework.Views;
    using Xunit;

    
    public class The_configuration : TestBase
    {
        PresentationFrameworkConfiguration config;
        IModule module;

        protected override void given_the_context_of()
        {
            config = CaliburnModule<PresentationFrameworkConfiguration>.Instance;
            module = config;
        }

        [Fact]
        public void can_provide_a_custom_method_binder()
        {
            config.Using(x => x.MessageBinder<FakeMessageBinder>());

            var registrations = module.GetComponents();

            var found = (from reg in registrations.OfType<Singleton>()
                         where reg.Service == typeof(IMessageBinder)
                         select reg).First();

            found.Implementation.ShouldBe(typeof(FakeMessageBinder));
        }

        [Fact]
        public void can_provide_a_custom_parser()
        {
            config.Using(x => x.Parser<FakeMessageParser>());

            var registrations = module.GetComponents();

            var found = (from reg in registrations.OfType<Singleton>()
                         where reg.Service == typeof(IParser)
                         select reg).First();

            found.Implementation.ShouldBe(typeof(FakeMessageParser));
        }

        [Fact]
        public void can_provide_a_custom_routed_message_handler()
        {
            config.Using(x => x.RoutedMessageController<FakeRoutedMessageController>());

            var registrations = module.GetComponents();

            var found = (from reg in registrations.OfType<Singleton>()
                         where reg.Service == typeof(IRoutedMessageController)
                         select reg).First();

            found.Implementation.ShouldBe(typeof(FakeRoutedMessageController));
        }

        [Fact]
        public void when_started_configures_required_components_and_defaults()
        {
            var registrations = module.GetComponents();

            var found = (from reg in registrations.OfType<Singleton>()
                         where reg.Service == typeof(IRoutedMessageController)
                         select reg).First();

            found.Implementation.ShouldNotBeNull();

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IMessageBinder)
                     select reg).First();

            found.Implementation.ShouldNotBeNull();

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IParser)
                     select reg).First();

            found.Implementation.ShouldNotBeNull();

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IViewModelDescriptionFactory)
                     select reg).First();

            found.Implementation.ShouldNotBeNull();

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IActionLocator)
                     select reg).First();

            found.Implementation.ShouldNotBeNull();

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IViewLocator)
                     select reg).First();

            found.Implementation.ShouldNotBeNull();

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IViewModelBinder)
                     select reg).First();

            found.Implementation.ShouldNotBeNull();

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IConventionManager)
                     select reg).First();

            found.Implementation.ShouldNotBeNull();

            found = (from reg in registrations.OfType<Singleton>()
                     where reg.Service == typeof(IWindowManager)
                     select reg).First();

            found.Implementation.ShouldNotBeNull();
        }
    }

    class FakeMessageBinder : IMessageBinder
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

    class FakeRoutedMessageController : IRoutedMessageController
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
    }

    class FakeMessageParser : IParser
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