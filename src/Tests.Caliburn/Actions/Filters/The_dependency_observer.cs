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
    using NSubstitute;


    public class The_dependency_observer : TestBase
    {
        IRoutedMessageHandler handler;
        TheNotifierClass notifier;
        DependencyObserver observer;
        IMessageTrigger trigger;

        protected override void given_the_context_of()
        {
            var methodFactory = new DefaultMethodFactory();

            handler = Mock<IRoutedMessageHandler>();
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
            handler.Received(count).UpdateAvailability(trigger);
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
            var disconnectedChainRef = new WeakReference(notifier.Model);

            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            notifier.Model = new TheReferencedClass();

            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();

            ExpectTriggerUpdate(1);
            disconnectedChainRef.IsAlive.ShouldBeFalse();
        }

        [Fact]
        public void should_detect_changes_on_intermediate_node()
        {
            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            notifier.Model = new TheReferencedClass();
            ExpectTriggerUpdate(1);
        }

        [Fact]
        public void should_detect_registered_changes_on_referenced_model()
        {
            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            ExpectTriggerUpdate(1);
        }

        [Fact]
        public void should_detect_registered_changes_on_target()
        {
            ConfigureObserver(new[] {
                "SomeProperty"
            });
            notifier.NotifyOfPropertyChange("SomeProperty");

            ExpectTriggerUpdate(1);
        }

        [Fact]
        public void should_detect_star_changes_on_leaf_node()
        {
            ConfigureObserver(new[] {
                "Model.*"
            });
            notifier.Model.NotifyOfPropertyChange("SomeModelProperty");
            notifier.Model.NotifyOfPropertyChange("AnotherModelProperty");

            ExpectTriggerUpdate(2);
        }

        [Fact]
        public void should_detect_star_changes_on_root()
        {
            ConfigureObserver(new[] {
                "*"
            });
            notifier.NotifyOfPropertyChange("SomeProperty");
            notifier.NotifyOfPropertyChange("SomeOtherProperty");

            ExpectTriggerUpdate(2);
        }

        [Fact]
        public void should_ignore_changes_on_deeper_path()
        {
            ConfigureObserver(new[] {
                "Model"
            });
            notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            ExpectTriggerUpdate(0);
        }

        [Fact]
        public void should_ignore_changes_on_disconnected_chains()
        {
            var disconnectedChain = notifier.Model;

            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            notifier.Model = new TheReferencedClass();
            disconnectedChain.NotifyOfPropertyChange("SomeModelProperty");

            ExpectTriggerUpdate(1); //first call is expected, second it's not
        }

        [Fact]
        public void should_ignore_changes_on_unregistered_path()
        {
            ConfigureObserver(new[] {
                "AnotherModel.SomeModelProperty"
            });
            notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            ExpectTriggerUpdate(0);
        }

        [Fact]
        public void should_ignore_unregistered_changes_on_target()
        {
            ConfigureObserver(new[] {
                "SomeProperty"
            });
            notifier.NotifyOfPropertyChange("SomeOtherProperty");

            ExpectTriggerUpdate(0);
        }

        [Fact]
        public void should_monitor_multiple_paths()
        {
            ConfigureObserver(new[] {
                "Model.*", "AnotherModel.SomeModelProperty"
            });
            notifier.Model.NotifyOfPropertyChange("AnotherModelProperty");
            notifier.AnotherModel.NotifyOfPropertyChange("SomeModelProperty");

            ExpectTriggerUpdate(2);
        }

        [Fact]
        public void should_reconnect_monitor_on_changed_chain()
        {
            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            notifier.Model = new TheReferencedClass();
            notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            ExpectTriggerUpdate(2);
        }

        [Fact]
        public void should_reconnect_monitor_on_previously_null_nodes()
        {
            notifier.Model = null;

            ConfigureObserver(new[] {
                "Model.SomeModelProperty"
            });
            notifier.Model = new TheReferencedClass();

            notifier.Model.NotifyOfPropertyChange("SomeModelProperty");

            ExpectTriggerUpdate(2);
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
