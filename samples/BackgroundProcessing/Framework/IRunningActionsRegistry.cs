namespace BackgroundProcessing.Framework {
    using System.Collections.Generic;

    public interface IRunningActionsRegistry {
        bool HasRunningActions { get; }
        IEnumerable<IRunningAction> RunningActions { get; }
        void RegisterTask(IRunningAction action);
        void UnregisterTask(IRunningAction action);
    }
}