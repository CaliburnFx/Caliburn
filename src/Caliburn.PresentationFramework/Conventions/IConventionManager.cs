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
        IElementConvention GetElementConvention(Type elementType);

        void ApplyActionCreationConventions(IAction action, IMethod targetMethod);

        IEnumerable<IViewApplicable> DetermineConventions(IViewModelDescription viewModelDescription, IEnumerable<IElementDescription> elementDescriptions);
    }
}