namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System;
    using System.Collections.Generic;
    using Core.InversionOfControl;
    using Core.Logging;
    using Views;

    /// <summary>
    /// Manages coroutine execution.
    /// </summary>
    public static class Coroutine
    {
        static readonly ILog Log = LogManager.GetLog(typeof(Coroutine));
        static IServiceLocator serviceLocator;
        static IBuilder builder;
        static Func<IEnumerator<IResult>, IResult> createParentEnumerator;

        /// <summary>
        /// Initializes the helper with the service locator.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="parentEnumeratorFactory">Creates the parent enumerator.</param>
        public static void Initialize(IServiceLocator serviceLocator, IBuilder builder, Func<IEnumerator<IResult>, IResult> parentEnumeratorFactory)
        {
            Coroutine.serviceLocator = serviceLocator;
            Coroutine.builder = builder;
            createParentEnumerator = parentEnumeratorFactory;
        }

        /// <summary>
        /// Executes the specified results.
        /// </summary>
        /// <param name="results">The results.</param>
        public static void Execute(IEnumerable<IResult> results)
        {
            Execute(results.GetEnumerator());
        }

        /// <summary>
        /// Executes the specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        public static void Execute(IResult result)
        {
            Execute(new[] { result });
        }

        /// <summary>
        /// Executes the result in the model's context.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="model">The model.</param>
        public static void ExecuteFor(IResult result, object model)
        {
            ExecuteFor(new[] { result }, model);
        }

        /// <summary>
        /// Executes the results for the model.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="model">The model.</param>
        public static void ExecuteFor(IEnumerable<IResult> results, object model)
        {
            ExecuteFor(results.GetEnumerator(), model);
        }

        /// <summary>
        /// Executes a coroutine.
        /// </summary>
        /// <param name="coroutine">The coroutine to execute.</param>
        public static void ExecuteFor(IEnumerator<IResult> coroutine, object model)
        {
            var view = View.GetViewInstanceFromModel(model, null);
            var node = View.GetInteractionNode(view);

            Execute(coroutine, new ResultExecutionContext(serviceLocator, null, node));
        }

        /// <summary>
        /// Executes a coroutine.
        /// </summary>
        /// <param name="coroutine">The coroutine to execute.</param>
        public static void Execute(IEnumerator<IResult> coroutine)
        {
            Execute(coroutine, new ResultExecutionContext(serviceLocator, null, null));
        }

        /// <summary>
        /// Executes a coroutine.
        /// </summary>
        /// <param name="coroutine">The coroutine to execute.</param>
        /// <param name="context">The context to execute the coroutine within.</param>
        public static void Execute(IEnumerator<IResult> coroutine, ResultExecutionContext context)
        {
            Log.Info("Executing coroutine.");

            var enumerator = createParentEnumerator(coroutine);
            builder.BuildUp(enumerator);

            enumerator.Completed += Completed;
            enumerator.Execute(context);
        }

        /// <summary>
        /// Called upon completion of a coroutine.
        /// </summary>
        public static event EventHandler<ResultCompletionEventArgs> Completed = (s, e) =>{
            var enumerator = (IResult)s;
            enumerator.Completed -= Completed;

            if(e.Error != null)
                Log.Error(e.Error);
            else if(e.WasCancelled)
                Log.Info("Coroutine execution cancelled.");
            else Log.Info("Coroutine execution completed.");
        };
    }
}