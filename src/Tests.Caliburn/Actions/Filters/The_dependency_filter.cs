namespace Tests.Caliburn.Actions.Filters
{
    using System.Collections.Generic;
    using global::Caliburn.Core.Metadata;
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Filters;
    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class The_dependency_filter : TestBase
    {
        private IServiceLocator _container;
        private TheMethodHost _methodHost;
        private IRoutedMessageHandler _handler;
        private IMessageTrigger _trigger;
        private DependenciesAttribute _attribute;

        protected override void given_the_context_of()
        {
            _container = Mock<IServiceLocator>();
            _methodHost = new TheMethodHost();
            _handler = Mock<IRoutedMessageHandler>();
            _handler.Stub(x => x.Unwrap()).Return(_methodHost);
            _trigger = Stub<IMessageTrigger>();
            _trigger.Message = Stub<IRoutedMessage>();
        }

        [Test]
        public void can_initialize_DependencyObserver()
        {
            _attribute = new DependenciesAttribute("AProperty", "A.Property.Path");
            _attribute.Initialize(typeof(TheMethodHost), _methodHost, _container);
            _handler.Stub(x => x.FindMetadata<DependencyObserver>()).Return(new List<DependencyObserver>());
            _attribute.MakeAwareOf(_handler);
            _handler.AssertWasCalled(x => x.AddMetadata(null), options => options.IgnoreArguments());

            var args = _handler.GetArgumentsForCallsMadeOn(x => x.AddMetadata(null));
            var observer = args[0][0] as DependencyObserver;
            Assert.IsNotNull(observer);

            _handler.Stub(x => x.FindMetadata<DependencyObserver>()).Return(new List<DependencyObserver> { observer });
            _attribute.MakeAwareOf(_handler, _trigger);
            //TODO: assert DependencyObserver.MakeAwareOf(IMessageTrigger trigger, IEnumerable<string> dependencies)
        }

        internal class TheMethodHost : MetadataContainer { }
    }
}