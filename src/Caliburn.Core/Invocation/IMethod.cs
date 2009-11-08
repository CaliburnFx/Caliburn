namespace Caliburn.Core.Invocation
{
    using System.Reflection;
    using Metadata;
    using Threading;

    /// <summary>
    /// Abstracts a generic way of invoking procedures and functions without using reflection.
    /// </summary>
    public interface IMethod : IMetadataContainer
    {
        /// <summary>
        /// Gets the <see cref="MethodInfo"/> to which this instance applies.
        /// </summary>
        /// <value>The info.</value>
        MethodInfo Info { get; }

        /// <summary>
        /// Invokes the specified method on the provided instance with the given parameters.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The result of the function or null if it is a procedure.</returns>
        object Invoke(object instance, params object[] parameters);

        /// <summary>
        /// Creates a background task for executing this method asynchronously.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>An instance of <see cref="IBackgroundTask"/>.</returns>
        IBackgroundTask CreateBackgroundTask(object instance, params object[] parameters);
    }
}