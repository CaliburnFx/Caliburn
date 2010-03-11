namespace Caliburn.ShellFramework.Menus
{
    using System;
    using System.Collections.Generic;
    using Core.Logging;
    using PresentationFramework.ApplicationModel;

    public class MenuScope
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(MenuScope));

        private readonly List<MenuInstruction> _toAdd = new List<MenuInstruction>();
        private readonly List<MenuInstruction> _toRemove = new List<MenuInstruction>();

        public MenuScope(ILifecycleNotifier scope)
        {
            scope.Activated += NotifierActivated;
            scope.Deactivated += NotifierDeactivated;
        }

        public MenuScope Add(IMenuItem parent, IMenuItem child)
        {
            return AddAt(parent, () => -1, child);
        }

        public MenuScope AddBefore(IMenuItem exisiting, IMenuItem newItem)
        {
            return AddAt(exisiting.Parent, () => exisiting.Parent.IndexOf(exisiting), newItem);
        }

        public MenuScope AddAfter(IMenuItem exisiting, IMenuItem newItem)
        {
            return AddAt(exisiting.Parent, () => exisiting.Parent.IndexOf(exisiting) + 1, newItem);
        }

        public MenuScope AddAt(IMenuItem parent, Func<int> index, IMenuItem child)
        {
            _toAdd.Add(new MenuInstruction
            {
                Parent = parent,
                Index = index,
                Child = child
            });

            return this;
        }

        public void Remove(IMenuItem child)
        {
            var index = child.Parent.IndexOf(child);

            _toRemove.Add(new MenuInstruction
            {
                Parent = child.Parent,
                Child = child,
                Index = () => index
            });
        }

        private void NotifierActivated(object sender, EventArgs e)
        {
            Log.Info("Activating menus for {0}.", sender);

            foreach (var instruction in _toAdd)
            {
                var index = instruction.Index();

                if (index == -1)
                    instruction.Parent.Add(instruction.Child);
                else instruction.Parent.Insert(index, instruction.Child);
            }

            foreach(var instruction in _toRemove)
            {
                instruction.Parent.Remove(instruction.Child);
            }
        }

        private void NotifierDeactivated(object sender, EventArgs e)
        {
            Log.Info("Deactivating menus for {0}.", sender);

            foreach (var instruction in _toAdd)
            {
                instruction.Parent.Remove(instruction.Child);
            }

            foreach (var instruction in _toRemove)
            {
                instruction.Parent.Insert(instruction.Index(), instruction.Child);
            }
        }

        private class MenuInstruction
        {
            public IMenuItem Parent { get; set; }
            public Func<int> Index { get; set; }
            public IMenuItem Child { get; set; }
        }
    }
}