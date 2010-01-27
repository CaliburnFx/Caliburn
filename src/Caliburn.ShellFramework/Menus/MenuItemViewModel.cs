namespace Caliburn.ShellFramework.Menus
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows.Controls;
    using Core;
    using Microsoft.Practices.ServiceLocation;
    using PresentationFramework;
    using PresentationFramework.ApplicationModel;
    using Resources;

    public class MenuItemViewModel : PropertyChangedBase, IMenuItem
    {
        private readonly Func<IEnumerable<IResult>> _execute;
        private readonly Func<bool> _canExecute = () => true;
        private string _text;
        private Image _icon;
        private IMenuItem _parent;

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

        public string Name
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

        public MenuItemViewModel WithIcon()
        {
            return WithIcon(Assembly.GetCallingAssembly(), "Resources/Icons/" + Name.Replace(" ", string.Empty) + ".png");
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