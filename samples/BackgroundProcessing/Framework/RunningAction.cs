namespace BackgroundProcessing.Framework {
    using System;
    using Caliburn.Core.Invocation;
    using Caliburn.PresentationFramework;

    public class RunningAction : PropertyChangedBase, IRunningAction {
        [ThreadStatic]
        static IRunningAction current;

        readonly IBackgroundTask bgTask;
        readonly bool isCancellable;
        readonly bool isIndeterminate;
        double currentPercentage;
        string title;
        bool updatingProgressFromInnerIBackgroundTask;

        public RunningAction(IBackgroundTask bgTask, bool isIndeterminate, bool isCancellable) {
            this.isIndeterminate = isIndeterminate;
            this.isCancellable = isCancellable;
            this.bgTask = bgTask;

            //receive progress notification from implementor of IBackgroundTask to track progress
            //also if not reported using RunningAction class
            this.bgTask.ProgressChanged += (o, e) => {
                updatingProgressFromInnerIBackgroundTask = true;
                try {
                    CurrentPercentage = e.ProgressPercentage / 100.0;
                }
                finally {
                    updatingProgressFromInnerIBackgroundTask = false;
                }
            };

            this.bgTask.Starting += (o, e) => Current = this;
            this.bgTask.Completed += (o, e) => Current = null;
        }

        public static IRunningAction Current {
            get { return current; }
            internal set { current = value; }
        }

        public bool IsIndeterminate {
            get { return isIndeterminate; }
        }

        public bool IsCancellable {
            get { return isCancellable; }
        }

        public string Title {
            get { return title; }
            set {
                title = value;
                NotifyOfPropertyChange("Title");
            }
        }

        public double CurrentPercentage {
            get { return currentPercentage; }
            set {
                currentPercentage = value;
                NotifyOfPropertyChange("CurrentPercentage");

                //notify change to other listeners of IBackgroundTask.ProgressChanged
                //only if progress change is NOT originated from another notification
                if(!updatingProgressFromInnerIBackgroundTask)
                    BackgroundTask.CurrentContext.ReportProgress((int)(value * 100));
            }
        }

        public bool CancellationPending {
            get { return bgTask.CancellationPending; }
        }

        public void Cancel() {
            bgTask.Cancel();
        }

        public static void UpdateProgress(int percentage) {
            BackgroundTask.CurrentContext.ReportProgress(percentage);
        }
    }
}