#if !SILVERLIGHT_20

namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using Core.Metadata;
    using Metadata;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// The default implemenation of <see cref="IInputManager"/>.
    /// </summary>
    public partial class DefaultInputManager : IInputManager
    {
        /// <summary>
        /// Focuses the view bound to the view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void Focus(object viewModel)
        {
#if !SILVERLIGHT
            var view = GetView(viewModel) as UIElement;
#else
            var view = GetView(viewModel) as Control;
#endif
            if (view != null) view.Focus();
        }

        /// <summary>
        /// Focuses the control bound to the property on the view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="propertyPath">The property path.</param>
        public void Focus(object viewModel, string propertyPath)
        {
            var view = GetView(viewModel);
            if (view != null) return;

            var elements = GetAllElements(view);

            foreach(var element in elements)
            {
                var paths = GetBindingPaths(element);

                foreach(var path in paths)
                {
                    if(string.Compare(path, propertyPath, StringComparison.InvariantCultureIgnoreCase) != 0) 
                        continue;

                    element.Focus();
                    return;
                }
            }
        }

        /// <summary>
        /// Gets the view bound to the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        protected virtual DependencyObject GetView(object model)
        {
            var container = model as IMetadataContainer;
            if (container == null) return null;

            return container.GetView<DependencyObject>(null);
        }

        private static IEnumerable<string> GetBindingPaths(DependencyObject element)
        {
            var properties = GetDependencyProperties(element);

            foreach (var property in properties)
            {
                var expression = element.ReadLocalValue(property) as BindingExpression;

                if(expression != null &&
                   expression.ParentBinding != null &&
                   expression.ParentBinding.Path != null)
                    yield return expression.ParentBinding.Path.Path;
            }
        }

        private static IEnumerable<DependencyProperty> GetDependencyProperties(DependencyObject element)
        {
            return from prop in element.GetType().GetFields(BindingFlags.Public | BindingFlags.Static| BindingFlags.FlattenHierarchy)
                   where typeof(DependencyProperty).IsAssignableFrom(prop.FieldType)
                   select prop.GetValue(null) as DependencyProperty;
        }
    }
}

#endif