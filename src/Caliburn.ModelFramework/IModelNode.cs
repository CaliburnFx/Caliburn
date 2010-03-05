namespace Caliburn.ModelFramework
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Implemented by different types of models.
    /// </summary>
    public interface IModelNode : IEditableObject, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        bool IsDirty { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        bool IsValid { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is in edit mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in edit mode; otherwise, <c>false</c>.
        /// </value>
        bool IsEditing { get; }

        /// <summary>
        /// Gets or sets the parent model.
        /// </summary>
        /// <value>The parent.</value>
        IModelNode Parent { get; set; }

        /// <summary>
        /// Occurs when the model has changed.
        /// </summary>
        event Action<IModelNode, IUndoRedoHandle> ModelChanged;

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        void Accept(IModelVisitor visitor);

        /// <summary>
        /// Fires a property change notification.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        void NotifyOfPropertyChange(string propertyName);

        /// <summary>
        /// Bubbles a property change notification.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        void BubblePropertyChange(string propertyName);

        /// <summary>
        /// Bubbles a model change notification.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="handle">The handle.</param>
        void BubbleModelChange(IModelNode source, IUndoRedoHandle handle);
    }
}