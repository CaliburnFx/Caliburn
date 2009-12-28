namespace Caliburn.PresentationFramework.ViewModels
{
    using System;

    public interface IViewModelDescriptionFactory
    {
        IViewModelDescription Create(Type targetType);
    }
}