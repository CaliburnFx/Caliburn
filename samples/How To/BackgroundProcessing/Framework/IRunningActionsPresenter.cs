namespace BackgroundProcessing.Framework
{
    using System.Collections.Generic;

    public interface IRunningActionsRegistry
    {
        void RegisterTask(IRunningAction action);
        void UnregisterTask(IRunningAction action);
        bool HasRunningActions{ get;}
        IEnumerable<IRunningAction> RunningActions { get; }
    }
}