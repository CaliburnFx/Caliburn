namespace Caliburn.PresentationFramework.Metadata
{
    using System;
    using Core.Metadata;
    using System.Collections.Generic;

    /// <summary>
    /// Metadata that stores the current view over a model.
    /// </summary>
    public class ViewMetadata : IMetadata
    {
        private static readonly object _defaultContext = new object();
        private readonly Dictionary<object, object> _views = new Dictionary<object, object>();

        /// <summary>
        /// Sets the view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="context">The context</param>
        /// <param name="useWeakReference">if set to <c>true</c> uses a weak reference.</param>
        public void SetView(object view, object context, bool useWeakReference)
        {
            _views[context ?? _defaultContext] = useWeakReference ? new WeakReference(view) : view;
        }

        /// <summary>
        /// Gets a strongly typed view.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetView<T>(object context)
        {
            object view;

            if(_views.TryGetValue(context ?? _defaultContext, out view))
            {
                var weakReference = view as WeakReference;

                if(weakReference != null)
                {
                    var target = weakReference.Target;

                    if (target != null) 
                        return (T)target;
                    return default(T);
                }

                return (T)view;
            }

            return default(T);
        }
    }
}