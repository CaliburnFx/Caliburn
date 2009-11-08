namespace Caliburn.Silverlight.ApplicationFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Browser;
    using Microsoft.Practices.ServiceLocation;
    using PresentationFramework.ApplicationModel;

    public class HistoryManager<THome>
        where THome : IPresenter
    {
        private IList<HistoryInfo> _historyInfo;
        private readonly IPresenterManager _host;
        private readonly IStateManager _stateManager;
        private readonly IServiceLocator _serviceLocator;

        public HistoryManager(IPresenterManager host, IStateManager stateManager, IServiceLocator serviceLocator)
        {
            _host = host;
            _stateManager = stateManager;
            _serviceLocator = serviceLocator;

            _stateManager.AfterStateLoad += OnAfterStateLoad;
        }

        public void Initialize(params Assembly[] assemblies)
        {
            var keys = from assembly in assemblies
                       from type in assembly.GetTypes()
                       from att in type.GetCustomAttributes(typeof(HistoryKeyAttribute), false)
                       select ((HistoryKeyAttribute)att).CreateHistoryInfo(type);

            _historyInfo = keys.ToList();

            _host.PropertyChanged += Host_PropertyChanged;
            _stateManager.Initialize("Home");
        }

        private void Host_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentPresenter")
            {
                var presenter = _host.CurrentPresenter;

                if (presenter == null)
                    _stateManager.Remove("View");
                else
                {
                    _stateManager.InsertOrUpdate("View", GetHistoryKey(presenter));
                    _stateManager.CommitChanges(presenter.DisplayName);
                }
            }
        }

        private void OnAfterStateLoad(object sender, EventArgs e)
        {
            var historyKey = _stateManager.Get("View");

            var presenter = string.IsNullOrEmpty(historyKey)
                                ? _serviceLocator.GetInstance<THome>()
                                : (from info in _historyInfo
                                   where info.HistoryKey == historyKey
                                   select (IPresenter)_serviceLocator.GetInstance(info.ServiceType)).FirstOrDefault();

            if(presenter != null)
            {
                _host.Open(presenter);
                HtmlPage.Document.SetProperty("title", presenter.DisplayName);
            }
        }

        private string GetHistoryKey(IPresenter presenter)
        {
            var attribute = (from key in _historyInfo
                             where presenter.GetType().GetInterfaces().Contains(key.ServiceType)
                             select key).FirstOrDefault();

            return attribute.HistoryKey;
        }
    }
}