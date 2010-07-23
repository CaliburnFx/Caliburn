#if SILVERLIGHT

namespace Caliburn.ShellFramework.History
{
    using System;
    using System.Linq;
    using Core;

    public static class HistoryExtensions
    {
        public static string GetHistoryValue(this object screen)
        {
            var keyAttribute = screen.GetHistoryKey();
            return keyAttribute != null ? keyAttribute.Value : null;
        }

        public static IHistoryKey GetHistoryKey(this object screen)
        {
            return screen == null ? null : screen.GetType().GetHistoryKey();
        }

        public static IHistoryKey GetHistoryKey(this Type type)
        {
            return type.GetAttributes<IHistoryKey>(false)
                .FirstOrDefault();
        }
    }
}

#endif