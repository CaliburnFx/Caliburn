#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.Parsers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection.Emit;
	using System.Windows;
	using Triggers;
	using System.Reflection;
	using Core;

	/// <summary>
	/// An implementation of <see cref="ITriggerParser"/> that parses routed/attached events.
	/// </summary>
	public class AttachedEventTriggerParser : ITriggerParser
	{
	    private static readonly Type _dependencyObjectType = typeof(DependencyObject);

		/// <summary>
		/// Parses the specified trigger text.
		/// </summary>
		/// <param name="target">The targeted ui element.</param>
		/// <param name="triggerText">The trigger text.</param>
		/// <returns></returns>
		public IMessageTrigger Parse(DependencyObject target, string triggerText)
		{
			return new AttachedEventMessageTrigger { RoutedEvent = GetRoutedEvent(target, triggerText) };
		}

        /// <summary>
        /// Locates the routed event.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="triggerText">The trigger text.</param>
        /// <returns></returns>
		protected virtual RoutedEvent GetRoutedEvent(DependencyObject target, string triggerText)
		{
		    var eventOwner = target.GetType();

			if (triggerText.Contains("."))
			{
				var ownerTypeName = triggerText.Substring(0, triggerText.LastIndexOf("."));
				triggerText = triggerText.Substring(ownerTypeName.Length + 1);

			    var types = GetSearchableAssemblies().SelectMany(a => a.GetExportedTypes())
			        .Where(x => _dependencyObjectType.IsAssignableFrom(x) || (x.IsAbstract && x.IsSealed));

			    eventOwner = types.Where(t => t.FullName.Equals(ownerTypeName)).FirstOrDefault()
			                 ?? types.Where(t => t.Name.Equals(ownerTypeName)).FirstOrDefault();

			    if (eventOwner == null)
					throw new CaliburnException("Type " + ownerTypeName + " not found.");
			}

			var fieldEventProp = eventOwner.GetField(triggerText + "Event", BindingFlags.Static | BindingFlags.Public);

			if (fieldEventProp == null)
                throw new CaliburnException(triggerText + " event was not found on type " + eventOwner.Name + ".");

			if (!typeof(RoutedEvent).IsAssignableFrom(fieldEventProp.FieldType))
                throw new CaliburnException(triggerText + " event was not found on type " + eventOwner.Name + ".");

			return (RoutedEvent)fieldEventProp.GetValue(null);
		}

        /// <summary>
        /// Gets the assemblies that should be inspected for routed events.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<Assembly> GetSearchableAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !(assembly is AssemblyBuilder));
        }
	}
}

#endif