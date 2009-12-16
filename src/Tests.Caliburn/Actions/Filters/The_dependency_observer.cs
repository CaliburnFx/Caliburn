namespace Tests.Caliburn.Actions.Filters
{
    using System;
    using System.Collections.Generic;
    using global::Caliburn.Core;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.Core.MemoryManagement;
    using global::Caliburn.Core.Threading;
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Filters;
    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class The_dependency_observer : TestBase
    {
        private IServiceLocator _container;
        private IRoutedMessageHandler _handler;
        private TheNotifierClass _notifier;
        private DependencyObserver _observer;
        private IMessageTrigger _trigger;
        private bool _expectationsWasSet;

        protected override void given_the_context_of()
        {
            _expectationsWasSet = false;

            var methodFactory = new DefaultMethodFactory(
                new DefaultThreadPool()
                );
            _container = Stub<IServiceLocator>();
            _container.Stub(x => x.GetInstance<IMethodFactory>()).Return(methodFactory).Repeat.Any();

            _handler = StrictMock<IRoutedMessageHandler>();
            _notifier = new TheNotifierClass();
            _observer = new DependencyObserver(_handler, _notifier, _container);
            _trigger = Mock<IMessageTrigger>();
        }

        private void ConfigureObserver(IEnumerable<string> dependencies)
        {
            _observer.MakeAwareOf(_trigger, dependencies);
        }

        private void ExpectTriggerUpdate(int count)
        {
            _handler.Expect(x => x.UpdateAvailability(_trigger)).Repeat.Times(count);
            _expectationsWasSet = true;
        }

        private void AssertTriggerUpdateExpectations()
        {
            if (!_expectationsWasSet) throw new Exception("Expectations was not set");
            _handler.VerifyAllExpectations();
        }

        [Test]
        public void should_detect_registered_changes_on_target()
        {
            ExpectTriggerUpdate(1);

            ConfigureObserver(new[] { "SomeProperty" });
            _notifier.NotifyOfPropertyChange("SomeProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_ignore_unregistered_changes_on_target()
        {
            ExpectTriggerUpdate(0);

            ConfigureObserver(new[] { "SomeProperty" });
            _notifier.NotifyOfPropertyChange("SomeOtherProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_detect_registered_changes_on_referenced_model()
        {
            ExpectTriggerUpdate(1);

            ConfigureObserver(new[] { "Model.SomeModelProperty" });
            _notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_ignore_changes_on_unregistered_path()
        {
            ExpectTriggerUpdate(0);

            ConfigureObserver(new[] { "AnotherModel.SomeModelProperty" });
            _notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_ignore_changes_on_deeper_path()
        {
            ExpectTriggerUpdate(0);

            ConfigureObserver(new[] { "Model" });
            _notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }


        [Test]
        public void should_detect_star_changes_on_root()
        {
            ExpectTriggerUpdate(2);

            ConfigureObserver(new[] { "*" });
            _notifier.NotifyOfPropertyChange("SomeProperty");
            _notifier.NotifyOfPropertyChange("SomeOtherProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_detect_star_changes_on_leaf_node()
        {
            ExpectTriggerUpdate(2);

            ConfigureObserver(new[] { "Model.*" });
            _notifier.Model.NotifyOfPropertyChange("SomeModelProperty");
            _notifier.Model.NotifyOfPropertyChange("AnotherModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        [Ignore]
        [ExpectedException(MatchType = MessageMatch.Contains, ExpectedMessage = "'*' marker in path")]
        public void should_throw_on_star_invalid_use()
        {
            ConfigureObserver(new[] { "*.xxx" });
        }

        [Test]
        public void should_detect_changes_on_intermediate_node()
        {
            ExpectTriggerUpdate(1);
            ConfigureObserver(new[] { "Model.SomeModelProperty" });
            _notifier.Model = new TheReferencedClass();
            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_reconnect_monitor_on_changed_chain()
        {
            ExpectTriggerUpdate(2);

            ConfigureObserver(new[] { "Model.SomeModelProperty" });
            _notifier.Model = new TheReferencedClass();
            _notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_reconnect_monitor_on_previously_null_nodes()
        {
            ExpectTriggerUpdate(2);

            _notifier.Model = null;

            ConfigureObserver(new[] { "Model.SomeModelProperty" });
            _notifier.Model = new TheReferencedClass();

            _notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_monitor_multiple_paths()
        {
            ExpectTriggerUpdate(2);

            ConfigureObserver(new[] { "Model.*", "AnotherModel.SomeModelProperty" });
            _notifier.Model.NotifyOfPropertyChange("AnotherModelProperty");
            _notifier.AnotherModel.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_ignore_changes_on_disconnected_chains()
        {
            ExpectTriggerUpdate(1); //first call is expected, second it's not

            var disconnectedChain = _notifier.Model;

            ConfigureObserver(new[] { "Model.SomeModelProperty" });
            _notifier.Model = new TheReferencedClass();
            disconnectedChain.NotifyOfPropertyChange("SomeModelProperty");


            AssertTriggerUpdateExpectations();
        }

        [Test]
        public void should_allow_nodes_collection()
        {
            ExpectTriggerUpdate(1); //strict mock requires expectations

            var disconnectedChainRef = new WeakReference<TheReferencedClass>(_notifier.Model);

            ConfigureObserver(new[] { "Model.SomeModelProperty" });
            _notifier.Model = new TheReferencedClass();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.That(disconnectedChainRef.IsAlive, Is.False);
        }

        internal class TheNotifierClass : PropertyChangedBase
        {
            public int SomeProperty
            {
                get { return 0; }
            }

            public string SomeOtherProperty
            {
                get { return string.Empty; }
            }

            private TheReferencedClass _instance = new TheReferencedClass();

            public TheReferencedClass Model
            {
                get { return _instance; }
                set
                {
                    _instance = value;
                    NotifyOfPropertyChange("Model");
                }
            }

            private TheReferencedClass _anotherInstance = new TheReferencedClass();

            public TheReferencedClass AnotherModel
            {
                get { return _anotherInstance; }
                set
                {
                    _anotherInstance = value;
                    NotifyOfPropertyChange("AnotherModel");
                }
            }
        }

        internal class TheReferencedClass : PropertyChangedBase
        {
            public int SomeModelProperty
            {
                get { return 0; }
            }

            public int AnotherModelProperty
            {
                get { return 0; }
            }
        }
    }
}