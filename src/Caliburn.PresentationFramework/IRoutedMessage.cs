namespace Caliburn.PresentationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Instances of this interface can be routed through the interaction hierarchy.
    /// </summary>
    public interface IRoutedMessage : IEquatable<IRoutedMessage>
    {
        /// <summary>
        /// Gets or sets the availability effect.
        /// </summary>
        /// <value>The availability effect.</value>
        IAvailabilityEffect AvailabilityEffect { get; set; }

        /// <summary>
        /// Gets the source of the message.
        /// </summary>
        /// <value>The source.</value>
        IInteractionNode Source { get; }

#if !SILVERLIGHT
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        FreezableCollection<Parameter> Parameters { get; }
#else
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        List<Parameter> Parameters { get; }
#endif
        /// <summary>
        /// Initializes the message for interaction with the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        void Initialize(IInteractionNode node);

        /// <summary>
        /// Indicates whether this message is related to the potential target.
        /// </summary>
        /// <param name="potentialTarget">The potential target.</param>
        /// <returns></returns>
        bool RelatesTo(object potentialTarget);

        /// <summary>
        /// Gets the default handlers for this type of message.
        /// </summary>
        /// <param name="node">The node to get default handlers for.</param>
        /// <returns></returns>
        IEnumerable<IRoutedMessageHandler> GetDefaultHandlers(IInteractionNode node);

        /// <summary>
        /// Occurs when the message is invalidated.
        /// </summary>
        event Action Invalidated;
    }
}