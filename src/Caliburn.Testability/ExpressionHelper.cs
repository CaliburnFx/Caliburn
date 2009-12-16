using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Caliburn.Testability
{
	internal static class ExpressionHelper
	{
		public static string GetPathFromExpression<T,K>(Expression<Func<T, K>> property)
		{
			var expressionString = property.ToString();
			var dotIndex = expressionString.IndexOf('.');
			return expressionString.Substring(dotIndex + 1);
		}
	}
}
