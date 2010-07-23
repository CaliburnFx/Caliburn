#if SILVERLIGHT

namespace Caliburn.ShellFramework.History
{
    public interface IHistoryKey
    {
        string Value { get; }
        object GetInstance();
    }
}

#endif