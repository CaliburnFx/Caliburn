namespace Caliburn.PresentationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Core.Metadata;
    using Metadata;

    /// <summary>
    /// Hosts extension methods and method overrides for execution of <see cref="IResult"/> and <see cref="IEnumerable{IResult}"/>
    /// </summary>
    public static class EnumerableResults
    {
        /// <summary>
        /// Executes an <see cref="IEnumerable{IResult}"/>, optionally looking up the <see cref="IInteractionNode"/>.
        /// </summary>
        public static Action<object, object, IEnumerable<IResult>> ExecuteEnumerableResultCore =
            (model, context, results) =>{
                var view = GetViewInstanceFromModel(model, context);
                var node = GetInteractionNodeFromView(view);
                new SequentialResult(results).Execute(null, node);
            };

        /// <summary>
        /// Executes an <see cref="IResult"/>, optionally looking up the <see cref="IInteractionNode"/>.
        /// </summary>
        public static Action<object, object, IResult> ExecuteResultCore =
            (model, context, result) =>{
                var view = GetViewInstanceFromModel(model, context);
                var node = GetInteractionNodeFromView(view);
                result.Execute(null, node);
            };

        /// <summary>
        /// Get the <see cref="IInteractionNode"/> associated with the view.
        /// </summary>
        public static Func<DependencyObject, IInteractionNode> GetInteractionNodeFromView =
            view =>{
                IInteractionNode node = null;
                if (view != null)
                    node = view.GetValue(DefaultRoutedMessageController.NodeProperty) as IInteractionNode;
                return node;
            };

        /// <summary>
        /// Gets the view instance associated with the model.
        /// </summary>
        public static Func<object, object, DependencyObject> GetViewInstanceFromModel =
            (model, context) =>{
                var container = model as IMetadataContainer;
                return container != null ? container.GetView<DependencyObject>(context) : null;
            };

        /// <summary>
        /// Executes the specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        public static void Execute(this IResult result)
        {
            result.ExecuteFor(null);
        }

        /// <summary>
        /// Executes the specified results.
        /// </summary>
        /// <param name="results">The results.</param>
        public static void Execute(this IEnumerable<IResult> results)
        {
            results.ExecuteFor(null);
        }

        /// <summary>
        /// Executes the result for the model.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="model">The model.</param>
        public static void ExecuteFor(this IResult result, object model)
        {
            result.ExecuteFor(model, null);
        }

        /// <summary>
        /// Executes the results for the model.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="model">The model.</param>
        public static void ExecuteFor(this IEnumerable<IResult> results, object model)
        {
            results.ExecuteFor(model, null);
        }

        /// <summary>
        /// Executes the result for the model within the given context.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        public static void ExecuteFor(this IResult result, object model, object context)
        {
            ExecuteResultCore(model, context, result);
        }

        /// <summary>
        /// Executes the results for the model within the given context.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        public static void ExecuteFor(this IEnumerable<IResult> results, object model, object context)
        {
            ExecuteEnumerableResultCore(model, context, results);
        }
    }
}