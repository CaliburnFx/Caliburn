namespace Caliburn.Testability
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Hosts extension methods related to expression trees.
    /// </summary>
    internal static class ExpressionHelper
    {
        /// <summary>
        /// Gets the path from the expression.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <typeparam name="K">The type of property.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns>The path.</returns>
        public static string GetPathFromExpression<T, K>(Expression<Func<T, K>> property)
        {
            var expressionString = property.ToString();
            var dotIndex = expressionString.IndexOf('.');
            return expressionString.Substring(dotIndex + 1);
        }
    }
}