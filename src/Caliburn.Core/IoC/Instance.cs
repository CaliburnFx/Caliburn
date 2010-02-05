namespace Caliburn.Core.IoC
{
    using System;

    /// <summary>
    /// Represents the registration of an existing instance.
    /// </summary>
    public class Instance : ComponentRegistrationBase
    {
        /// <summary>
        /// Gets or sets the implementation.
        /// </summary>
        /// <value>The implementation.</value>
        public object Implementation { get; set; }

        /// <summary>
        /// Gets the component info.
        /// </summary>
        /// <param name="decoratedType">Type of the decorated.</param>
        /// <returns></returns>
        public override IComponentRegistration GetComponentInfo(Type decoratedType)
        {
            throw new NotSupportedException();
        }
    }
}