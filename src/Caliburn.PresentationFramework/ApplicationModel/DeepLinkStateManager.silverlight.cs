//http://blogs.microsoft.co.il/blogs/alex_golesh/archive/2008/07/15/silverlight-tip-how-to-reflect-scriptobject-content-in-runtime.aspx
#if SILVERLIGHT

namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Browser;
    using Caliburn.Core;

    /// <summary>
    /// An implementation of <see cref="IStateManager"/> that stores state in the browser's URL.
    /// Note:  This implementation requires the presence of the ASP.NET ScriptManager.
    /// </summary>
    public class DeepLinkStateManager : PropertyChangedBase, IStateManager
    {
        private readonly Dictionary<string, string> _state = new Dictionary<string, string>();
        private string _stateName;
        private bool _isLoadingState, _isSavingState, _isInitialized;

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
            if(_isInitialized)
                return _isInitialized;

            try
            {
                var hostID = HtmlPage.Plugin.Id;

                if(string.IsNullOrEmpty(hostID))
                    return false;

                _stateName = stateName ?? string.Empty;

                HtmlPage.RegisterScriptableObject("DeepLinker", this);

                string initScript = @"
                ReflectProperties = function(obj)
                {
	                var props = new Array();
                	
	                for (var s in obj)
	                {
		                if (typeof(obj[s]) != 'function')
		                {
			                props[props.length] = s;
		                }
	                }
                	
	                return props;
                }

                ReflectMethods = function(obj)
                {
	                var methods = new Array();
                	
	                for (var s in obj)
	                {
		                if (typeof(obj[s]) == 'function')
		                {
			                methods[methods.length] = s;
		                }
	                }
                	
	                return methods;	
                }

                var __navigateHandler = new Function('obj','args','document.getElementById(\'" + hostID + @"\').content.DeepLinker.HandleNavigate(args.get_state())');
                Sys.Application.add_navigate(__navigateHandler);
                __navigateHandler(this, new Sys.HistoryEventArgs(Sys.Application._state));
            ";

                HtmlPage.Window.Eval(initScript);

                _isInitialized = true;
            }
            catch
            {
            }

            return _isInitialized;
        }

        /// <summary>
        /// Inserts or updates a value in the state.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public virtual void InsertOrUpdate(string key, string value)
        {
            if(_isLoadingState) return;
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
            if(_isLoadingState) return false;
            return _state.Remove(key);
        }

        /// <summary>
        /// Commits the changes to the backing store.
        /// </summary>
        /// <param name="stateName">Name of the state.</param>
        /// <returns></returns>
        public virtual bool CommitChanges(string stateName)
        {
            if(_isLoadingState || _isSavingState) return false;

            _isSavingState = true;

            BeforeStateCommit(this, EventArgs.Empty);

            try
            {
                string script = "Sys.Application.addHistoryPoint({";

                foreach(var pair in _state)
                {
                    if(pair.Value == null)
                    {
                        script = script + string.Format("{0}:null, ", pair.Key);
                    }
                    else
                    {
                        script = script + string.Format("{0}:'{1}', ", pair.Key, pair.Value);
                    }
                }

                script = script.Remove(script.Length - 2);
                script += string.Format("}}, '{0}');", stateName ?? _stateName);

                HtmlPage.Window.Eval(script);

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _isSavingState = false;
            }
        }

        /// <summary>
        /// Handles a navigation event signaled by the browser.
        /// </summary>
        /// <param name="newState">The new state.</param>
        [ScriptableMember]
        public virtual void HandleNavigate(ScriptObject newState)
        {
            if(_isLoadingState || _isSavingState) return;

            _isLoadingState = true;

            _state.Clear();

            var props = ReflectProperties<string[]>(newState);

            if (null != props)
            {
                foreach (string prop in props)
                {
                    _state.Add(prop, newState.GetProperty(prop).ToString());
                }
            }

            AfterStateLoad(this, EventArgs.Empty);

            _isLoadingState = false;
        }

        /// <summary>
        /// Reflects the properties of a scriptable object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static T ReflectProperties<T>(ScriptObject obj)
        {
            T retVal = default(T);

            var properties = HtmlPage.Window.Invoke("ReflectProperties", obj) as ScriptObject;

            if (null != properties)
                if (int.Parse(properties.GetProperty("length").ToString()) > 0)
                    retVal = properties.ConvertTo<T>();

            return retVal;
        }

        /// <summary>
        /// Reflects the methods of a scriptable object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static T ReflectMethods<T>(ScriptObject obj)
        {
            T retVal = default(T);

            var methods = HtmlPage.Window.Invoke("ReflectMethods", obj) as ScriptObject;

            if (null != methods)
                if (int.Parse(methods.GetProperty("length").ToString()) > 0)
                    retVal = methods.ConvertTo<T>();

            return retVal;
        }
    }
}

#endif