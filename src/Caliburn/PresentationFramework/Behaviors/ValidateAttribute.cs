#if NET || SILVERLIGHT_40
namespace Caliburn.PresentationFramework.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Core.Behaviors;

    /// <summary>
    /// Applies a behavior which implements <see cref="IDataErrorInfo"/>.
    /// </summary>
    public class ValidateAttribute : Attribute, IBehavior 
    {
        /// <summary>
        /// Gets the interfaces which represent this behavior.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <returns>The representative interfaces.</returns>
        public IEnumerable<Type> GetInterfaces(Type implementation)
        {
            yield return typeof(IDataErrorInfo);
            yield return typeof(IProxy);
        }
    }
}
#else
namespace Caliburn.PresentationFramework.Behaviors
{
    using System;
    using System.Collections.Generic;
    using Core.Behaviors;

    /// <summary>
    /// Applies a behavior which implements property validation using exceptions.
    /// </summary>
    public class ValidateAttribute : Attribute, IBehavior 
    {
        /// <summary>
        /// Gets the interfaces which represent this behavior.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <returns>The representative interfaces.</returns>
        public IEnumerable<Type> GetInterfaces(Type implementation)
        {
            yield return typeof(IProxy);
        }
    }
}
#endif