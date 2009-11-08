namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Describes a service capable of managing basic state values.
    /// </summary>
    public interface IStateManager : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs after the state was loaded from the backing store.
        /// </summary>
        event EventHandler AfterStateLoad;

        /// <summary>
        /// Occurs before the state is committed to the backing store.
        /// </summary>
        event EventHandler BeforeStateCommit;

        /// <summary>
        /// Initializes the backing store.
        /// </summary>
        /// <param name="stateName">Name of the state.</param>
        /// <returns></returns>
        bool Initialize(string stateName);

        /// <summary>
        /// Commits the changes to the backing store.
        /// </summary>
        /// <param name="stateName">Name of the state.</param>
        /// <returns></returns>
        bool CommitChanges(string stateName);

        /// <summary>
        /// Inserts or updates a value in the state.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void InsertOrUpdate(string key, string value);

        /// <summary>
        /// Gets the value with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string Get(string key);

        /// <summary>
        /// Removes the value with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        bool Remove(string key);
    }
}