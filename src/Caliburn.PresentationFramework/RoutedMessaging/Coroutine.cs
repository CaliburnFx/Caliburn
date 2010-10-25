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

        /// <summary>
        /// Creates the parent enumerator.
        /// </summary>
        public static Func<IEnumerator<IResult>, IResult> CreateParentEnumerator = inner => new SequentialResult(inner);

        /// <summary>
        /// Initializes the helper with the service locator.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public static void Initialize(IServiceLocator serviceLocator)
        {
            Coroutine.serviceLocator = serviceLocator;
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
        /// Executes the results for the model.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="model">The model.</param>
        public static void ExecuteFor(this IEnumerable<IResult> results, object model)
        {
            var view = View.GetViewInstanceFromModel(model, null);
            var node = View.GetInteractionNode(view);

            Execute(results.GetEnumerator(), new ResultExecutionContext(serviceLocator, null, node));
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

            var enumerator = CreateParentEnumerator(coroutine);
            IoC.BuildUp(enumerator);

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