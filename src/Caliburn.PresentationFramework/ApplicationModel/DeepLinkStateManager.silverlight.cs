//http://blogs.microsoft.co.il/blogs/alex_golesh/archive/2008/07/15/silverlight-tip-how-to-reflect-scriptobject-content-in-runtime.aspx
#if SILVERLIGHT

namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Browser;
    using Caliburn.Core;
    using Core.Logging;

    /// <summary>
    /// An implementation of <see cref="IStateManager"/> that stores state in the browser's URL.
    /// Note:  This implementation requires the presence of the ASP.NET ScriptManager.
    /// </summary>
    public class DeepLinkStateManager : PropertyChangedBase, IStateManager
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(DeepLinkStateManager));

        private readonly Dictionary<string, string> state = new Dictionary<string, string>();
        private string stateName;
        private bool isLoadingState, isSavingState, isInitialized;

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
            if(isInitialized)
                return isInitialized;

            try
            {
                Log.Info("Initializing the deep link state manager.");

                var hostID = HtmlPage.Plugin.Id;

                if(string.IsNullOrEmpty(hostID))
                    return false;

                this.stateName = stateName ?? string.Empty;

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

                Log.Info("Injecting javascript.");
                HtmlPage.Window.Eval(initScript);

                isInitialized = true;
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }

            return isInitialized;
        }

        /// <summary>
        /// Inserts or updates a value in the state.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public virtual void InsertOrUpdate(string key, string value)
        {
            if(isLoadingState) return;
            state[key] = value;
        }

        /// <summary>
        /// Gets the value with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public virtual string Get(string key)
        {
            string value;
            state.TryGetValue(key, out value);
            return value;
        }

        /// <summary>
        /// Removes the value with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public virtual bool Remove(string key)
        {
            if(isLoadingState) return false;
            return state.Remove(key);
        }

        /// <summary>
        /// Commits the changes to the backing store.
        /// </summary>
        /// <param name="stateName">Name of the state.</param>
        /// <returns></returns>
        public virtual bool CommitChanges(string stateName)
        {
            if(isLoadingState || isSavingState) return false;

            isSavingState = true;

            BeforeStateCommit(this, EventArgs.Empty);

            try
            {
                string script = "Sys.Application.addHistoryPoint({";

                foreach(var pair in state)
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
                script += string.Format("}}, '{0}');", stateName ?? this.stateName);

                HtmlPage.Window.Eval(script);

                return true;
            }
            catch(Exception ex)
            {
                Log.Error(ex);
                return false;
            }
            finally
            {
                isSavingState = false;
            }
        }

        /// <summary>
        /// Handles a navigation event signaled by the browser.
        /// </summary>
        /// <param name="newState">The new state.</param>
        [ScriptableMember]
        public virtual void HandleNavigate(ScriptObject newState)
        {
            Log.Info("Browser navigate occurred.");

            if(isLoadingState || isSavingState) 
                return;

            isLoadingState = true;

            state.Clear();

            var props = ReflectProperties<string[]>(newState);

            if (null != props)
            {
                foreach (string prop in props)
                {
                    state.Add(prop, newState.GetProperty(prop).ToString());
                }
            }

            AfterStateLoad(this, EventArgs.Empty);

            isLoadingState = false;
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