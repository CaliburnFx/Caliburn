namespace Caliburn.ShellFramework.Results
{
    using System;
    using PresentationFramework.Screens;

    public static class ExtensionMethods
    {
        public static IOpenResult<TChild> ConfigureChild<TChild>(this IOpenResult<TChild> result, Action<TChild> configure)
            where TChild : IScreen
        {
            result.OnConfigure = configure;
            return result;
        }

        public static IOpenResult<TChild> WhenShuttingDown<TChild>(this IOpenResult<TChild> result, Action<TChild> onShutdown)
            where TChild : IScreen
        {
            result.OnShutDown = onShutdown;
            return result;
        }
    }
}