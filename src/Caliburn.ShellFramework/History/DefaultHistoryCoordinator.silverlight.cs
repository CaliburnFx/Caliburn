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

        private readonly IStateManager stateManager;
        private readonly IAssemblySource assemblySource;

        private HistoryConfiguration config;
        private object previousScreen;
        private IList<IHistoryKey> historyKeys;

        public DefaultHistoryCoordinator(IStateManager stateManager, IAssemblySource assemblySource)
        {
            this.stateManager = stateManager;
            this.assemblySource = assemblySource;
        }

        public void Start(Action<HistoryConfiguration> configurator)
        {
            historyKeys = assemblySource.SelectMany(assembly => FindKeys(assembly)).ToList();
            assemblySource.AssemblyAdded += assembly => FindKeys(assembly).Apply(x => historyKeys.Add(x));

            config = new HistoryConfiguration {
                DetermineItem = value =>{
                    var key = historyKeys.FirstOrDefault(x => x.Value == value);
                    return key != null
                        ? key.GetInstance()
                        : null;
                }
            };

            configurator(config);

            config.Conductor.PropertyChanged += Host_PropertyChanged;
            config.Conductor.ActivationProcessed += Host_ActivationProcessed;
            stateManager.AfterStateLoad += OnAfterStateLoad;
            stateManager.Initialize(config.StateName);

            Log.Info("History coordinator started for conductor {0} with state {1}.", config.Conductor, config.StateName);
        }

        public void Refresh()
        {
            Log.Info("Refreshing {0} from history.", config.Conductor);

            var historyValue = stateManager.Get(config.HistoryKey);
            var screen = config.DetermineItem(historyValue);

            if(screen == null)
                config.ItemNotFound(historyValue);
            else if(config.Conductor.ActiveItem == screen) 
                return;
            else config.Conductor.ActivateItem(screen);
        }

        void Host_ActivationProcessed(object sender, ActivationProcessedEventArgs e)
        {
            if(e.Success)
            {
                UpdateTitle(sender);
                return;
            }

            if(previousScreen == null)
                return;

            Log.Info("Updating history key {0}.", config.HistoryKey);
            stateManager.InsertOrUpdate(config.HistoryKey, previousScreen.GetHistoryValue());
            stateManager.CommitChanges(previousScreen.DetermineDisplayName());
            UpdateTitle(previousScreen);
        }

        private void UpdateTitle(object item)
        {
            var oldTitle = HtmlPage.Document.GetProperty("title");
            var newTitle = config.AlterTitle(oldTitle.ToString(), item);

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

            if (config.Conductor.ActiveItem == previousScreen)
                return;

            previousScreen = config.Conductor.ActiveItem;

            if (previousScreen == null)
            {
                Log.Info("Removing history key {0}.", config.HistoryKey);
                stateManager.Remove(config.HistoryKey);
            }
            else
            {
                Log.Info("Updating history key {0}.", config.HistoryKey);

                stateManager.InsertOrUpdate(config.HistoryKey, previousScreen.GetHistoryValue());
                stateManager.CommitChanges(previousScreen.DetermineDisplayName());

                UpdateTitle(previousScreen);
            }
        }

        private void OnAfterStateLoad(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}

#endif