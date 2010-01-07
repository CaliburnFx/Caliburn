namespace Caliburn.Testability
{
    using System;
    using System.Linq.Expressions;

    internal static class ExpressionHelper
    {
        public static string GetPathFromExpression<T, K>(Expression<Func<T, K>> property)
        {
            var expressionString = property.ToString();
            var dotIndex = expressionString.IndexOf('.');
            return expressionString.Substring(dotIndex + 1);
        }
    }
}