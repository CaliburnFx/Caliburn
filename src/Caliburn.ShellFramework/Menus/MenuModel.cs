namespace Caliburn.ShellFramework.Menus
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.Practices.ServiceLocation;
    using PresentationFramework;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
    using Resources;

    public class MenuModel : BindableCollection<IMenu>, IMenu, IShortcut
    {
        private static IInputManager _inputManager;

        public static void Initialize(IInputManager inputManager)
        {
            _inputManager = inputManager;
        }

        private readonly Func<IEnumerable<IResult>> _execute;
        private readonly Func<bool> _canExecute = () => true;
        private string _text;
        private IMenu _parent;
        private ModifierKeys _modifiers;
        private Key _key;
        private string displayName;

        public static MenuModel Separator
        {
            get { return new MenuModel { IsSeparator = true }; }
        }

        public MenuModel() {}

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
            set
            {
                _parent = value; 
                OnPropertyChanged(new PropertyChangedEventArgs("Parent"));
            }
        }

        public string DisplayName
        {
            get { return displayName; }
            set
            {
                displayName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DisplayName"));
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Text"));
                DisplayName = string.IsNullOrEmpty(Text) ? null : Text.Replace("_", string.Empty);
            }
        }

        public Image Icon { get; private set; }
        public bool IsSeparator { get; private set; }

        public string ActionText
        {
            get { return "Execute"; }
        }

        public string InputGestureText
        {
            get { return _inputManager.GetDisplayString(_key, _modifiers); }
        }

        public IObservableCollection<IMenu> Children
        {
            get { return this; }
        }

        public bool CanExecute
        {
            get { return _canExecute(); }
        }

        public IEnumerable<IResult> Execute()
        {
            return _execute != null ? _execute() : new IResult[] {};
        }

        protected override void SetItem(int index, IMenu item)
        {
            base.SetItem(index, item);
            item.Parent = this;
        }

        protected override void InsertItem(int index, IMenu item)
        {
            base.InsertItem(index, item);
            item.Parent = this;
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
                Icon = new Image { Source = iconSource };

            return this;
        }
    }
}