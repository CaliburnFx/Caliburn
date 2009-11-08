namespace Caliburn.ModelFramework
{
    /// <summary>
    /// A handle for undoing or redoing a change made to a model.
    /// </summary>
    public interface IUndoRedoHandle
    {
        /// <summary>
        /// Undoes a change.
        /// </summary>
        void Undo();

        /// <summary>
        /// Redoes a change.
        /// </summary>
        void Redo();
    }
}