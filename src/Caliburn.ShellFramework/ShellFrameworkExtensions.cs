namespace Caliburn.ShellFramework
{
    using System.Collections.Generic;
    using Configuration;
    using Core;
    using Core.Configuration;
    using PresentationFramework.Screens;

    /// <summary>
    /// General extension methods related to the shell framework.
    /// </summary>
    public static class ShellFrameworkExtensions
    {
        /// <summary>
        /// Adds the shell framework module's configuration to the system.
        /// </summary>
        /// <param name="hook">The hook.</param>
        public static ShellFrameworkConfiguration ShellFramework(this IModuleHook hook)
        {
            return hook.Module(CaliburnModule<ShellFrameworkConfiguration>.Instance);
        }

        /// <summary>
        /// Determines the display name for the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static string DetermineDisplayName(this object item)
        {
            var displayNamed = item as IHaveDisplayName;
            if (displayNamed != null)
                return displayNamed.DisplayName;
            return item != null ? item.ToString() : string.Empty;
        }

        /// <summary>
        /// Creates the scope.
        /// </summary>
        /// <typeparam name="TScope">The type of scope controller.</typeparam>
        /// <typeparam name="TInScope">The type of the item in scope.</typeparam>
        /// <param name="inScope">The in item scope.</param>
        /// <param name="scope">The scope controller.</param>
        /// <returns></returns>
        public static Scope<TScope, TInScope> CreateScope<TScope, TInScope>(this TInScope inScope, TScope scope) 
            where TInScope : IList<TInScope>, IChild<TInScope> 
            where TScope : IActivate, IDeactivate
        {
            return new Scope<TScope, TInScope>(scope);
        }

        /// <summary>
        /// Returns the descendants and self.
        /// </summary>
        /// <typeparam name="T">The type of item to query.</typeparam>
        /// <param name="self">The item to return descendants for.</param>
        /// <returns>The flattened hierarchy.</returns>
        public static IEnumerable<T> DescendantsAndSelf<T>(this T self)
            where T : IEnumerable<T>
        {
            var queue = new Queue<T>(self);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                current.Apply(queue.Enqueue);
                yield return current;
            }
        }
    }
}