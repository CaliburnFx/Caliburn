using System;
using System.Reflection;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Tests.Caliburn.Core.Threading
{
    using System.Threading;
    using global::Caliburn.Core.Invocation;

    
    public class A_background_task : TestBase
    {
        [Fact]
        public async Task can_queue_work()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            var task = new BackgroundTask(() =>{
                taskCompletionSource.SetResult(true);
                return null;
            });

            task.Start(null);

            await Task.WhenAny(taskCompletionSource.Task, Task.Delay(250));
            taskCompletionSource.Task.Result.ShouldBeTrue();
        }

        [Fact]
        public void sets_up_context_when_executing_queued_work()
        {
            var task = new BackgroundTask(
                () =>{
                    BackgroundTask.CurrentContext.ShouldNotBeNull();
                    return true;
                });

            task.Start(null);
        }

        [Fact]
        public void tears_down_context_after_executing()
        {
            var task = new BackgroundTask(() => true);
            task.Start(null);

            BackgroundTask.CurrentContext.ShouldBeNull();
        }

        [Fact]
        public void is_not_busy_before_start()
        {
            var task = new BackgroundTask(() => null);

            task.IsBusy.ShouldBeFalse();
        }

        [Fact]
        public void is_busy_during_work()
        {
            BackgroundTask task = null;

            task = new BackgroundTask(
                () =>
                {
                    task.IsBusy.ShouldBeTrue();
                    return true;
                });

            task.Start(null);
        }

        [Fact]
        public void is_not_busy_after_work()
        {
            var task = new BackgroundTask(() => null);

            task.IsBusy.ShouldBeFalse();
        }

        [Fact]
        public void is_not_cancelled_at_start()
        {
            var task = new BackgroundTask(() => null);

            task.CancellationPending.ShouldBeFalse();
        }

        [Fact]
        public void when_cancelled_before_start_will_not_execute_the_delegate()
        {
            bool wasExecuted = false;

            var task = new BackgroundTask(() => wasExecuted = true);
            task.Cancel();
            task.Start(null);

            wasExecuted.ShouldBeFalse();
        }

        [Fact]
        public void when_cancelled_during_execution_will_update_context()
        {
            BackgroundTask task = null;
            
            task = new BackgroundTask(
                () => {
                    task.Cancel();
                    BackgroundTask.CurrentContext.CancellationPending.ShouldBeTrue();
                    task.CancellationPending.ShouldBeTrue();
                    return null;
                });

            task.Start(null);
        }

        [Fact]
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
                    s.ShouldBe(task);
                    e.ProgressPercentage.ShouldBe(percentage);

                    handle.Set();
                };

            task.Start(userState);

            handle.WaitOne(1000);
            wasFired.ShouldBeTrue();
        }

        [Fact]
        public void fires_event_when_work_is_complete()
        {
            object userState = new object();
            bool wasFired = false;

            var handle = new ManualResetEvent(false);
            var task = new BackgroundTask(() => null);

            task.Completed +=
                (s, e) => {
                    wasFired = true;
                    e.Result.ShouldBeNull();
                    e.Error.ShouldBeNull();
                    e.Cancelled.ShouldBeFalse();

                    handle.Set();
                };

            task.Start(userState);

            handle.WaitOne(1000);
            wasFired.ShouldBeTrue();
        }

        [Fact]
        public async Task fires_complete_event_when_exception_occurs()
        {
            object userState = new object();
            var exception = new Exception();

            var tcs = new TaskCompletionSource<bool>();

            var task = new BackgroundTask(
                () => {
                    throw exception;
                });

            task.Completed +=
                (s, e) =>
                {
                    Action resultAction = () =>
                    {
                        var result = e.Result;
                    };
                    resultAction.ShouldThrow<TargetInvocationException>();
                    e.Error.ShouldBeSameAs(exception);
                    e.Cancelled.ShouldBeFalse();

                    tcs.SetResult(true);
                };

            task.Start(userState);

            await Task.WhenAny(tcs.Task, Task.Delay(500));
            tcs.Task.IsCompleted.ShouldBeTrue();
            tcs.Task.Result.ShouldBeTrue();
        }

        [Fact]
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
                    e.Result.ShouldBe(result);
                    e.Error.ShouldBeNull();
                    e.Cancelled.ShouldBeFalse();

                    handle.Set();
                };

            task.Start(userState);

            handle.WaitOne(1000);
            wasFired.ShouldBeTrue();
        }

        [Fact]
        public void fires_complete_event_when_work_is_cancelled()
        {
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
                    s.ShouldBe(task);
                    Action resultAction = () =>
                    {
                        var result = e.Result;
                    };
                    resultAction.ShouldThrow<Exception>();
                    e.Error.ShouldBeNull();
                    e.Cancelled.ShouldBeTrue();

                    handle.Set();
                };

            task.Start(null);

            handle.WaitOne(1000);
            wasFired.ShouldBeTrue();
        }
    }
}