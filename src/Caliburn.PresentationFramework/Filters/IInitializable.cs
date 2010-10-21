namespace Caliburn.PresentationFramework.Filters
{
    using System;
    using System.Reflection;
    using Core.InversionOfControl;

    /// <summary>
    /// An <see cref="IFilter"/> that requires initialization.
    /// </summary>
    public interface IInitializable : IFilter
    {
        /// <summary>
        /// Initializes the filter.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="member">The member.</param>
        /// <param name="serviceLocator">The serviceLocator.</param>
        void Initialize(Type targetType, MemberInfo member, IServiceLocator serviceLocator);
    }
}