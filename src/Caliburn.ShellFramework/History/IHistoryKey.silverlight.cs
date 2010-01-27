#if SILVERLIGHT

namespace Caliburn.ShellFramework.History
{
    using PresentationFramework.Screens;

    public interface IHistoryKey
    {
        string Value { get; }
        IScreen GetInstance();
    }
}

#endif