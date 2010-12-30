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
    using PresentationFramework.Screens;
    using Resources;

    /// <summary>
    /// The default implementation of <see cref="IMenu"/>.
    /// </summary>
    public class MenuModel : BindableCollection<IMenu>, IMenu, IShortcut
    {
        static IInputManager inputManager;
        static IResourceManager resourceManager;

        /// <summary>
        /// Initializes the menu model system.
        /// </summary>
        /// <param name="inputManager">The input manager.</param>
        /// <param name="resourceManager">The resource manager.</param>
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

        /// <summary>
        /// Gets the separator model.
        /// </summary>
        /// <value>The separator.</value>
        public static MenuModel Separator
        {
            get { return new MenuModel { IsSeparator = true }; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuModel"/> class.
        /// </summary>
        public MenuModel() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuModel"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public MenuModel(string text) 
            : this()
        {
            Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuModel"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="execute">The action.</param>
        public MenuModel(string text, Func<IEnumerable<IResult>> execute)
            : this(text)
        {
            this.execute = execute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuModel"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="execute">The action.</param>
        /// <param name="canExecute">The action guard.</param>
        public MenuModel(string text, Func<IEnumerable<IResult>> execute, Func<bool> canExecute)
            : this(text, execute)
        {
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Gets or Sets the Parent
        /// </summary>
        /// <value></value>
        public IMenu Parent
        {
            get { return parent; }
            set
            {
                parent = value; 
                OnPropertyChanged(new PropertyChangedEventArgs("Parent"));
            }
        }

        /// <summary>
        /// Gets or Sets the Parent
        /// </summary>
        /// <value></value>
        object IChild.Parent
        {
            get { return Parent; }
            set { Parent = (IMenu)value; }
        }

        /// <summary>
        /// Gets or Sets the Display Name
        /// </summary>
        /// <value></value>
        public string DisplayName
        {
            get { return displayName; }
            set
            {
                displayName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DisplayName"));
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
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

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        public Image Icon { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is separator.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is separator; otherwise, <c>false</c>.
        /// </value>
        public bool IsSeparator { get; private set; }

        /// <summary>
        /// Gets the action text.
        /// </summary>
        /// <value>The action text.</value>
        public string ActionText
        {
            get { return "Execute"; }
        }

        /// <summary>
        /// Gets the input gesture text.
        /// </summary>
        /// <value>The input gesture text.</value>
        public string InputGestureText
        {
            get { return inputManager.GetDisplayString(key, modifiers); }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public IObservableCollection<IMenu> Children
        {
            get { return this; }
        }

        /// <summary>
        /// Gets a value indicating whether the action can execute.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if it can execute; otherwise, <c>false</c>.
        /// </value>
        public bool CanExecute
        {
            get { return canExecute(); }
        }

        /// <summary>
        /// Executes this menu action.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IResult> Execute()
        {
            return execute != null ? execute() : new IResult[] {};
        }

        /// <summary>
        /// Sets the item at the specified position.
        /// </summary>
        /// <param name="index">The index to set the item at.</param>
        /// <param name="item">The item to set.</param>
        protected override void SetItem(int index, IMenu item)
        {
            base.SetItem(index, item);
            item.Parent = this;
        }

        /// <summary>
        /// Inserts the item to the specified position.
        /// </summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="item">The item to be inserted.</param>
        protected override void InsertItem(int index, IMenu item)
        {
            base.InsertItem(index, item);
            item.Parent = this;
        }

        /// <summary>
        /// Gets or sets the modifers.
        /// </summary>
        /// <value>The modifers.</value>
        ModifierKeys IShortcut.Modifers
        {
            get { return modifiers; }
            set { modifiers = value; }
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        Key IShortcut.Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// Adds a global shortcut to the menu item.
        /// </summary>
        /// <param name="modifiers">The modifiers.</param>
        /// <param name="key">The key.</param>
        /// <returns>The item.</returns>
        public MenuModel WithGlobalShortcut(ModifierKeys modifiers, Key key)
        {
            this.modifiers = modifiers;
            this.key = key;
            inputManager.AddShortcut(this);
            return this;
        }

        /// <summary>
        /// Adds an icon using the default location resolution to the menu item in the calling assembly.
        /// </summary>
        /// <returns>The item.</returns>
        public MenuModel WithIcon()
        {
            return WithIcon(Assembly.GetCallingAssembly(), ResourceExtensions.DetermineIconPath(DisplayName));
        }

        /// <summary>
        /// Adds an icon to the menu item, searching the calling assmebly for the resource.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The item.</returns>
        public MenuModel WithIcon(string path)
        {
            return WithIcon(Assembly.GetCallingAssembly(), path);
        }

        /// <summary>
        /// Adds an icon to the menu item.
        /// </summary>
        /// <param name="source">The source assmelby.</param>
        /// <param name="path">The path.</param>
        /// <returns>The item.</returns>
        public MenuModel WithIcon(Assembly source, string path)
        {
            var iconSource = resourceManager.GetBitmap(path, source.GetAssemblyName());

            if (iconSource != null)
                Icon = new Image { Source = iconSource };

            return this;
        }
    }
}