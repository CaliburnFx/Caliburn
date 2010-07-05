namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

#if !SILVERLIGHT
    public partial class DefaultInputManager
    {
        private static IEnumerable<UIElement> GetAllElements(DependencyObject root)
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var element = current as UIElement;

                if (element != null)
                    yield return element;

                foreach (object child in LogicalTreeHelper.GetChildren(current))
                {
                    var childDo = child as DependencyObject;
					if (childDo != null) 
					    queue.Enqueue(childDo);
                }
            }
        }
    }
#else
    public partial class DefaultInputManager
    {
        private static IEnumerable<Control> GetAllElements(DependencyObject root)
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var element = current as Control;

                if (element != null)
                    yield return element;

                var childCount = VisualTreeHelper.GetChildrenCount(current);
                if (childCount > 0)
                {
                    for(var i = 0; i < childCount; i++)
                    {
                        var childDo = VisualTreeHelper.GetChild(current, i);
                        queue.Enqueue(childDo);
                    }
                }
                else
                {
                    var contentControl = current as ContentControl;
                    if (contentControl != null)
                    {
                        if (contentControl.Content != null
                            && contentControl.Content is DependencyObject)
                            queue.Enqueue(contentControl.Content as DependencyObject);
                    }
                }
            }
        }
    }
#endif
}