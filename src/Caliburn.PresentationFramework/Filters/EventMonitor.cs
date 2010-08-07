using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.PresentationFramework.RoutedMessaging;
using Caliburn.Core.Invocation;
using System.Reflection;
using Caliburn.PresentationFramework.Invocation;

namespace Caliburn.PresentationFramework.Filters
{
	 

	public class EventMonitor
	{
		private readonly IRoutedMessageHandler _messageHandler;
		private readonly IList<IMessageTrigger> _triggersToNotify;
		

		public static EventMonitor TryHook(IRoutedMessageHandler messageHandler, string eventName){
			var target = messageHandler.Unwrap();
			var eventInfo = target.GetType().GetEvent(eventName);
			if (eventInfo == null) return null;
			return new EventMonitor(messageHandler, eventInfo);
		}


		internal EventMonitor(IRoutedMessageHandler messageHandler, EventInfo eventInfo)
		{
			_messageHandler = messageHandler;
			_triggersToNotify = new List<IMessageTrigger>();

			EventHelper.WireEvent(messageHandler.Unwrap(), eventInfo, ChangedEventHandler);
		}


		public void ChangedEventHandler(object sender, EventArgs e)
		{
			Execute.OnUIThread(() =>{

				foreach (var messageTrigger in _triggersToNotify)
				{
					_messageHandler.UpdateAvailability(messageTrigger);
				}
			});
		}

		internal void MakeAwareOf(IMessageTrigger trigger)
		{
			if (!_triggersToNotify.Contains(trigger))
				_triggersToNotify.Add(trigger);
		}
	}
}
