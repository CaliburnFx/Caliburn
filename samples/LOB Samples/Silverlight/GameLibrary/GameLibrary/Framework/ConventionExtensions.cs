namespace GameLibrary.Framework
{
    using System;
    using System.Windows;
    using Caliburn.PresentationFramework.Conventions;

    public static class ConventionExtensions
    {
        public static void AddElementConvention<T>(this IConventionManager conventionManager, string defaultEvent, DependencyProperty bindableProperty, Action<T, object> setter, Func<T, object> getter)
            where T : DependencyObject
        {
            conventionManager.AddElementConvention(
                new DefaultElementConvention<T>(
                    defaultEvent,
                    bindableProperty,
                    setter,
                    getter,
                    null
                    ));
        }
    }
}