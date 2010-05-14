namespace Caliburn.Reactive
{
    using System;

    /// <summary>
    /// A service capable of publishing events.
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publishes the specified event.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="event">The event instance.</param>
        void Publish<TEvent>(TEvent @event);

        /// <summary>
        /// Gets the event for use by subscribers.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <returns></returns>
        IObservable<TEvent> GetEvent<TEvent>();
    }
}