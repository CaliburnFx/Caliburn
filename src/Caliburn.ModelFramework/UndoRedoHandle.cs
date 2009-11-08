namespace Caliburn.ModelFramework
{
    using System;

    /// <summary>
    /// An action based implementation of <see cref="IUndoRedoHandle"/>.
    /// </summary>
    public class UndoRedoHandle : IUndoRedoHandle
    {
        private readonly Action _undo;
        private readonly Action _redo;

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoRedoHandle"/> class.
        /// </summary>
        /// <param name="undo">The undo.</param>
        /// <param name="redo">The redo.</param>
        public UndoRedoHandle(Action undo, Action redo)
        {
            _undo = undo;
            _redo = redo;
        }

        /// <summary>
        /// Undoes a change.
        /// </summary>
        public void Undo()
        {
            _undo();
        }

        /// <summary>
        /// Redoes a change.
        /// </summary>
        public void Redo()
        {
            _redo();
        }
    }
}