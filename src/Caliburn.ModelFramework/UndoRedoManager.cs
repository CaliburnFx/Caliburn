namespace Caliburn.ModelFramework
{
    using System;
    using System.Collections.Generic;
    using PresentationFramework;

    /// <summary>
    /// Catches and manages all undo/redo events for an instance of <see cref="IModelNode"/>.
    /// </summary>
    public class UndoRedoManager : PropertyChangedBase
    {
        private readonly Stack<IUndoRedoHandle> _undoStack = new Stack<IUndoRedoHandle>();
        private readonly Stack<IUndoRedoHandle> _redoStack = new Stack<IUndoRedoHandle>();

        /// <summary>
        /// Registers the specified model for change tracking.
        /// </summary>
        /// <param name="trackable">The model to track changes on.</param>
        public void Register(IModelNode trackable)
        {
            trackable.ModelChanged += ModelToManage_ModelChanged;
        }

        /// <summary>
        /// Unregisters change tracking on the specified model.
        /// </summary>
        /// <param name="trackable">The model to end change tracking on.</param>
        public void Unregister(IModelNode trackable)
        {
            trackable.ModelChanged -= ModelToManage_ModelChanged;
        }

        /// <summary>
        /// Gets a value indicating whether this instance can undo changes on a tracked instance.
        /// </summary>
        /// <value><c>true</c> if this instance can undo; otherwise, <c>false</c>.</value>
        public bool CanUndo
        {
            get { return _undoStack.Count > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can redo changes on a tracked instance.
        /// </summary>
        /// <value><c>true</c> if this instance can redo; otherwise, <c>false</c>.</value>
        public bool CanRedo
        {
            get { return _redoStack.Count > 0; }
        }

        /// <summary>
        /// Clears the tracked changes.
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();

            NotifyOfPropertyChange("CanUndo");
            NotifyOfPropertyChange("CanRedo");
        }

        /// <summary>
        /// Undoes the last change.
        /// </summary>
        /// <returns></returns>
        public bool Undo()
        {
            if(!CanUndo) return false;

            var handle = _undoStack.Pop();
            _redoStack.Push(handle);

            handle.Undo();

            NotifyOfPropertyChange("CanUndo");
            NotifyOfPropertyChange("CanRedo");

            return true;
        }

        /// <summary>
        /// Redoes the last change.
        /// </summary>
        /// <returns></returns>
        public bool Redo()
        {
            if(!CanRedo) return false;

            var handle = _redoStack.Pop();
            _undoStack.Push(handle);

            handle.Redo();

            NotifyOfPropertyChange("CanUndo");
            NotifyOfPropertyChange("CanRedo");

            return true;
        }

        /// <summary>
        /// Pushes the specified <see cref="IUndoRedoHandle"/> onto the undo stack.
        /// </summary>
        /// <param name="handle">The handle.</param>
        public void Push(IUndoRedoHandle handle)
        {
            _redoStack.Clear();
            _undoStack.Push(handle);

            NotifyOfPropertyChange("CanUndo");
            NotifyOfPropertyChange("CanRedo");
        }

        /// <summary>
        /// Pushes the specified undo and redo actions onto the stack.
        /// </summary>
        /// <param name="undoAction">The undo action.</param>
        /// <param name="redoAction">The redo action.</param>
        public void Push(Action undoAction, Action redoAction)
        {
            Push(new UndoRedoHandle(undoAction, redoAction));
        }

        private void ModelToManage_ModelChanged(IModelNode modelNode, IUndoRedoHandle handle)
        {
            Push(handle);
        }
    }
}