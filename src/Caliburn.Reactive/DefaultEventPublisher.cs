namespace Caliburn.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The default implementation of <see cref="IEventPublisher"/>.
    /// </summary>
    public class DefaultEventPublisher : IEventPublisher
    {
        private readonly Dictionary<Type, object> subjects = new Dictionary<Type, object>();
        private readonly object @lock = new object();

        /// <summary>
        /// Gets the event for use by subscribers.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <returns></returns>
        public IObservable<TEvent> GetEvent<TEvent>()
        {
            object subject;
            var eventType = typeof(TEvent);

            if(!subjects.TryGetValue(eventType, out subject))
            {
                lock(@lock)
                {
                    if(!subjects.TryGetValue(eventType, out subject))
                    {
                        subjects[eventType] = subject = new Subject<TEvent>();
                    }
                }
            }

            return ((ISubject<TEvent>)subject).AsObservable();
        }

        /// <summary>
        /// Publishes the specified sample event.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="sampleEvent">The sample event.</param>
        public void Publish<TEvent>(TEvent sampleEvent)
        {
            object subject;

            if(subjects.TryGetValue(typeof(TEvent), out subject))
                ((ISubject<TEvent>)subject).OnNext(sampleEvent);
        }
    }
}