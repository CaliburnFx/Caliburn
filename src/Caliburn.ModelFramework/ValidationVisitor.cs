namespace Caliburn.ModelFramework
{
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="IModelVisitor"/> that validates a model.
    /// </summary>
    public class ValidationVisitor : IModelVisitor
    {
        private readonly List<IValidationResult> _result = new List<IValidationResult>();

        /// <summary>
        /// Gets the result of the validation process.
        /// </summary>
        /// <value>The result.</value>
        public IList<IValidationResult> Result
        {
            get { return _result; }
        }

        /// <summary>
        /// Visits an <see cref="IModel"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">The model.</param>
        public void VisitModel<T>(T model)
            where T : IModel
        {
            model.ValidationResults.Clear();

            model.UseInterrogators<IModelValidator<T>>(
                interrogators =>{
                    foreach(var validator in interrogators)
                    {
                        if(!validator.Interrogate(model)) return;
                    }
                });

            _result.AddRange(model.ValidationResults);

            model.NotifyOfPropertyChange("IsValid");
        }

        /// <summary>
        /// Visits an <see cref="IProperty{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">The property.</param>
        public void VisitProperty<T>(IProperty<T> property)
        {
            property.ValidationResults.Clear();

            property.UseInterrogators<IPropertyValidator<T>>(
                interrogators =>{
                    foreach(var validator in interrogators)
                    {
                        if(!validator.Interrogate(property)) return;
                    }
                });

            _result.AddRange(property.ValidationResults);

            property.NotifyOfPropertyChange("IsValid");
        }

        /// <summary>
        /// Visits a <see cref="ICollectionNode{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionNode">The presentation collection.</param>
        public void VisitCollection<T>(ICollectionNode<T> collectionNode)
        {
            collectionNode.NotifyOfPropertyChange("IsValid");
        }
    }
}