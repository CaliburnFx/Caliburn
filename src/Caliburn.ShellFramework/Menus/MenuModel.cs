namespace Caliburn.ShellFramework.Menus
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows.Controls;
    using System.Windows.Input;
    using PresentationFramework;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
    using Resources;

    public class MenuModel : BindableCollection<IMenu>, IMenu, IShortcut
    {
        private static IInputManager inputManager;
        static IResourceManager resourceManager;

        public static void Initialize(IInputManager inputManager, IResourceManager resourceManager)
        {
            MenuModel.inputManager = inputManager;
            MenuModel.resourceManager = resourceManager;
        }

        private readonly Func<IEnumerable<IResult>> execute;
        private readonly Func<bool> canExecute = () => true;
        private string text;
        private IMenu parent;
        private ModifierKeys modifiers;
        private Key key;
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
            this.execute = execute;
        }

        public MenuModel(string text, Func<IEnumerable<IResult>> execute, Func<bool> canExecute)
            : this(text, execute)
        {
            this.canExecute = canExecute;
        }

        public IMenu Parent
        {
            get { return parent; }
            set
            {
                parent = value; 
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
            get { return text; }
            set
            {
                text = value;
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
            get { return inputManager.GetDisplayString(key, modifiers); }
        }

        public IObservableCollection<IMenu> Children
        {
            get { return this; }
        }

        public bool CanExecute
        {
            get { return canExecute(); }
        }

        public IEnumerable<IResult> Execute()
        {
            return execute != null ? execute() : new IResult[] {};
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
            get { return modifiers; }
            set { modifiers = value; }
        }

        Key IShortcut.Key
        {
            get { return key; }
            set { key = value; }
        }

        public MenuModel WithGlobalShortcut(ModifierKeys modifiers, Key key)
        {
            this.modifiers = modifiers;
            this.key = key;
            inputManager.AddShortcut(this);
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
            var iconSource = resourceManager.GetBitmap(path, source.GetAssemblyName());

            if (source != null)
                Icon = new Image { Source = iconSource };

            return this;
        }
    }
}