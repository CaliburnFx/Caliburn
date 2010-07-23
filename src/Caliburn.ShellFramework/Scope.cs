namespace Caliburn.ShellFramework
{
    using System;
    using System.Collections.Generic;
    using Core.Logging;
    using PresentationFramework.Screens;

    public class Scope<TScope, TInScope>
        where TScope : IActivate, IDeactivate
        where TInScope : IList<TInScope>, IChild<TInScope>
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(Scope<TScope, TInScope>));

        private readonly List<Instruction> _toAdd = new List<Instruction>();
        private readonly List<Instruction> _toRemove = new List<Instruction>();

        public Scope(TScope scope)
        {
            scope.Activated += ScopeActivated;
            scope.Deactivated += ScopedDeactivated;
        }

        public Scope<TScope, TInScope> Add(TInScope parent, TInScope child)
        {
            return AddAt(parent, () => -1, child);
        }

        public Scope<TScope, TInScope> AddBefore(TInScope exisiting, TInScope newInScope)
        {
            return AddAt(exisiting.Parent, () => exisiting.Parent.IndexOf(exisiting), newInScope);
        }

        public Scope<TScope, TInScope> AddAfter(TInScope exisiting, TInScope newInScope)
        {
            return AddAt(exisiting.Parent, () => exisiting.Parent.IndexOf(exisiting) + 1, newInScope);
        }

        public Scope<TScope, TInScope> AddAt(TInScope parent, Func<int> index, TInScope child)
        {
            _toAdd.Add(new Instruction {
                Parent = parent,
                Index = index,
                Child = child
            });

            return this;
        }

        public void Remove(TInScope child)
        {
            var index = child.Parent.IndexOf(child);

            _toRemove.Add(new Instruction {
                Parent = child.Parent,
                Child = child,
                Index = () => index
            });
        }

        private void ScopeActivated(object sender, EventArgs e)
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

        private void ScopedDeactivated(object sender, EventArgs e)
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

        private class Instruction
        {
            public TInScope Parent { get; set; }
            public Func<int> Index { get; set; }
            public TInScope Child { get; set; }
        }
    }
}