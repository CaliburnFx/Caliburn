namespace Caliburn.PresentationFramework.ViewModels
{
    using System;

    /// <summary>
    /// A service responsible for creating instances of <see cref="IViewModelDescription"/>.
    /// </summary>
    public interface IViewModelDescriptionFactory
    {
        /// <summary>
        /// Creates a description based on the target type.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>A view model description.</returns>
        IViewModelDescription Create(Type targetType);
    }
}