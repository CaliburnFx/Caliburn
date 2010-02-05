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
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.Screens;

    public class HistoryCoordinator : IHistoryCoordinator
    {
        private readonly IStateManager _stateManager;
        private readonly IAssemblySource _assemblySource;

        private HistoryConfiguration _config;
        private IScreen _previousScreen;
        private IList<IHistoryKey> _historyKeys;

        public HistoryCoordinator(IStateManager stateManager, IAssemblySource assemblySource)
        {
            _stateManager = stateManager;
            _assemblySource = assemblySource;
        }

        public void Start(Action<HistoryConfiguration> configurator)
        {
            _historyKeys = _assemblySource.SelectMany(assembly => FindKeys(assembly)).ToList();
            _assemblySource.AssemblyAdded += assembly => FindKeys(assembly).Apply(x => _historyKeys.Add(x));

            _config = new HistoryConfiguration
            {
                DetermineScreen = value =>{
                    var key = _historyKeys.FirstOrDefault(x => x.Value == value);
                    return key != null
                               ? key.GetInstance()
                               : null;
                }
            };

            configurator(_config);

            _config.Host.PropertyChanged += Host_PropertyChanged;
            _stateManager.AfterStateLoad += OnAfterStateLoad;
            _stateManager.Initialize(_config.StateName);
        }

        public void Refresh()
        {
            var historyValue = _stateManager.Get(_config.HistoryKey);
            var screen = _config.DetermineScreen(historyValue);

            if(screen == null)
                _config.ScreenNotFound(historyValue);
            else if(_config.Host.ActiveScreen == screen) return;
            else
            {
                _config.Host.OpenScreen(screen, wasSuccess =>{
                    if(wasSuccess)
                        UpdateTitle(screen);
                    else if(_previousScreen != null)
                    {
                        _stateManager.InsertOrUpdate(_config.HistoryKey, _previousScreen.GetHistoryValue());
                        _stateManager.CommitChanges(_previousScreen.DisplayName);
                        UpdateTitle(_previousScreen);
                    }
                });
            }
        }

        private void UpdateTitle(IScreen screen)
        {
            var oldTitle = HtmlPage.Document.GetProperty("title");
            var newTitle = _config.AlterTitle(oldTitle.ToString(), screen);

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
            if (e.PropertyName != "ActiveScreen") return;

            if (_config.Host.ActiveScreen == _previousScreen)
                return;

            _previousScreen = _config.Host.ActiveScreen;

            if (_previousScreen == null)
                _stateManager.Remove(_config.HistoryKey);
            else
            {
                _stateManager.InsertOrUpdate(_config.HistoryKey, _previousScreen.GetHistoryValue());
                _stateManager.CommitChanges(_previousScreen.DisplayName);

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