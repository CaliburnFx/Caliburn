namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Xml;
    using Core;
    using Core.Logging;

    /// <summary>
    /// An implementation of <see cref="IStateManager"/> that uses isolated storage as its backing store.
    /// </summary>
    public class IsolatedStorageStateManager : PropertyChangedBase, IStateManager
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(IsolatedStorageStateManager));
        private readonly Dictionary<string, string> _state = new Dictionary<string, string>();
        private string _stateName;

        /// <summary>
        /// Occurs after the state was loaded from the backing store.
        /// </summary>
        public event EventHandler AfterStateLoad = delegate { };

        /// <summary>
        /// Occurs before the state is committed to the backing store.
        /// </summary>
        public event EventHandler BeforeStateCommit = delegate { };

        /// <summary>
        /// Initializes the backing store.
        /// </summary>
        /// <param name="stateName">Name of the state.</param>
        /// <returns></returns>
        public virtual bool Initialize(string stateName)
        {
            if(string.IsNullOrEmpty(stateName))
            {
                var ex = new CaliburnException("State name is null or empty.");
                Log.Error(ex);
                throw ex;
            }

            _stateName = stateName;

            return LoadState();
        }

        private bool LoadState()
        {
            try
            {
                Log.Info("Loading state.");

                using(var store = IsolatedStorageFile.GetUserStoreForApplication())
                using(var stream = new IsolatedStorageFileStream(_stateName, FileMode.Open, store))
                using(var reader = XmlReader.Create(stream))
                {
                    if(reader.ReadToFollowing("State"))
                    {
                        int stateDepth = reader.Depth;
                        reader.Read();

                        while(reader.Depth > stateDepth)
                        {
                            var key = reader.GetAttribute("Key");
                            var value = reader.ReadElementContentAsString();

                            _state[key] = value;
                        }
                    }
                }

                AfterStateLoad(this, EventArgs.Empty);
                return true;
            }
            catch(Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Commits the changes.
        /// </summary>
        /// <param name="stateName">Name of the state.</param>
        /// <returns></returns>
        public virtual bool CommitChanges(string stateName)
        {
            BeforeStateCommit(this, EventArgs.Empty);

            _stateName = stateName ?? _stateName;

            try
            {
                Log.Info("Committing state.");

                using(var store = IsolatedStorageFile.GetUserStoreForApplication())
                using(var stream = new IsolatedStorageFileStream(_stateName, FileMode.OpenOrCreate, store))
                using(var writer = XmlWriter.Create(stream))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("State");

                    foreach(var pair in _state)
                    {
                        writer.WriteStartElement("Item");
                        writer.WriteAttributeString("Key", pair.Key);
                        writer.WriteString(pair.Value);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndDocument();
                }

                return true;
            }
            catch(Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Inserts or updates a value in the state.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public virtual void InsertOrUpdate(string key, string value)
        {
            _state[key] = value;
        }

        /// <summary>
        /// Gets the value with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public virtual string Get(string key)
        {
            string value;
            _state.TryGetValue(key, out value);
            return value;
        }

        /// <summary>
        /// Removes the value with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public virtual bool Remove(string key)
        {
            return _state.Remove(key);
        }
    }
}