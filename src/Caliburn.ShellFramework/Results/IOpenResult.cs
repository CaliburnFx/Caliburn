namespace Caliburn.ShellFramework.Results
{
    using System;
    using PresentationFramework;

    public interface IOpenResult<TChild> : IResult
    {
        Action<TChild> OnConfigure { get; set; }
        Action<TChild> OnShutDown { get; set; }
    }
}