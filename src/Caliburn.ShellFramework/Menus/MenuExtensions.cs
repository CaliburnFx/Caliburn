namespace Caliburn.ShellFramework.Menus
{
    using System.Collections.Generic;

    public static class MenuExtensions
    {
        public static IEnumerable<IMenu> Descendants(this IMenu parent)
        {
            var queue = new Queue<IMenu>(parent);

            while(queue.Count > 0)
            {
                var current = queue.Dequeue();

                foreach(var child in current)
                {
                    queue.Enqueue(child);
                }

                yield return current;
            }
        }
    }
}