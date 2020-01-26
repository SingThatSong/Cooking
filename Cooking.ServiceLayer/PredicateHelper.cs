using System;
using System.Linq;
using System.Linq.Expressions;

namespace ServiceLayer
{
    /// <summary>
    /// Extentions for Expression creation.
    /// </summary>
    internal static class PredicateHelper
    {
        /// <summary>
        /// Expression for true. Acts as a basis for expression assembly.
        /// </summary>
        /// <typeparam name="T">Any type used in expression chain.</typeparam>
        /// <returns>Expression for true.</returns>
        public static Expression<Func<T, bool>> True<T>() => _ => true;

        /// <summary>
        /// Expression for false. Acts as a basis for expression assembly.
        /// </summary>
        /// <typeparam name="T">Any type used in expression chain.</typeparam>
        /// <returns>Expression for false.</returns>
        public static Expression<Func<T, bool>> False<T>() => _ => false;

        /// <summary>
        /// Expression for |.
        /// </summary>
        /// <typeparam name="T">Any type used in expression chain.</typeparam>
        /// <param name="expr1">Left side of expression.</param>
        /// <param name="expr2">Right side of expression.</param>
        /// <returns>New expression, concatenating left and right expressions with | operator.</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                           Expression<Func<T, bool>> expr2)
        {
            InvocationExpression invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// Expression for &.
        /// </summary>
        /// <typeparam name="T">Any type used in expression chain.</typeparam>
        /// <param name="expr1">Left side of expression.</param>
        /// <param name="expr2">Right side of expression.</param>
        /// <returns>New expression, concatenating left and right expressions with & operator.</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                            Expression<Func<T, bool>> expr2)
        {
            InvocationExpression invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }
}
