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
    using Rhino.Mocks;

    
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
            handler.Stub(x => x.Unwrap()).Return(methodHost);
            trigger = Stub<IMessageTrigger>();
            trigger.Message = Stub<IRoutedMessage>();
        }

        class TheMethodHost : PropertyChangedBase {}

        [Fact]
        public void can_initialize_DependencyObserver()
        {
            var metadata = new List<object>();

            attribute = new DependenciesAttribute("AProperty", "A.Property.Path");
            handler.Stub(x => x.Metadata).Return(metadata).Repeat.Any();
            attribute.Initialize(typeof(TheMethodHost), typeof(TheMethodHost), container);
            handler.Stub(x => x.Unwrap()).Return(methodHost).Repeat.Any();
            attribute.MakeAwareOf(handler);

            metadata.FirstOrDefaultOfType<DependencyObserver>().ShouldNotBeNull();

            attribute.MakeAwareOf(handler, trigger);
            //TODO: assert DependencyObserver.MakeAwareOf(IMessageTrigger trigger, IEnumerable<string> dependencies)
        }
    }
}