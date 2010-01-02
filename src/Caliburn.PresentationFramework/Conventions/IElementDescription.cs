namespace Caliburn.PresentationFramework.Conventions
{
    using System;

    public interface IElementDescription
    {
        Type Type { get; }
        string Name { get; }
        IElementConvention Convention { get; }
    }
}