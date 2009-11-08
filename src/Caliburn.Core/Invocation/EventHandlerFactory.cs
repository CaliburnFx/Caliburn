namespace Caliburn.Core.Invocation
{
    using System;
    using System.Reflection;

    /// <summary>
    /// An implementation of <see cref="IEventHandlerFactory"/>.
    /// </summary>
    public class EventHandlerFactory : IEventHandlerFactory
    {
        private static readonly Type genericEventHandlerType = typeof(GenericEventHandler<,>);

        /// <summary>
        /// Wires an event handler to the sender for the specified event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>The event handler.</returns>
        public IEventHandler Wire(object sender, string eventName)
        {
            var eventInfo = sender.GetType().GetEvent(eventName);

            if(eventInfo == null)
                throw new CaliburnException(
                    string.Format("The event '{0}' does not exist on '{1}'.", eventName, sender.GetType().FullName)
                    );

            return Wire(sender, eventInfo);
        }

        /// <summary>
        /// Wires an event handler to the sender for the specified event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventInfo">The event info.</param>
        /// <returns>The event handler.</returns>
        public IEventHandler Wire(object sender, EventInfo eventInfo)
        {
            var parameters = eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters();
            var typeParameters = GetTypeParameters(parameters);

            var handler = (IEventHandler)Activator.CreateInstance(
                                             genericEventHandlerType.MakeGenericType(typeParameters)
                                             );

            eventInfo.AddEventHandler(
                sender,
                Delegate.CreateDelegate(eventInfo.EventHandlerType, handler, "Invoke")
                );

            return handler;
        }

        private static Type[] GetTypeParameters(ParameterInfo[] parameters)
        {
            if(parameters.Length != 2)
                throw new CaliburnException("The event signature must have two parameters.");

            return new[]
            {
                parameters[0].ParameterType,
                parameters[1].ParameterType,
            };
        }

        private class GenericEventHandler<T, K> : IEventHandler
        {
            private Action<object[]> _action;

            /// <summary>
            /// Sets the actual handler for the event.
            /// </summary>
            /// <param name="action">The action.</param>
            public void SetActualHandler(Action<object[]> action)
            {
                _action = action;
            }

            /// <summary>
            /// Invokes the handler.
            /// </summary>
            /// <param name="argT">The arg T.</param>
            /// <param name="argK">The arg K.</param>
            public void Invoke(T argT, K argK)
            {
                Execute(argT, argK);
            }

            private void Execute(params object[] invokeArgs)
            {
                if(_action != null)
                    _action(invokeArgs);
                else throw new CaliburnException("The invoker does not have a valid Action.");
            }
        }
    }
}