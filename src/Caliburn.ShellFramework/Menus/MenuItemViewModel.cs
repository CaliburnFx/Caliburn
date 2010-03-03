namespace Caliburn.ShellFramework.Menus
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Core;
    using Microsoft.Practices.ServiceLocation;
    using PresentationFramework;
    using PresentationFramework.ApplicationModel;
    using Resources;

    public class MenuItemViewModel : PropertyChangedBase, IMenuItem, IShortcut
    {
        private static IInputManager _inputManager;

        public static void Initialize(IInputManager inputManager)
        {
            _inputManager = inputManager;
        }

        private readonly Func<IEnumerable<IResult>> _execute;
        private readonly Func<bool> _canExecute = () => true;
        private string _text;
        private Image _icon;
        private IMenuItem _parent;
        private ModifierKeys _modifiers;
        private Key _key;

        public static MenuItemViewModel Separator
        {
            get { return new MenuItemViewModel { IsSeparator = true }; }
        }

        private MenuItemViewModel()
        {
            Children = new BindableCollection<IMenuItem>();
        }

        public MenuItemViewModel(string text) 
            : this()
        {
            Text = text;
        }

        public MenuItemViewModel(string text, Func<IEnumerable<IResult>> execute)
            : this(text)
        {
            _execute = execute;
        }

        public MenuItemViewModel(string text, Func<IEnumerable<IResult>> execute, Func<bool> canExecute)
            : this(text, execute)
        {
            _canExecute = canExecute;
        }

        public IMenuItem Parent
        {
            get { return _parent; }
            set { _parent = value; NotifyOfPropertyChange(() => Parent); }
        }

        public string DisplayName
        {
            get { return string.IsNullOrEmpty(Text) ? null : Text.Replace("_", string.Empty); }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; NotifyOfPropertyChange(() => Text); }
        }

        public Image Icon
        {
            get { return _icon; }
        }

        public bool IsSeparator { get; private set; }

        public string ActionText
        {
            get { return "Execute"; }
        }

        public string InputGestureText
        {
            get
            {
#if !SILVERLIGHT
                return new KeyGesture(_key, _modifiers).GetDisplayStringForCulture(CultureInfo.CurrentUICulture);
#else
                return CreateGestureText();
#endif
            }
        }

#if SILVERLIGHT

        private string CreateGestureText()
        {
            if (_key == Key.None)
                return string.Empty;
            
            string gestureText = string.Empty;
            string keyText = CreateKeyText();

            if (!string.IsNullOrEmpty(keyText))
            {
                gestureText += CreateModifierText();

                if (gestureText != string.Empty)
                    gestureText = gestureText + '+';

                gestureText = gestureText + keyText;
            }

            return gestureText;
        }

        private string CreateKeyText()
        {
            if (_key == Key.None)
                return string.Empty;

            if ((_key >= Key.D0) && (_key <= Key.D9))
                return char.ToString((char)((ushort)((_key - 0x22) + 0x30)));

            if ((_key >= Key.A) && (_key <= Key.Z))
                return char.ToString((char)((ushort)((_key - 0x2c) + 0x41)));

            string str = Abbreviate(_key);

            if ((str != null) && ((str.Length != 0) || (str == string.Empty)))
                return str;

            return _key.ToString();
        }

        private string CreateModifierText()
        {
            string str = "";

            if ((_modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                str = str + Abbreviate(ModifierKeys.Control);

            if ((_modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                if (str.Length > 0)
                    str = str + '+';
                str = str + Abbreviate(ModifierKeys.Alt);
            }

            if ((_modifiers & ModifierKeys.Windows) == ModifierKeys.Windows)
            {
                if (str.Length > 0)
                    str = str + '+';
                str = str + Abbreviate(ModifierKeys.Windows);
            }

            if ((_modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
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

        private static string Abbreviate(Key key)
        {
            switch (key)
            {
                case Key.Back:
                    return "Backspace";

                case Key.Escape:
                    return "Esc";

                case Key.None:
                    return string.Empty;
            }

            return null;
        }

#endif

        public IObservableCollection<IMenuItem> Children { get; private set; }

        public bool CanExecute
        {
            get { return _canExecute(); }
        }

        public IEnumerable<IResult> Execute()
        {
            return _execute != null ? _execute() : new IResult[] {};
        }

        public MenuScope CreateScope(ILifecycleNotifier scope)
        {
            return new MenuScope(scope);
        }

        public void Add(params IMenuItem[] menuItems)
        {
            menuItems.Apply(x => x.Parent = this);
            menuItems.Apply(Children.Add);
        }

        public int IndexOf(IMenuItem item)
        {
            return Children.IndexOf(item);
        }

        public void Insert(int index, IMenuItem menuItem)
        {
            menuItem.Parent = this;
            Children.Insert(index, menuItem);
        }

        public bool Remove(IMenuItem menuItem)
        {
            return Children.Remove(menuItem);
        }

        ModifierKeys IShortcut.Modifers
        {
            get { return _modifiers; }
            set { _modifiers = value; }
        }

        Key IShortcut.Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public MenuItemViewModel WithGlobalShortcut(ModifierKeys modifiers, Key key)
        {
            _modifiers = modifiers;
            _key = key;

            _inputManager.AddShortcut(this);

            return this;
        }

        public MenuItemViewModel WithIcon()
        {
            return WithIcon(Assembly.GetCallingAssembly(), "Resources/Icons/" + DisplayName.Replace(" ", string.Empty) + ".png");
        }

        public MenuItemViewModel WithIcon(string path)
        {
            return WithIcon(Assembly.GetCallingAssembly(), path);
        }

        public MenuItemViewModel WithIcon(Assembly source, string path)
        {
            var manager = ServiceLocator.Current.GetInstance<IResourceManager>();
            var iconSource = manager.GetBitmap(path, source.GetAssemblyName());

            if (source != null)
                _icon = new Image { Source = iconSource };

            return this;
        }

        public IEnumerator<IMenuItem> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}