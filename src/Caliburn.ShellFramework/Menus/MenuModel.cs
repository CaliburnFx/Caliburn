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
    using PresentationFramework.RoutedMessaging;
    using Resources;

    public class MenuModel : PropertyChangedBase, IMenu, IShortcut
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
        private IMenu _parent;
        private ModifierKeys _modifiers;
        private Key _key;

        public static MenuModel Separator
        {
            get { return new MenuModel { IsSeparator = true }; }
        }

        public MenuModel()
        {
            Children = new BindableCollection<IMenu>();
        }

        public MenuModel(string text) 
            : this()
        {
            Text = text;
        }

        public MenuModel(string text, Func<IEnumerable<IResult>> execute)
            : this(text)
        {
            _execute = execute;
        }

        public MenuModel(string text, Func<IEnumerable<IResult>> execute, Func<bool> canExecute)
            : this(text, execute)
        {
            _canExecute = canExecute;
        }

        public IMenu Parent
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
            get { return _inputManager.GetDisplayString(_key, _modifiers); }
        }

        public IObservableCollection<IMenu> Children { get; private set; }

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

        public void Add(params IMenu[] menuItems)
        {
            menuItems.Apply(x => x.Parent = this);
            menuItems.Apply(Children.Add);
        }

        public int IndexOf(IMenu item)
        {
            return Children.IndexOf(item);
        }

        public void Insert(int index, IMenu menuItem)
        {
            menuItem.Parent = this;
            Children.Insert(index, menuItem);
        }

        public bool Remove(IMenu menuItem)
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

        public MenuModel WithGlobalShortcut(ModifierKeys modifiers, Key key)
        {
            _modifiers = modifiers;
            _key = key;

            _inputManager.AddShortcut(this);

            return this;
        }

        public MenuModel WithIcon()
        {
            return WithIcon(Assembly.GetCallingAssembly(), "Resources/Icons/" + DisplayName.Replace(" ", string.Empty) + ".png");
        }

        public MenuModel WithIcon(string path)
        {
            return WithIcon(Assembly.GetCallingAssembly(), path);
        }

        public MenuModel WithIcon(Assembly source, string path)
        {
            var manager = ServiceLocator.Current.GetInstance<IResourceManager>();
            var iconSource = manager.GetBitmap(path, source.GetAssemblyName());

            if (source != null)
                _icon = new Image { Source = iconSource };

            return this;
        }

        public IEnumerator<IMenu> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}