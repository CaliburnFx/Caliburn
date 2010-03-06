using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Tests.Caliburn.Core.Threading
{
    using System.Threading;
    using global::Caliburn.Core.Invocation;

    [TestFixture]
    [Ignore]
    public class A_background_task : TestBase
    {
        [Test]
        public void can_queue_work()
        {
            bool wasExecuted = false;
            var handle = new ManualResetEvent(false);

            var task = new BackgroundTask(() =>{
                wasExecuted = true;
                handle.Set();
                return null;
            });

            task.Start(null);

            handle.WaitOne(1000);
            Assert.That(wasExecuted, Is.True);
        }

        [Test]
        public void sets_up_context_when_executing_queued_work()
        {
            var task = new BackgroundTask(
                () =>{
                    Assert.That(BackgroundTask.CurrentContext, Is.Not.Null);
                    return true;
                });

            task.Start(null);
        }

        [Test]
        public void tears_down_context_after_executing()
        {
            var task = new BackgroundTask(() => true);
            task.Start(null);

            Assert.That(BackgroundTask.CurrentContext, Is.Null);
        }

        [Test]
        public void is_not_busy_before_start()
        {
            var task = new BackgroundTask(() => null);

            Assert.That(task.IsBusy, Is.False);
        }

        [Test]
        public void is_busy_during_work()
        {
            BackgroundTask task = null;

            task = new BackgroundTask(
                () =>
                {
                    Assert.That(task.IsBusy, Is.True);
                    return true;
                });

            task.Start(null);
        }

        [Test]
        public void is_not_busy_after_work()
        {
            var task = new BackgroundTask(() => null);

            Assert.That(task.IsBusy, Is.False);
        }

        [Test]
        public void is_not_cancelled_at_start()
        {
            var task = new BackgroundTask(() => null);

            Assert.That(task.CancellationPending, Is.False);
        }

        [Test]
        public void when_cancelled_before_start_will_not_execute_the_delegate()
        {
            bool wasExecuted = false;

            var task = new BackgroundTask(() => wasExecuted = true);
            task.Cancel();
            task.Start(null);

            Assert.That(wasExecuted, Is.False);
        }

        [Test]
        public void when_cancelled_during_execution_will_update_context()
        {
            BackgroundTask task = null;
            
            task = new BackgroundTask(
                () => {
                    task.Cancel();
                    Assert.That(BackgroundTask.CurrentContext.CancellationPending, Is.True);
                    Assert.That(task.CancellationPending, Is.True);
                    return null;
                });

            task.Start(null);
        }

        [Test]
        public void fires_event_when_progress_is_updated()
        {
            const int percentage = 20;
            object userState = new object();
            bool wasFired = false;
            var handle = new ManualResetEvent(false);

            var task = new BackgroundTask(
                () => {
                    BackgroundTask.CurrentContext.ReportProgress(percentage);
                    return null;
                });

            task.ProgressChanged +=
                (s, e) => {
                    wasFired = true;
                    Assert.That(s, Is.EqualTo(task));
                    Assert.That(e.ProgressPercentage, Is.EqualTo(percentage));

                    handle.Set();
                };

            task.Start(userState);

            handle.WaitOne(1000);
            Assert.That(wasFired, Is.True);
        }

        [Test]
        public void fires_event_when_work_is_complete()
        {
            object userState = new object();
            bool wasFired = false;

            var handle = new ManualResetEvent(false);
            var task = new BackgroundTask(() => null);

            task.Completed +=
                (s, e) => {
                    wasFired = true;
                    Assert.That(e.Result, Is.Null);
                    Assert.That(e.Error, Is.Null);
                    Assert.That(e.Cancelled, Is.False);

                    handle.Set();
                };

            task.Start(userState);

            handle.WaitOne(1000);
            Assert.That(wasFired, Is.True);
        }

        [Test]
        public void fires_complete_event_when_exception_occurs()
        {
            object userState = new object();
            var exception = new Exception();
            bool wasFired = false;

            var handle = new ManualResetEvent(false);

            var task = new BackgroundTask(
                () => {
                    throw exception;
                });

            task.Completed +=
                (s, e) =>
                {
                    wasFired = true;
                    Assert.That(e.Result, Is.Null);
                    Assert.That(e.Error, Is.EqualTo(exception));
                    Assert.That(e.Cancelled, Is.False);

                    handle.Set();
                };

            task.Start(userState);

            handle.WaitOne(1000);
            Assert.That(wasFired, Is.True);
        }

        [Test]
        public void fires_complete_event_with_result_when_available()
        {
            object userState = new object();
            bool wasFired = false;
            const int result = 8;

            var handle = new ManualResetEvent(false);
            var task = new BackgroundTask(() => result);

            task.Completed +=
                (s, e) =>
                {
                    wasFired = true;
                    Assert.That(e.Result, Is.EqualTo(result));
                    Assert.That(e.Error, Is.Null);
                    Assert.That(e.Cancelled, Is.False);

                    handle.Set();
                };

            task.Start(userState);

            handle.WaitOne(1000);
            Assert.That(wasFired, Is.True);
        }

        [Test]
        public void fires_complete_event_when_work_is_cancelled()
        {
            object userState = new object();
            bool wasFired = false;

            var handle = new ManualResetEvent(false);
            BackgroundTask task = null;
            
            task = new BackgroundTask(
                () => {
                    task.Cancel();
                    return null;
                });

            task.Completed +=
                (s, e) =>
                {
                    wasFired = true;
                    Assert.That(s, Is.EqualTo(task));
                    Assert.That(e.UserState, Is.EqualTo(userState));
                    Assert.That(e.Result, Is.Null);
                    Assert.That(e.Error, Is.Null);
                    Assert.That(e.Cancelled, Is.True);

                    handle.Set();
                };

            task.Start(userState);

            handle.WaitOne(1000);
            Assert.That(wasFired, Is.True);
        }
    }
}