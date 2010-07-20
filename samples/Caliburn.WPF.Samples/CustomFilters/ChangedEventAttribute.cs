namespace CustomFilters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Caliburn.PresentationFramework.Filters;
	using Caliburn.Core;
	using Caliburn.Core.Invocation;
	using Caliburn.PresentationFramework.RoutedMessaging;
	using System.Reflection;


	/// <summary>
	/// Demonstrates a filter hooking an event of an object held by the action handler (View Model),
	/// or even an event of the View Model itself.
	/// When the event is raised, the filter forces a re-evaluation of action preconditions.
	/// </summary>
	public class ChangedEventAttribute : Attribute, IInitializable, IFilter, IHandlerAware
	{
		public ChangedEventAttribute() : base() { }
		public ChangedEventAttribute(string eventPath)
			: this()
		{
			EventPath = eventPath;
		}

		public int Priority { get; set; }
		public string EventPath { get; set; }

		//Filters implementing IInitializable have access to information about member being decorated with the filter.
		//Plus, they have the opportunity to pull some service from the service locator
		string _actualEventPath;
		void IInitializable.Initialize(Type targetType, System.Reflection.MemberInfo member, Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator)
		{
			if (member is MethodInfo)
			{
				_actualEventPath = EventPath ?? string.Format("Can{0}Changed", member.Name);
			}
		}

		//Filters implementing IHandlerAware are made aware of each IRoutedMessageHandler they handle; filter has
		//the opporunity to store some addictional helper or metadata in the specific handler instance
		void IHandlerAware.MakeAwareOf(Caliburn.PresentationFramework.RoutedMessaging.IRoutedMessageHandler messageHandler)
		{
			var monitor = messageHandler.Metadata.FirstOrDefaultOfType<ChangedEventMonitor>();
			if (monitor == null && !string.IsNullOrEmpty(_actualEventPath))
			{
				var eventPair = GetEventPair(messageHandler.Unwrap(), _actualEventPath);
				if (eventPair != null)
				{
					monitor = new ChangedEventMonitor(eventPair.EventInfo, eventPair.Sender, messageHandler);
					messageHandler.Metadata.Add(monitor);
				}
			}
		}

		//Filters implementing IHandlerAware are made aware of each trigger that could possibly activate the handler
		void IHandlerAware.MakeAwareOf(Caliburn.PresentationFramework.RoutedMessaging.IRoutedMessageHandler messageHandler, Caliburn.PresentationFramework.RoutedMessaging.IMessageTrigger trigger)
		{
			var monitor = messageHandler.Metadata.FirstOrDefaultOfType<ChangedEventMonitor>();
			if (monitor != null)
			{
				monitor.AddTrigger(trigger);
			}
		}



		//this function has an important limitation: it can obtain the EventInfo ONLY
		//if the full property path up to the instance raising the event is uninterrupted (i.e. no null intances on the path)
		//at the time of initialization of the filter (which occurs very early in the lifetime of the ViewModel)
		EventPair GetEventPair(object rootObject, string eventPath)
		{
			if (rootObject== null) return null;

			var currentInstance = rootObject;
			var currentPath = eventPath;
			while (true)
			{
				var dotIndex = currentPath.IndexOf(".");
				if (dotIndex < 0) break;

				var propertyName = currentPath.Substring(0, dotIndex);
				if (string.IsNullOrEmpty(propertyName)) throw new ArgumentException(string.Format("Invalid event path", eventPath), "eventPath");

				var propertyInfo = currentInstance.GetType().GetProperty(propertyName);
				if (propertyInfo == null) return null;

				currentInstance = propertyInfo.GetGetMethod().Invoke(currentInstance, new object[] { });
				if (currentInstance==null) return null;

				if (dotIndex == currentPath.Length - 1) throw new ArgumentException(string.Format("Invalid event path", eventPath), "eventPath");
				currentPath = currentPath.Substring(dotIndex + 1);
			}
			var pair = new EventPair
			{
				EventInfo = currentInstance.GetType().GetEvent(currentPath),
				Sender = currentInstance
			};
			if (pair.EventInfo == null) pair = null;
			return pair;
		}
		internal class EventPair {
			public EventInfo EventInfo;
			public object Sender;
		}


		//helper class used for each message handler instance to monitor the specified event
		internal class ChangedEventMonitor
		{

			IList<IMessageTrigger> _triggers = new List<IMessageTrigger>();

			public ChangedEventMonitor(EventInfo @event, object sender, IRoutedMessageHandler messageHandler)
			{
				if (@event != null)
				{
					EventHelper.WireEvent(sender, @event, (o, e) =>
					{
						foreach (var trigger in _triggers)
							messageHandler.UpdateAvailability(trigger);
					});
				}
			}


			public void AddTrigger(IMessageTrigger trigger)
			{
				_triggers.Add(trigger);
			}


		}
	}
}
