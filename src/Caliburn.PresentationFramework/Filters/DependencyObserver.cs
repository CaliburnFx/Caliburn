namespace Caliburn.PresentationFramework.Filters
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using Core;
	using Core.Invocation;
	using RoutedMessaging;
	using System;

	/// <summary>
	/// Metadata which can be used to trigger availability changes in triggers based on <see cref="INotifyPropertyChanged"/>.
	/// </summary>
	public class DependencyObserver
	{
		private readonly IRoutedMessageHandler _messageHandler;
		private readonly IMethodFactory _methodFactory;
		private readonly INotifyPropertyChanged _notifier;
		private readonly IDictionary<string, SinglePropertyPathObserver> _singlePathObservers;

		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyObserver"/> class.
		/// </summary>
		/// <param name="messageHandler">The message handler.</param>
		/// <param name="methodFactory">The method factory.</param>
		/// <param name="notifier">The notifier.</param>
		public DependencyObserver(IRoutedMessageHandler messageHandler, IMethodFactory methodFactory, INotifyPropertyChanged notifier)
		{
			_messageHandler = messageHandler;
			_methodFactory = methodFactory;
			_notifier = notifier;
			_singlePathObservers = new Dictionary<string, SinglePropertyPathObserver>();
		}

		/// <summary>
		/// Makes the metadata aware of the relationship between an <see cref="IMessageTrigger"/> and its dependencies.
		/// </summary>
		/// <param name="trigger">The trigger.</param>
		/// <param name="dependencies">The dependencies.</param>
		public void MakeAwareOf(IMessageTrigger trigger, IEnumerable<string> dependencies)
		{
			foreach (var dependency in dependencies)
			{
				var observer = GetSinglePathObserver(dependency);
				observer.RegisterTrigger(trigger);
			}
		}

		private SinglePropertyPathObserver GetSinglePathObserver(string propertyPath)
		{
			SinglePropertyPathObserver info;

			if (!_singlePathObservers.TryGetValue(propertyPath, out info))
			{
				info = new SinglePropertyPathObserver(_messageHandler, _methodFactory, _notifier, propertyPath);
				_singlePathObservers[propertyPath] = info;
			}

			return info;
		}

		//SEE: Tests.Caliburn.Actions.Filters.The_dependency_observer.backreferences_should_not_leak_the_observer_strict
		//~DependencyObserver()
		//{
		//    if (_monitoringInfos != null)
		//    {
		//        foreach (var info in _monitoringInfos.Values)
		//        {
		//            info.Dispose();
		//        }
		//    }
		//}
		
	}



	public class SinglePropertyPathObserver : IChangeMonitorNode
	{
		private IRoutedMessageHandler _messageHandler;
		private IList<IMessageTrigger> _triggersToNotify = new List<IMessageTrigger>();
		private PropertyPathMonitor _monitor;

		public SinglePropertyPathObserver(IRoutedMessageHandler messageHandler, IMethodFactory methodFactory, INotifyPropertyChanged notifier, string propertyPath)
		{
			_messageHandler = messageHandler;
			_monitor = new PropertyPathMonitor(methodFactory, notifier, propertyPath, this);
		}

		public void RegisterTrigger(IMessageTrigger trigger)
		{
			if (!_triggersToNotify.Contains(trigger))
				_triggersToNotify.Add(trigger);
		}

		public void NotifyChange()
		{
			_triggersToNotify.Apply(x => _messageHandler.UpdateAvailability(x));
		}
		public IChangeMonitorNode Parent
		{
			get { return null; }
		}

		public bool ShouldStopMonitoring()
		{
			return false;
		}


		public void Dispose()
		{
			if (_monitor != null)
				_monitor.Dispose();
		}

	 
		

	 
	}
}