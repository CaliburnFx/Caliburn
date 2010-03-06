namespace BackgroundProcessing.Framework
{
    using System;
    using Caliburn.Core.Invocation;
    using Caliburn.PresentationFramework;

    public class RunningAction : PropertyChangedBase, IRunningAction
    {
        [ThreadStatic] private static IRunningAction _current;

        private readonly IBackgroundTask _bgTask;
        private readonly bool _isCancellable;
        private readonly bool _isIndeterminate;
        private bool _updatingProgressFromInnerIBackgroundTask;
        private string _title;
        private double _currentPercentage;

        public static IRunningAction Current
        {
            get { return _current; }
            internal set { _current = value; }
        }

        public bool IsIndeterminate
        {
            get { return _isIndeterminate; }
        }

        public bool IsCancellable
        {
            get { return _isCancellable; }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyOfPropertyChange("Title");
            }
        }

        public double CurrentPercentage
        {
            get { return _currentPercentage; }
            set
            {
                _currentPercentage = value;
                NotifyOfPropertyChange("CurrentPercentage");

                //notify change to other listeners of IBackgroundTask.ProgressChanged
                //only if progress change is NOT originated from another notification
                if (!_updatingProgressFromInnerIBackgroundTask)
                    BackgroundTask.CurrentContext.ReportProgress((int)(value * 100));
            }
        }

        public bool CancellationPending
        {
            get { return _bgTask.CancellationPending; }
        }

        public static void UpdateProgress(int percentage)
        {
            BackgroundTask.CurrentContext.ReportProgress(percentage);
        }

        public RunningAction(IBackgroundTask bgTask, bool isIndeterminate, bool isCancellable)
        {
            _isIndeterminate = isIndeterminate;
            _isCancellable = isCancellable;
            _bgTask = bgTask;

            //receive progress notification from implementor of IBackgroundTask to track progress
            //also if not reported using RunningAction class
            _bgTask.ProgressChanged += (o, e) =>{
                _updatingProgressFromInnerIBackgroundTask = true;
                try
                {
                    CurrentPercentage = e.ProgressPercentage/100.0;
                }
                finally
                {
                    _updatingProgressFromInnerIBackgroundTask = false;
                }
            };

            _bgTask.Starting += (o, e) => Current = this;
            _bgTask.Completed += (o, e) => Current = null;
        }

        public void Cancel()
        {
            _bgTask.Cancel();
        }
    }
}