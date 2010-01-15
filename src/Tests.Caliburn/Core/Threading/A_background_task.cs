using System;
using Caliburn.Core.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Tests.Caliburn.Fakes;

namespace Tests.Caliburn.Core.Threading
{
    [TestFixture]
    public class A_background_task : TestBase
    {
        private IThreadPool _threadPool;

        protected override void given_the_context_of()
        {
            _threadPool = new FakeThreadPool();
        }

        [Test]
        public void can_queue_work()
        {
            bool wasExecuted = false;

            var task = new BackgroundTask(_threadPool, () => wasExecuted = true);
            task.Enqueue(null);

            Assert.That(wasExecuted, Is.True);
        }

        [Test]
        public void sets_up_context_when_executing_queued_work()
        {
            var task = new BackgroundTask(
                _threadPool,
                () =>
                {
                    Assert.That(BackgroundTask.CurrentContext, Is.Not.Null);
                    return true;
                });

            task.Enqueue(null);
        }

        [Test]
        public void tears_down_context_after_executing()
        {
            var task = new BackgroundTask(_threadPool, () => true);
            task.Enqueue(null);

            Assert.That(BackgroundTask.CurrentContext, Is.Null);
        }

        [Test]
        public void is_not_busy_before_start()
        {
            var task = new BackgroundTask(_threadPool, () => null);

            Assert.That(task.IsBusy, Is.False);
        }

        [Test]
        public void is_busy_during_work()
        {
            BackgroundTask task = null;

            task = new BackgroundTask(
                _threadPool,
                () =>
                {
                    Assert.That(task.IsBusy, Is.True);
                    return true;
                });

            task.Enqueue(null);
        }

        [Test]
        public void is_not_busy_after_work()
        {
            var task = new BackgroundTask(_threadPool, () => null);

            Assert.That(task.IsBusy, Is.False);
        }

        [Test]
        public void is_not_cancelled_at_start()
        {
            var task = new BackgroundTask(_threadPool, () => null);

            Assert.That(task.CancellationPending, Is.False);
        }

        [Test]
        public void when_cancelled_before_start_will_not_execute_the_delegate()
        {
            bool wasExecuted = false;

            var task = new BackgroundTask(_threadPool, () => wasExecuted = true);
            task.Cancel();
            task.Enqueue(null);

            Assert.That(wasExecuted, Is.False);
        }

        [Test]
        public void when_cancelled_during_execution_will_update_context()
        {
            BackgroundTask task = null;
            
            task = new BackgroundTask(
                _threadPool, 
                () => {
                    task.Cancel();
                    Assert.That(BackgroundTask.CurrentContext.IsCancelled, Is.True);
                    Assert.That(task.CancellationPending, Is.True);
                    return null;
                });

            task.Enqueue(null);
        }

        [Test]
        public void fires_event_when_progress_is_updated()
        {
            const double percentage = .2;
            object userState = new object();
            bool wasFired = false;

            var task = new BackgroundTask(
                _threadPool,
                () => {
                    BackgroundTask.CurrentContext.UpdateProgress(percentage);
                    return null;
                });

            task.ProgressChanged +=
                (s, e) => {
                    wasFired = true;
                    Assert.That(s, Is.EqualTo(task));
                    Assert.That(e.Percentage, Is.EqualTo(percentage));
                    Assert.That(e.UserState, Is.EqualTo(userState));
                };

            task.Enqueue(userState);

            Assert.That(wasFired, Is.True);
        }

        [Test]
        public void fires_event_when_work_is_complete()
        {
            object userState = new object();
            bool wasFired = false;

            var task = new BackgroundTask(_threadPool, () => null);

            task.Completed +=
                (s, e) => {
                    wasFired = true;
                    Assert.That(s, Is.EqualTo(task));
                    Assert.That(e.UserState, Is.EqualTo(userState));
                    Assert.That(e.Result, Is.Null);
                    Assert.That(e.Error, Is.Null);
                    Assert.That(e.Cancelled, Is.False);
                };

            task.Enqueue(userState);

            Assert.That(wasFired, Is.True);
        }

        [Test]
        public void fires_complete_event_when_exception_occurs()
        {
            object userState = new object();
            var exception = new Exception();
            bool wasFired = false;

            var task = new BackgroundTask(
                _threadPool, 
                () => {
                    throw exception;
                });

            task.Completed +=
                (s, e) =>
                {
                    wasFired = true;
                    Assert.That(s, Is.EqualTo(task));
                    Assert.That(e.UserState, Is.EqualTo(userState));
                    Assert.That(e.Result, Is.Null);
                    Assert.That(e.Error, Is.EqualTo(exception));
                    Assert.That(e.Cancelled, Is.False);
                };

            task.Enqueue(userState);

            Assert.That(wasFired, Is.True);
        }

        [Test]
        public void fires_complete_event_with_result_when_available()
        {
            object userState = new object();
            bool wasFired = false;
            const int result = 8;

            var task = new BackgroundTask(_threadPool, () => result);

            task.Completed +=
                (s, e) =>
                {
                    wasFired = true;
                    Assert.That(s, Is.EqualTo(task));
                    Assert.That(e.UserState, Is.EqualTo(userState));
                    Assert.That(e.Result, Is.EqualTo(result));
                    Assert.That(e.Error, Is.Null);
                    Assert.That(e.Cancelled, Is.False);
                };

            task.Enqueue(userState);

            Assert.That(wasFired, Is.True);
        }

        [Test]
        public void fires_complete_event_when_work_is_cancelled()
        {
            object userState = new object();
            bool wasFired = false;

            BackgroundTask task = null;
            
            task = new BackgroundTask(
                _threadPool, 
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
                };

            task.Enqueue(userState);

            Assert.That(wasFired, Is.True);
        }
    }
}