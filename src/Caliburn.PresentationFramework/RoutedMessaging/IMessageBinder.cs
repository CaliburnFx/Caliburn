namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A service that is capable of properly binding values to a methods parameters and return.
    /// </summary>
    public interface IMessageBinder
    {
        /// <summary>
        /// Determines whether the supplied value is recognized as a specialy treated value.
        /// </summary>
        /// <param name="potential">The potential value.</param>
        /// <returns>
        /// 	<c>true</c> if a special value; otherwise, <c>false</c>.
        /// </returns>
        bool IsSpecialValue(string potential);

        /// <summary>
        /// Identifies a special value along with its handler.
        /// </summary>
        /// <param name="specialValue">The special value.</param>
        /// <param name="handler">The handler.</param>
        void AddValueHandler(string specialValue, Func<IInteractionNode, object, object> handler);

        /// <summary>
        /// Determines the parameters that a method should be invoked with.
        /// </summary>
        /// <param name="message">The message to determine the parameters for.</param>
        /// <param name="requiredParameters">The requirements for method binding.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <param name="context">The context.</param>
        /// <returns>The actual parameters</returns>
        object[] DetermineParameters(IRoutedMessage message, IList<RequiredParameter> requiredParameters, IInteractionNode handlingNode, object context);

        /// <summary>
        /// Creates a result from the message outcome.
        /// </summary>
        /// <param name="outcome">The outcome of processing the message.</param>
        IResult CreateResult(MessageProcessingOutcome outcome);
    }
}