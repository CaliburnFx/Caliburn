namespace Caliburn.Core.Invocation
{
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// A factory for delegates.
    /// </summary>
    public static class DelegateFactory
    {
        /// <summary>
        /// Represents a generic delegate to a function.
        /// </summary>
        public delegate object LateBoundFunc(object target, object[] arguments);

        /// <summary>
        /// Represents a generic delegate to a procedure.
        /// </summary>
        public delegate void LateBoundProc(object target, object[] arguments);

        /// <summary>
        /// Creates a delegate to the specified method.
        /// </summary>
        /// <typeparam name="T">Should be a <see cref="LateBoundFunc"/> or <see cref="LateBoundProc"/>.</typeparam>
        /// <param name="method">The method to create the delegate for.</param>
        /// <returns>The delegate.</returns>
        public static T Create<T>(MethodInfo method)
        {
            var instanceParameter = Expression.Parameter(typeof(object), "target");
            var argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

            MethodCallExpression call;

            if (method.IsStatic)
            {
                call = Expression.Call(
                    method,
                    CreateParameterExpressions(method, argumentsParameter)
                    );
            }
            else call = Expression.Call(
                    Expression.Convert(instanceParameter, method.DeclaringType),
                    method,
                    CreateParameterExpressions(method, argumentsParameter)
                    );

            var lambda = Expression.Lambda<T>(
                typeof(LateBoundProc).IsAssignableFrom(typeof(T))
                    ? call
                    : (Expression)Expression.Convert(call, typeof(object)),
                instanceParameter,
                argumentsParameter
                );

            return lambda.Compile();
        }

        private static Expression[] CreateParameterExpressions(MethodInfo method, Expression argumentsParameter)
        {
            return method.GetParameters().Select(
                (parameter, index) =>
                Expression.Convert(
                    Expression.ArrayIndex(argumentsParameter, Expression.Constant(index)),
                    parameter.ParameterType
                    )
                ).ToArray();
        }
    }
}