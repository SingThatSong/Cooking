using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Cooking.Data.Context;

/// <summary>
/// Model builde extention.
/// See https://stackoverflow.com/a/55514664.
/// </summary>
public static class ModelBuilderExtension
{
    /// <summary>
    /// Apply global filters to all types inherited from T.
    /// </summary>
    /// <typeparam name="T">Base type for entries.</typeparam>
    /// <param name="modelBuilder">Model builder.</param>
    /// <param name="expression">Expression to filter.</param>
    public static void ApplyGlobalFilters<T>(this ModelBuilder modelBuilder, Expression<Func<T, bool>> expression)
    {
        IEnumerable<Type>? entities = modelBuilder.Model
            .GetEntityTypes()
            .Where(e => typeof(T).IsAssignableFrom(e.ClrType))
            .Select(e => e.ClrType);
        foreach (Type? entity in entities)
        {
            ParameterExpression? newParam = Expression.Parameter(entity);
            Expression? newbody = ReplacingExpressionVisitor.Replace(expression.Parameters.Single(), newParam, expression.Body);
            modelBuilder.Entity(entity).HasQueryFilter(Expression.Lambda(newbody, newParam));
        }
    }
}
