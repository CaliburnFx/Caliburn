namespace Caliburn.Core
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// A source of assemblies that are inspectable by the framework.
    /// </summary>
    public interface IAssemblySource : ICollection<Assembly>
    {
        /// <summary>
        /// Occurs when an assembly is added.
        /// </summary>
        event Action<Assembly> AssemblyAdded;
    }
}