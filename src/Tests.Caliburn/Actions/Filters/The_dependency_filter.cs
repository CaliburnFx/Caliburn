using Shouldly;

namespace Tests.Caliburn.Actions.Filters
{
    using System.Collections.Generic;
    using global::Caliburn.Core;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Filters;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using Xunit;
    using NSubstitute;


    public class The_dependency_filter : TestBase
    {
        IServiceLocator container;
        TheMethodHost methodHost;
        IRoutedMessageHandler handler;
        IMessageTrigger trigger;
        DependenciesAttribute attribute;

        protected override void given_the_context_of()
        {
            container = Mock<IServiceLocator>();
            methodHost = new TheMethodHost();
            handler = Mock<IRoutedMessageHandler>();
            handler.Unwrap().Returns(methodHost);
            trigger = Mock<IMessageTrigger>();
            trigger.Message = Mock<IRoutedMessage>();
        }

        class TheMethodHost : PropertyChangedBase {}

        [Fact]
        public void can_initialize_DependencyObserver()
        {
            var metadata = new List<object>();

            attribute = new DependenciesAttribute("AProperty", "A.Property.Path");
            handler.Metadata.Returns(metadata);
            attribute.Initialize(typeof(TheMethodHost), typeof(TheMethodHost), container);
            handler.Unwrap().Returns(methodHost);
            attribute.MakeAwareOf(handler);

            metadata.FirstOrDefaultOfType<DependencyObserver>().ShouldNotBeNull();

            attribute.MakeAwareOf(handler, trigger);
            //TODO: assert DependencyObserver.MakeAwareOf(IMessageTrigger trigger, IEnumerable<string> dependencies)
        }
    }
}
