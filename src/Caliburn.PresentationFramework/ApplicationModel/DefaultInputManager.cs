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
        private static readonly ILog Log = LogManager.GetLog(typeof(DefaultInputManager));

        private readonly List<IShortcut> _shortcuts = new List<IShortcut>();

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

        private void ElementToFocusEnabled(object sender, DependencyPropertyChangedEventArgs e)
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
            return _shortcuts;
        }

        /// <summary>
        /// Adds the shortcut.
        /// </summary>
        /// <param name="shortcut">The shortcut.</param>
        public void AddShortcut(IShortcut shortcut)
        {
            _shortcuts.Add(shortcut);
        }

        /// <summary>
        /// Removes the shortcut.
        /// </summary>
        /// <param name="shortcut">The shortcut.</param>
        public void RemoveShortcut(IShortcut shortcut)
        {
            _shortcuts.Remove(shortcut);
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
#if !SILVERLIGHT
            return new KeyGesture(key, modifierKeys).GetDisplayStringForCulture(CultureInfo.CurrentUICulture);
#else
            return CreateGestureText(key, modifierKeys);
#endif
        }

#if SILVERLIGHT

        private string CreateGestureText(Key key, ModifierKeys modifierKeys)
        {
            if (key == Key.None)
                return string.Empty;
            
            string gestureText = string.Empty;
            string keyText = CreateKeyText(key);

            if (!string.IsNullOrEmpty(keyText))
            {
                gestureText += CreateModifierText(modifierKeys);

                if (gestureText != string.Empty)
                    gestureText = gestureText + '+';

                gestureText = gestureText + keyText;
            }

            return gestureText;
        }

        private string CreateKeyText(Key key)
        {
            if (key == Key.None)
                return string.Empty;

            //if ((key >= Key.D0) && (key <= Key.D9))
            //    return char.ToString((char)((ushort)((key - 0x22) + 0x30)));

            //if ((key >= Key.A) && (key <= Key.Z))
            //    return char.ToString((char)((ushort)((key - 0x2c) + 0x41)));

            //string str = Abbreviate(key);

            //if ((str != null) && ((str.Length != 0) || (str == string.Empty)))
            //    return str;

            return key.ToString();
        }

        private string CreateModifierText(ModifierKeys modifiers)
        {
            string str = "";

            if ((modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                str = str + Abbreviate(ModifierKeys.Control);

            if ((modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                if (str.Length > 0)
                    str = str + '+';
                str = str + Abbreviate(ModifierKeys.Alt);
            }

            if ((modifiers & ModifierKeys.Windows) == ModifierKeys.Windows)
            {
                if (str.Length > 0)
                    str = str + '+';
                str = str + Abbreviate(ModifierKeys.Windows);
            }

            if ((modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                return str;

            if (str.Length > 0)
                str = str + '+';

            return (str + Abbreviate(ModifierKeys.Shift));
        }

        private static string Abbreviate(ModifierKeys modifierKeys)
        {
            string str = string.Empty;

            switch (modifierKeys)
            {
                case ModifierKeys.Alt:
                    return "Alt";

                case ModifierKeys.Control:
                    return "Ctrl";

                case (ModifierKeys.Control | ModifierKeys.Alt):
                    return str;

                case ModifierKeys.Shift:
                    return "Shift";

                case ModifierKeys.Windows:
                    return "Windows";
            }

            return str;
        }

        //private static string Abbreviate(Key key)
        //{
        //    switch (key)
        //    {
        //        case Key.Back:
        //            return "Backspace";

        //        case Key.Escape:
        //            return "Esc";

        //        case Key.None:
        //            return string.Empty;
        //    }

        //    return null;
        //}

#endif

        private void OnKeyUp(object s, KeyEventArgs e)
        {
            foreach (var shortcut in _shortcuts)
            {
                if (e.Key == shortcut.Key && Keyboard.Modifiers == shortcut.Modifers)
                {
                    if (shortcut.CanExecute)
                        Coroutine.Execute(shortcut.Execute());
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