#if SILVERLIGHT

namespace Caliburn.ShellFramework.History
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Browser;
    using Core;
    using Core.Logging;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.Screens;

    public class DefaultHistoryCoordinator : IHistoryCoordinator
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(DefaultHistoryCoordinator));

        private readonly IStateManager _stateManager;
        private readonly IAssemblySource _assemblySource;

        private HistoryConfiguration _config;
        private object _previousScreen;
        private IList<IHistoryKey> _historyKeys;

        public DefaultHistoryCoordinator(IStateManager stateManager, IAssemblySource assemblySource)
        {
            _stateManager = stateManager;
            _assemblySource = assemblySource;
        }

        public void Start(Action<HistoryConfiguration> configurator)
        {
            _historyKeys = _assemblySource.SelectMany(assembly => FindKeys(assembly)).ToList();
            _assemblySource.AssemblyAdded += assembly => FindKeys(assembly).Apply(x => _historyKeys.Add(x));

            _config = new HistoryConfiguration {
                DetermineItem = value =>{
                    var key = _historyKeys.FirstOrDefault(x => x.Value == value);
                    return key != null
                        ? key.GetInstance()
                        : null;
                }
            };

            configurator(_config);

            _config.Conductor.PropertyChanged += Host_PropertyChanged;
            _config.Conductor.ActivationProcessed += Host_ActivationProcessed;
            _stateManager.AfterStateLoad += OnAfterStateLoad;
            _stateManager.Initialize(_config.StateName);

            Log.Info("History coordinator started for conductor {0} with state {1}.", _config.Conductor, _config.StateName);
        }

        public void Refresh()
        {
            Log.Info("Refreshing {0} from history.", _config.Conductor);

            var historyValue = _stateManager.Get(_config.HistoryKey);
            var screen = _config.DetermineItem(historyValue);

            if(screen == null)
                _config.ItemNotFound(historyValue);
            else if(_config.Conductor.ActiveItem == screen) 
                return;
            else _config.Conductor.ActivateItem(screen);
        }

        void Host_ActivationProcessed(object sender, ActivationProcessedEventArgs e)
        {
            if(e.Success)
            {
                UpdateTitle(sender);
                return;
            }

            if(_previousScreen == null)
                return;

            Log.Info("Updating history key {0}.", _config.HistoryKey);
            _stateManager.InsertOrUpdate(_config.HistoryKey, _previousScreen.GetHistoryValue());
            _stateManager.CommitChanges(_previousScreen.DetermineDisplayName());
            UpdateTitle(_previousScreen);
        }

        private void UpdateTitle(object item)
        {
            var oldTitle = HtmlPage.Document.GetProperty("title");
            var newTitle = _config.AlterTitle(oldTitle.ToString(), item);

            if (!string.IsNullOrEmpty(newTitle))
                HtmlPage.Document.SetProperty("title", newTitle);
        }

        private IEnumerable<IHistoryKey> FindKeys(Assembly assembly)
        {
            return from type in assembly.GetExportedTypes()
                   let key = type.GetHistoryKey()
                   where key != null
                   select key;
        }

        private void Host_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "ActiveItem") 
                return;

            if (_config.Conductor.ActiveItem == _previousScreen)
                return;

            _previousScreen = _config.Conductor.ActiveItem;

            if (_previousScreen == null)
            {
                Log.Info("Removing history key {0}.", _config.HistoryKey);
                _stateManager.Remove(_config.HistoryKey);
            }
            else
            {
                Log.Info("Updating history key {0}.", _config.HistoryKey);

                _stateManager.InsertOrUpdate(_config.HistoryKey, _previousScreen.GetHistoryValue());
                _stateManager.CommitChanges(_previousScreen.DetermineDisplayName());

                UpdateTitle(_previousScreen);
            }
        }

        private void OnAfterStateLoad(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}

#endif