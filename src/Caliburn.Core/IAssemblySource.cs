namespace Caliburn.Core
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// A source of assemblies that are inspectable by the framework.
    /// </summary>
    public interface IAssemblySource : IEnumerable<Assembly>
    {
        /// <summary>
        /// Adds the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        void Add(Assembly assembly);

        /// <summary>
        /// Removes the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        bool Remove(Assembly assembly);

        /// <summary>
        /// Occurs when an assembly is added.
        /// </summary>
        event Action<Assembly> AssemblyAdded;
    }
}