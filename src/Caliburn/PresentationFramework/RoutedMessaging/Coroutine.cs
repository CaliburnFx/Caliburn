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
        /// Executes the specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        public static void BeginExecute(IResult result)
        {
            BeginExecute(new[] { result });
        }

        /// <summary>
        /// Executes the result in the model's context.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="model">The model.</param>
        public static void BeginExecuteFor(IResult result, object model)
        {
            BeginExecuteFor(new[] { result }, model);
        }

        /// <summary>
        /// Executes the specified results.
        /// </summary>
        /// <param name="results">The results.</param>
        public static void BeginExecute(IEnumerable<IResult> results)
        {
            BeginExecute(results.GetEnumerator());
        }

        /// <summary>
        /// Executes the specified results.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="callback">The completion callback.</param>
        public static void BeginExecute(IEnumerable<IResult> results, EventHandler<ResultCompletionEventArgs> callback)
        {
            BeginExecute(results.GetEnumerator(), callback);
        }

        /// <summary>
        /// Executes the results for the model.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="model">The model.</param>
        public static void BeginExecuteFor(IEnumerable<IResult> results, object model)
        {
            BeginExecuteFor(results.GetEnumerator(), model);
        }

        /// <summary>
        /// Executes the results for the model.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="model">The model.</param>
        /// <param name="callback">The completion callback.</param>
        public static void BeginExecuteFor(IEnumerable<IResult> results, object model, EventHandler<ResultCompletionEventArgs> callback)
        {
            BeginExecuteFor(results.GetEnumerator(), model, callback);
        }

        /// <summary>
        /// Executes a coroutine.
        /// </summary>
        /// <param name="coroutine">The coroutine to execute.</param>
        public static void BeginExecuteFor(IEnumerator<IResult> coroutine, object model)
        {
            BeginExecuteFor(coroutine, model, null);
        }

        /// <summary>
        /// Executes a coroutine.
        /// </summary>
        /// <param name="coroutine">The coroutine to execute.</param>
        /// <param name="callback">The completion callback.</param>
        public static void BeginExecuteFor(IEnumerator<IResult> coroutine, object model, EventHandler<ResultCompletionEventArgs> callback)
        {
            var view = View.GetViewInstanceFromModel(model, null);
            var node = View.GetInteractionNode(view);

            BeginExecute(coroutine, new ResultExecutionContext(serviceLocator, null, node), callback);
        }

        /// <summary>
        /// Executes a coroutine.
        /// </summary>
        /// <param name="coroutine">The coroutine to execute.</param>
        public static void BeginExecute(IEnumerator<IResult> coroutine)
        {
            BeginExecute(coroutine, new ResultExecutionContext(serviceLocator, null, null));
        }

        /// <summary>
        /// Executes a coroutine.
        /// </summary>
        /// <param name="coroutine">The coroutine to execute.</param>
        /// <param name="callback">The completion callback.</param>
        public static void BeginExecute(IEnumerator<IResult> coroutine, EventHandler<ResultCompletionEventArgs> callback)
        {
            BeginExecute(coroutine, new ResultExecutionContext(serviceLocator, null, null), callback);
        }

        /// <summary>
        /// Executes a coroutine.
        /// </summary>
        /// <param name="coroutine">The coroutine to execute.</param>
        /// <param name="context">The context to execute the coroutine within.</param>
        public static void BeginExecute(IEnumerator<IResult> coroutine, ResultExecutionContext context)
        {
            BeginExecute(coroutine, context, null);
        }

        /// <summary>
        /// Executes a coroutine.
        /// </summary>
        /// <param name="coroutine">The coroutine to execute.</param>
        /// <param name="context">The context to execute the coroutine within.</param>
        /// <param name="callback">The completion callback.</param>
        public static void BeginExecute(IEnumerator<IResult> coroutine, ResultExecutionContext context, EventHandler<ResultCompletionEventArgs> callback)
        {
            Log.Info("Executing coroutine.");

            var enumerator = createParentEnumerator(coroutine);
            builder.BuildUp(enumerator);

            if (callback != null)
                enumerator.Completed += callback;
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