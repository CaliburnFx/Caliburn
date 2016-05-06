namespace Caliburn.Core.Behaviors
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents behavior which should be added to a proxy.
    /// </summary>
    public interface IBehavior
    {
        /// <summary>
        /// Gets the interfaces which represent this behavior.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <returns>The representative interfaces.</returns>
        IEnumerable<Type> GetInterfaces(Type implementation);
    }
}