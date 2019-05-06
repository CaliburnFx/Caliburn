namespace Caliburn.PresentationFramework.Filters
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using Core.Invocation;
	using RoutedMessaging;

	/// <summary>
	/// Metadata which can be used to trigger availability changes in triggers based on <see cref="INotifyPropertyChanged"/>.
	/// </summary>
	public class DependencyObserver
	{
		readonly IRoutedMessageHandler messageHandler;
		readonly IMethodFactory methodFactory;
		readonly INotifyPropertyChanged notifier;
		readonly IDictionary<string, SinglePropertyPathObserver> singlePathObservers;

		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyObserver"/> class.
		/// </summary>
		/// <param name="messageHandler">The message handler.</param>
		/// <param name="methodFactory">The method factory.</param>
		/// <param name="notifier">The notifier.</param>
		public DependencyObserver(IRoutedMessageHandler messageHandler, IMethodFactory methodFactory, INotifyPropertyChanged notifier)
		{
			this.messageHandler = messageHandler;
			this.methodFactory = methodFactory;
			this.notifier = notifier;
			singlePathObservers = new Dictionary<string, SinglePropertyPathObserver>();
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
				if (observer!= null)
					observer.RegisterTrigger(trigger);
			}
		}

		SinglePropertyPathObserver GetSinglePathObserver(string propertyPath)
		{
			SinglePropertyPathObserver pathObserver;

			if (!singlePathObservers.TryGetValue(propertyPath, out pathObserver))
			{
				pathObserver = new SinglePropertyPathObserver(messageHandler, methodFactory, notifier, propertyPath);
				singlePathObservers[propertyPath] = pathObserver;
			}

			return pathObserver;
		}

		//SEE: Tests.Caliburn.Actions.Filters.The_dependency_observer.backreferences_should_not_leak_the_observer_strict
//		~DependencyObserver()
//		{
//		    if (_monitoringInfos != null)
//		    {
//		        foreach (var info in _monitoringInfos.Values)
//		        {
//		            info.Dispose();
//		        }
//		    }
//		}
	}
}