namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Collections.Generic;
    using Actions;
    using Core.Invocation;
    using ViewModels;

    public interface IConventionManager
    {
        void AddElementConvention(IElementConvention convention);
        void AddBindingConvention(IBindingConvention convention);
        void AddActionConvention(IActionConvention convention);

        IElementConvention GetElementConvention(Type elementType);
        IEnumerable<IViewApplicable> DetermineConventions(IViewModelDescription viewModelDescription, IEnumerable<IElementDescription> elementDescriptions);

        void ApplyActionCreationConventions(IAction action, IMethod targetMethod);
    }
}