namespace Caliburn.ShellFramework.Results
{
    using System;
    using PresentationFramework.RoutedMessaging;

    public interface IOpenResult<TTarget> : IResult
    {
        Action<TTarget> OnConfigure { get; set; }
        Action<TTarget> OnClose { get; set; }
    }
}