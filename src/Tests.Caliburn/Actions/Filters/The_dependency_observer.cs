using Shouldly;

namespace Tests.Caliburn.Actions.Filters
{
    using System;
    using System.Collections.Generic;
    using global::Caliburn.Core;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Filters;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using Xunit;
    using Rhino.Mocks;

    
    public class The_dependency_observer : TestBase
    {
        IRoutedMessageHandler handler;
        TheNotifierClass notifier;
        DependencyObserver observer;
        IMessageTrigger trigger;
        bool expectationsWasSet;

        protected override void given_the_context_of()
        {
            expectationsWasSet = false;

            var methodFactory = new DefaultMethodFactory();

            handler = StrictMock<IRoutedMessageHandler>();
            notifier = new TheNotifierClass();
            observer = new DependencyObserver(handler, methodFactory, notifier);
            trigger = Mock<IMessageTrigger>();
        }

        void ConfigureObserver(IEnumerable<string> dependencies)
        {
            observer.MakeAwareOf(trigger, dependencies);
        }

        void ExpectTriggerUpdate(int count)
        {
            handler.Expect(x => x.UpdateAvailability(trigger)).Repeat.Times(count);
            expectationsWasSet = true;
        }

        void AssertTriggerUpdateExpectations()
        {
            if(!expectationsWasSet)
                throw new Exception("Expectations was not set");
            handler.VerifyAllExpectations();
        }

        internal class TheNotifierClass : PropertyChangedBase
        {
            TheReferencedClass _anotherInstance = new TheReferencedClass();
            TheReferencedClass _instance = new TheReferencedClass();

            public int SomeProperty
            {
                get { return 0; }
            }

            public string SomeOtherProperty
            {
                get { return string.Empty; }
            }

            public TheReferencedClass Model
            {
                get { return _instance; }
                set
                {
                    _instance = value;
                    NotifyOfPropertyChange("Model");
                }
            }

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
            public int SubscriptionCount = 0;

            //public override event System.ComponentModel.PropertyChangedEventHandler PropertyChanged
            //{
            //    add { base.PropertyChanged += value; SubscriptionCount++; }
            //    remove { SubscriptionCount--; base.PropertyChanged -= value; }
            //}


            public int SomeModelProperty
            {
                get { return 0; }
            }

            public int AnotherModelProperty
            {
                get { return 0; }
            }
        }

        [Fact]
        //ref http://caliburn.codeplex.com/WorkItem/View.aspx?WorkItemId=6100
        //see also http://caliburn.codeplex.com/Thread/View.aspx?ThreadId=212171
        public void backreferences_should_not_leak_the_observer()
        {
            var handlerRef = new WeakReference(handler);

            //this reference emulates a back pointer to a long-living model
            var parent = notifier.Model;


            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });

            //emulates the collection of the cluster composed by Screen, View, MessageHandler and ancillary filters 
            //(included Dependecies along with its internal PropertyPathMonitor)
            observer = null;
            handler = null;
            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();


            handlerRef.IsAlive.ShouldBeFalse();


            //the first time a ppMonitor is notified AFTER the collection of its DependenyObserver,
            //it unregisters the unnecessary handler
            parent.NotifyOfPropertyChange("anyProperty");

            parent.SubscriptionCount.ShouldBe(0);
        }


        [Fact(Skip="NOTE: to make this test pass, the finalizer of DependencyObserver should be in place")]
        //see http://caliburn.codeplex.com/Thread/View.aspx?ThreadId=212171 for the rationale behind the finalizer removal
        public void backreferences_should_not_leak_the_observer_strict()
        {
            var handlerRef = new WeakReference(handler);
            var parent = notifier.Model;

            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });

            observer = null;
            handler = null;
            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();

            handlerRef.IsAlive.ShouldBeFalse();
            parent.SubscriptionCount.ShouldBe(0);
        }

        [Fact(Skip = "NOTE: to make this test pass, the finalizer of DependencyObserver should be in place")]
        //see http://caliburn.codeplex.com/Thread/View.aspx?ThreadId=212171 for the rationale behind the finalizer removal
        public void should_allow_nodes_collection()
        {
            ExpectTriggerUpdate(1); //strict mock requires expectations

            var disconnectedChainRef = new WeakReference(notifier.Model);

            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            notifier.Model = new TheReferencedClass();

            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();

            disconnectedChainRef.IsAlive.ShouldBeFalse();
        }

        [Fact]
        public void should_detect_changes_on_intermediate_node()
        {
            ExpectTriggerUpdate(1);
            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            notifier.Model = new TheReferencedClass();
            AssertTriggerUpdateExpectations();
        }

        [Fact]
        public void should_detect_registered_changes_on_referenced_model()
        {
            ExpectTriggerUpdate(1);

            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Fact]
        public void should_detect_registered_changes_on_target()
        {
            ExpectTriggerUpdate(1);

            ConfigureObserver(new[] {
                "SomeProperty"
            });
            notifier.NotifyOfPropertyChange("SomeProperty");

            AssertTriggerUpdateExpectations();
        }

        [Fact]
        public void should_detect_star_changes_on_leaf_node()
        {
            ExpectTriggerUpdate(2);

            ConfigureObserver(new[] {
                "Model.*"
            });
            notifier.Model.NotifyOfPropertyChange("SomeModelProperty");
            notifier.Model.NotifyOfPropertyChange("AnotherModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Fact]
        public void should_detect_star_changes_on_root()
        {
            ExpectTriggerUpdate(2);

            ConfigureObserver(new[] {
                "*"
            });
            notifier.NotifyOfPropertyChange("SomeProperty");
            notifier.NotifyOfPropertyChange("SomeOtherProperty");

            AssertTriggerUpdateExpectations();
        }

        [Fact]
        public void should_ignore_changes_on_deeper_path()
        {
            ExpectTriggerUpdate(0);

            ConfigureObserver(new[] {
                "Model"
            });
            notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Fact]
        public void should_ignore_changes_on_disconnected_chains()
        {
            ExpectTriggerUpdate(1); //first call is expected, second it's not

            var disconnectedChain = notifier.Model;

            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            notifier.Model = new TheReferencedClass();
            disconnectedChain.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Fact]
        public void should_ignore_changes_on_unregistered_path()
        {
            ExpectTriggerUpdate(0);

            ConfigureObserver(new[] {
                "AnotherModel.SomeModelProperty"
            });
            notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Fact]
        public void should_ignore_unregistered_changes_on_target()
        {
            ExpectTriggerUpdate(0);

            ConfigureObserver(new[] {
                "SomeProperty"
            });
            notifier.NotifyOfPropertyChange("SomeOtherProperty");

            AssertTriggerUpdateExpectations();
        }

        [Fact]
        public void should_monitor_multiple_paths()
        {
            ExpectTriggerUpdate(2);

            ConfigureObserver(new[] {
                "Model.*", "AnotherModel.SomeModelProperty"
            });
            notifier.Model.NotifyOfPropertyChange("AnotherModelProperty");
            notifier.AnotherModel.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Fact]
        public void should_reconnect_monitor_on_changed_chain()
        {
            ExpectTriggerUpdate(2);

            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            notifier.Model = new TheReferencedClass();
            notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Fact]
        public void should_reconnect_monitor_on_previously_null_nodes()
        {
            ExpectTriggerUpdate(2);

            notifier.Model = null;

            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            notifier.Model = new TheReferencedClass();

            notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            AssertTriggerUpdateExpectations();
        }

        [Fact]
        public void should_throw_exception_if_property_dose_not_exists_on_target()
        {
            Assert.Throws<CaliburnException>(() =>{
                ConfigureObserver(new[] {
                    "NotExistingProperty"
                });
            });
        }

        [Fact]
        public void should_throw_on_star_invalid_use()
        {
            var exception = Assert.Throws<CaliburnException>(() =>{
                ConfigureObserver(new[] {
                    "*.xxx"
                });
            });
            exception.Message.Contains("'*' marker in path").ShouldBeTrue();
        }
    }
}