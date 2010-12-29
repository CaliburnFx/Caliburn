namespace Caliburn.ShellFramework
{
    using System;
    using System.Collections.Generic;
    using Core.Logging;
    using PresentationFramework.Screens;

    /// <summary>
    /// Represents and activation scope for child elements.
    /// </summary>
    /// <typeparam name="TScope"></typeparam>
    /// <typeparam name="TInScope"></typeparam>
    public class Scope<TScope, TInScope>
        where TScope : IActivate, IDeactivate
        where TInScope : IList<TInScope>, IChild<TInScope>
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(Scope<TScope, TInScope>));

        private readonly List<Instruction> _toAdd = new List<Instruction>();
        private readonly List<Instruction> _toRemove = new List<Instruction>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Scope&lt;TScope, TInScope&gt;"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        public Scope(TScope scope)
        {
            scope.Activated += ScopeActivated;
            scope.Deactivated += ScopedDeactivated;
        }

        /// <summary>
        /// Adds the child to the parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="child">The child.</param>
        /// <returns>The scope.</returns>
        public Scope<TScope, TInScope> Add(TInScope parent, TInScope child)
        {
            return AddAt(parent, () => -1, child);
        }

        /// <summary>
        /// Adds an item before an existing item.
        /// </summary>
        /// <param name="exisiting">The exisiting item.</param>
        /// <param name="newInScope">The new item.</param>
        /// <returns>The scope.</returns>
        public Scope<TScope, TInScope> AddBefore(TInScope exisiting, TInScope newInScope)
        {
            return AddAt(exisiting.Parent, () => exisiting.Parent.IndexOf(exisiting), newInScope);
        }

        /// <summary>
        /// Adds an item after an existing item.
        /// </summary>
        /// <param name="exisiting">The existing item.</param>
        /// <param name="newInScope">The new item.</param>
        /// <returns>The scope.</returns>
        public Scope<TScope, TInScope> AddAfter(TInScope exisiting, TInScope newInScope)
        {
            return AddAt(exisiting.Parent, () => exisiting.Parent.IndexOf(exisiting) + 1, newInScope);
        }

        /// <summary>
        /// Adds an items to the parent at the specified index.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="index">The index.</param>
        /// <param name="child">The item.</param>
        /// <returns>The scope.</returns>
        public Scope<TScope, TInScope> AddAt(TInScope parent, Func<int> index, TInScope child)
        {
            _toAdd.Add(new Instruction {
                Parent = parent,
                Index = index,
                Child = child
            });

            return this;
        }

        /// <summary>
        /// Removes the specified child.
        /// </summary>
        /// <param name="child">The child.</param>
        /// <returns>The scope.</returns>
        public Scope<TScope, TInScope> Remove(TInScope child)
        {
            var index = child.Parent.IndexOf(child);

            _toRemove.Add(new Instruction {
                Parent = child.Parent,
                Child = child,
                Index = () => index
            });

            return this;
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