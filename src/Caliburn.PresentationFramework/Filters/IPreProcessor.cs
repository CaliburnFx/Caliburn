namespace Caliburn.PresentationFramework.Filters
{
    /// <summary>
    /// A <see cref="IFilter"/> that is executing before something.
    /// </summary>
    public interface IPreProcessor : IFilter
    {
        /// <summary>
        /// Gets a value indicating whether this filter affects triggers.
        /// </summary>
        /// <value><c>true</c> if affects triggers; otherwise, <c>false</c>.</value>
        bool AffectsTriggers { get; }

        /// <summary>
        /// Executes the filter.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <returns></returns>
        bool Execute(IRoutedMessage message, IInteractionNode handlingNode, object[] parameters);
    }
}