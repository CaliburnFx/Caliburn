#if !SILVERLIGHT_20

namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Implemented by services that provide focus and key binding functionality.
    /// </summary>
    public interface IInputManager
    {
        /// <summary>
        /// Focuses the view bound to the view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        void Focus(object viewModel);

        /// <summary>
        /// Focuses the control bound to the property on the view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="propertyPath">The property path.</param>
        void Focus(object viewModel, string propertyPath);

        /// <summary>
        /// Gets all shortcuts.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IShortcut> GetAllShortcuts();

        /// <summary>
        /// Adds the shortcut.
        /// </summary>
        /// <param name="shortcut">The shortcut.</param>
        void AddShortcut(IShortcut shortcut);

        /// <summary>
        /// Removes the shortcut.
        /// </summary>
        /// <param name="shortcut">The shortcut.</param>
        void RemoveShortcut(IShortcut shortcut);

        /// <summary>
        /// Registers the shortcut source.
        /// </summary>
        /// <param name="element">The element.</param>
        void RegisterShortcutSource(UIElement element);

        /// <summary>
        /// Unregisters the shortcut source.
        /// </summary>
        /// <param name="element">The element.</param>
        void UnregisterShortcutSource(UIElement element);

        /// <summary>
        /// Gets a display string representing this key combination.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="modifierKeys">The modifier keys.</param>
        /// <returns>The display string.</returns>
        string GetDisplayString(Key key, ModifierKeys modifierKeys);
    }
}

#endif