#if SILVERLIGHT

namespace Caliburn.ShellFramework.History
{
    /// <summary>
    /// A history key that links to a ViewModel instance.
    /// </summary>
    public interface IHistoryKey
    {
        /// <summary>
        /// Gets the key's value.
        /// </summary>
        /// <value>The value.</value>
        string Value { get; }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>
        object GetInstance();
    }
}

#endif