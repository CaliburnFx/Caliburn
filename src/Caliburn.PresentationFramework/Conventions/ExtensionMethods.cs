namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using ViewModels;

#if SILVERLIGHT
    using System.Windows.Media;
#endif

    public static class ExtensionMethods
    {
        public static Func<IConventionManager, DependencyObject, IEnumerable<IElementDescription>> GetNamedElements = DefaultGetNamedObjects;

        public static IEnumerable<IViewApplicable> DetermineConventions(this IConventionManager conventionManager, IViewModelDescription viewModelDescription, DependencyObject view)
        {
            return conventionManager.DetermineConventions(viewModelDescription, GetNamedElements(conventionManager, view));
        }

#if !SILVERLIGHT
        private static IEnumerable<IElementDescription> DefaultGetNamedObjects(IConventionManager conventionManager, DependencyObject root)
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var currentName = current.GetName();

                if (!string.IsNullOrEmpty(currentName))
                {
                    var currentType = current.GetType();
                    var currentConvention = conventionManager.GetElementConvention(currentType);

                    if (currentConvention != null)
                        yield return new DefaultElementDescription(currentType, currentName, currentConvention);
                }

                foreach (object child in LogicalTreeHelper.GetChildren(current))
                {
                    var childDo = child as DependencyObject;

                    if (childDo == null || childDo is UserControl)
                        continue;

                    queue.Enqueue(childDo);
                }
            }
        }
#else
        private static IEnumerable<IElementDescription> DefaultGetNamedObjects(IConventionManager conventionManager, DependencyObject root)
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var currentName = current.GetName();

                if (!string.IsNullOrEmpty(currentName))
                {
                    var currentType = current.GetType();
                    var currentConvention = conventionManager.GetElementConvention(currentType);

                    if (currentConvention != null)
                        yield return new DefaultElementDescription(currentType, currentName, currentConvention);
                }

                var childCount = VisualTreeHelper.GetChildrenCount(current);
                if (childCount > 0)
                {
                    for(var i = 0; i < childCount; i++)
                    {
                        var childDo = VisualTreeHelper.GetChild(current, i);

                        if(childDo is UserControl)
                            continue;

                        queue.Enqueue(childDo);
                    }
                }
                else
                {
                    var contentControl = current as ContentControl;
                    if (contentControl != null)
                    {
                        if (contentControl.Content != null
                            && contentControl.Content is DependencyObject
                            && !(contentControl.Content is UserControl))
                            queue.Enqueue(contentControl.Content as DependencyObject);
                    }
                }
            }
        }
#endif
    }
}