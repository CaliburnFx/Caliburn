namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Core.Logging;
    using RoutedMessaging;
    using Views;

    /// <summary>
    /// The default implemenation of <see cref="IInputManager"/>.
    /// </summary>
    public partial class DefaultInputManager : IInputManager
    {
        static readonly ILog Log = LogManager.GetLog(typeof(DefaultInputManager));

        readonly List<IShortcut> shortcuts = new List<IShortcut>();

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
            var view = GetView(viewModel) as DependencyObject;
            if (view == null)
            {
                Log.Warn("View not found for {0}.", viewModel);
                return;
            }

            var elements = GetAllElements(view);

            foreach(var element in elements)
            {
                var paths = GetBindingPaths(element);

                foreach(var path in paths)
                {
                    if(string.Compare(path, propertyPath, StringComparison.InvariantCultureIgnoreCase) != 0)
                        continue;

                    if (!element.IsEnabled)
                        element.IsEnabledChanged += ElementToFocusEnabled;
                    else element.Focus();

                    return;
                }
            }

            Log.Warn("Element for {0} not found on {1}.", propertyPath, view);
        }

        void ElementToFocusEnabled(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(!((bool)e.NewValue))
                return;

#if NET
            ((UIElement)sender).Focus();
            ((UIElement)sender).IsEnabledChanged -= ElementToFocusEnabled;
#else
            ((Control)sender).Focus();
            ((Control)sender).IsEnabledChanged -= ElementToFocusEnabled;
#endif
        }

        /// <summary>
        /// Gets all shortcuts.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IShortcut> GetAllShortcuts()
        {
            return shortcuts;
        }

        /// <summary>
        /// Adds the shortcut.
        /// </summary>
        /// <param name="shortcut">The shortcut.</param>
        public void AddShortcut(IShortcut shortcut)
        {
            shortcuts.Add(shortcut);
        }

        /// <summary>
        /// Removes the shortcut.
        /// </summary>
        /// <param name="shortcut">The shortcut.</param>
        public void RemoveShortcut(IShortcut shortcut)
        {
            shortcuts.Remove(shortcut);
        }

        /// <summary>
        /// Registers the shortcut source.
        /// </summary>
        /// <param name="element">The element.</param>
        public void RegisterShortcutSource(UIElement element)
        {
#if !SILVERLIGHT
            element.PreviewKeyUp += OnKeyUp;
#else
            element.KeyUp += OnKeyUp;
#endif
        }

        /// <summary>
        /// Unregisters the shortcut source.
        /// </summary>
        /// <param name="element">The element.</param>
        public void UnregisterShortcutSource(UIElement element)
        {
#if !SILVERLIGHT
            element.PreviewKeyUp -= OnKeyUp;
#else
            element.KeyUp -= OnKeyUp;
#endif
        }

        public string GetDisplayString(Key key, ModifierKeys modifierKeys)
        {
            return new KeyGesture(key, modifierKeys).GetDisplayStringForCulture(CultureInfo.CurrentUICulture);
        }

        void OnKeyUp(object s, KeyEventArgs e)
        {
            foreach (var shortcut in shortcuts)
            {
                if (e.Key == shortcut.Key && Keyboard.Modifiers == shortcut.Modifers)
                {
                    if (shortcut.CanExecute)
                        Coroutine.BeginExecute(shortcut.Execute());
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the view bound to the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        protected virtual object GetView(object model)
        {
            var viewAware = model as IViewAware;
            if (viewAware == null) return null;

            return viewAware.GetView(null);
        }

        static IEnumerable<string> GetBindingPaths(DependencyObject element)
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

        static IEnumerable<DependencyProperty> GetDependencyProperties(DependencyObject element)
        {
            return from prop in element.GetType().GetFields(BindingFlags.Public | BindingFlags.Static| BindingFlags.FlattenHierarchy)
                   where typeof(DependencyProperty).IsAssignableFrom(prop.FieldType)
                   select prop.GetValue(null) as DependencyProperty;
        }
    }
}
