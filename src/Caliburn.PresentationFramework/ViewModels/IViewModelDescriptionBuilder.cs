namespace Caliburn.PresentationFramework.ViewModels
{
    using System;

    public interface IViewModelDescriptionBuilder
    {
        IViewModelDescription Build(Type targetType);
    }
}