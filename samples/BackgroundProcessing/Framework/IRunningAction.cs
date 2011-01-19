namespace BackgroundProcessing.Framework {
    using System.ComponentModel;

    public interface IRunningAction : INotifyPropertyChanged {
        string Title { get; set; }
        bool IsIndeterminate { get; }
        bool IsCancellable { get; }
        double CurrentPercentage { get; set; }
        bool CancellationPending { get; }
        void Cancel();
    }
}