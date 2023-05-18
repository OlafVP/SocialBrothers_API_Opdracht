using System;
using System.Linq;
using System.Linq.Expressions;

namespace API_Opdracht.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string sortBy, bool isAscending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, sortBy);
            var selector = Expression.Lambda(property, parameter);

            var methodName = isAscending ? "OrderBy": "OrderByDescending";
            var method = typeof(Queryable).GetMethods()
                .FirstOrDefault(m => m.Name == methodName && m.GetParameters().Length == 2)
                ?.MakeGenericMethod(typeof(T), property.Type);

            
            if (method == null)
                throw new InvalidOperationException($"Sorting method '{methodName}' not found.");

            return (IQueryable<T>)method.Invoke(null, new object[] { query, selector });
        }

        public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            var searchExpression = typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(string))
                .Select(property =>
                {
                    var propertyAccess = Expression.Property(parameter, property);
                    var searchValue = Expression.Constant(searchTerm);
                    var containsCall = Expression.Call(propertyAccess, containsMethod, searchValue);

                    return (Expression)Expression.Equal(containsCall, Expression.Constant(true));
                })
                .Aggregate(Expression.OrElse);

            var lambda = Expression.Lambda<Func<T, bool>>(searchExpression, parameter);
            return query.Where(lambda);
        }

    }
}
