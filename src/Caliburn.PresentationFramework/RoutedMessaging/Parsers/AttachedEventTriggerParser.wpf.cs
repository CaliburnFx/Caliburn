#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.RoutedMessaging.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Windows;
    using Core;
    using Core.Logging;
    using Triggers;

    /// <summary>
    /// An implementation of <see cref="ITriggerParser"/> that parses routed/attached events.
    /// </summary>
    public class AttachedEventTriggerParser : ITriggerParser
    {
        static readonly ILog Log = LogManager.GetLog(typeof(AttachedEventTriggerParser));
        static readonly Type DependencyObjectType = typeof(DependencyObject);

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
                    .Where(x => DependencyObjectType.IsAssignableFrom(x) || (x.IsAbstract && x.IsSealed));

                eventOwner = types.Where(t => t.FullName.Equals(ownerTypeName)).FirstOrDefault()
                    ?? types.Where(t => t.Name.Equals(ownerTypeName)).FirstOrDefault();

                if (eventOwner == null)
                {
                    var exception = new CaliburnException("Type " + ownerTypeName + " not found.");
                    Log.Error(exception);
                    throw exception;
                }
            }

            var fieldEventProp = eventOwner.GetField(triggerText + "Event", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (fieldEventProp == null)
            {
                var exception = new CaliburnException(triggerText + " event was not found on type " + eventOwner.Name + ".");
                Log.Error(exception);
                throw exception;
            }

            if (!typeof(RoutedEvent).IsAssignableFrom(fieldEventProp.FieldType))
            {
                var exception = new CaliburnException(triggerText + " event was not found on type " + eventOwner.Name + ".");
                Log.Error(exception);
                throw exception;
            }

            return (RoutedEvent)fieldEventProp.GetValue(null);
        }
 


		/// <summary>
		/// Gets the assemblies that should be inspected for routed events.
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerable<Assembly> GetSearchableAssemblies()
		{
			return AppDomain.CurrentDomain.GetAssemblies()
				.Where(assembly => !IsDynamicAssembly(assembly));

		}

		/// <summary>
		/// Checks if an assembly is dynamically generated (it doesn't support GetExportedTypes)
		/// </summary>
		/// <param name="test">the assembly being test</param>
		/// <returns></returns>
		protected virtual bool IsDynamicAssembly(Assembly test)
		{
			return IsNet35DynamicAssembly(test) || IsNet40DynamicAssembly(test);
		}

		bool IsNet35DynamicAssembly(Assembly test)
		{
			return test is AssemblyBuilder;
		}

		bool IsNet40DynamicAssembly(Assembly test)
		{
			var type = test.GetType();
			while (!type.Equals(typeof(Assembly)))
			{
				if (type.FullName.Equals("System.Reflection.Emit.InternalAssemblyBuilder")) return true;
				type = type.BaseType;
			}
			return false;
		}
    }
}

#endif