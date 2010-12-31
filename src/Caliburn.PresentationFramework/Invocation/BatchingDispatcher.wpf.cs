#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.Invocation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Windows.Threading;
    using Core.Invocation;

    /// <summary>
    /// An impelementation of <see cref="IDispatcher"/> that efficiently batches updates to the UI thread.
    /// </summary>
    public class BatchingDispatcher : IDispatcher
    {
        readonly Dispatcher dispatcher;
        readonly List<Action> queuedActions = new List<Action>();
        readonly object locker = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchingDispatcher"/> class.
        /// </summary>
        public BatchingDispatcher()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            DefaultPriority = DispatcherPriority.Normal;

            new Thread(SendBatchOfUpdates)
            {
                IsBackground = true
            }.Start();
        }

        /// <summary>
        /// Gets or sets the default dispatcher priority.
        /// </summary>
        /// <value>The default priority.</value>
        public DispatcherPriority DefaultPriority { get; set; }

        /// <summary>
        /// Executes code on the background thread.
        /// </summary>
        /// <param name="backgroundAction">The background action.</param>
        /// <param name="uiCallback">The UI callback.</param>
        /// <param name="progressChanged">The progress change callback.</param>
        public IBackgroundTask ExecuteOnBackgroundThread(Action backgroundAction, RunWorkerCompletedEventHandler uiCallback, ProgressChangedEventHandler progressChanged)
        {
            var task = new BackgroundTask(
                () =>{
                    backgroundAction();
                    return null;
                });

            if(uiCallback != null)
                task.Completed += (s, e) => ExecuteOnUIThread(() => uiCallback(s, e));

            if(progressChanged != null)
                task.ProgressChanged += (s, e) => ExecuteOnUIThread(() => progressChanged(s, e));

            task.Start(null);

            return task;
        }

        /// <summary>
        /// Executes code on the UI thread.
        /// </summary>
        /// <param name="uiAction">The UI action.</param>
        public void ExecuteOnUIThread(Action uiAction)
        {
            if (dispatcher.CheckAccess())
                uiAction();
            else
            {
                lock (locker)
                    queuedActions.Add(uiAction);
                return;
            }
        }

        /// <summary>
        /// Executes code on the UI thread asynchronously.
        /// </summary>
        /// <param name="uiAction">The UI action.</param>
        public IDispatcherOperation BeginExecuteOnUIThread(Action uiAction)
        {
            var operation = dispatcher.BeginInvoke(
                uiAction,
                DefaultPriority
                );

            return new DispatcherOperationProxy(operation);
        }

        void SendBatchOfUpdates()
        {
            while (true)
            {
                Action[] actions;
                lock (locker)
                {
                    actions = queuedActions
                        .Take(100)
                        .ToArray();

                    for (int i = 0; i < actions.Length; i++)
                    {
                        queuedActions.RemoveAt(0);
                    }
                }

                if (actions.Length == 0)
                {
                    Thread.Sleep(500);
                    continue;
                }

                dispatcher.Invoke(
                    (Action)delegate
                {
                    using (dispatcher.DisableProcessing())
                    {
                        foreach (var action in actions)
                        {
                            action();
                        }
                    }
                });
            }
        }
    }
}

#endif