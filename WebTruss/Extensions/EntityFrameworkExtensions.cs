using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebTruss.Pagination;

namespace WebTruss.Extensions
{
    public static class EntityFrameworkExtensions
    {
        public static IOrderedQueryable<TSource> Order<TSource>(this IQueryable<TSource> source, string propertyName, bool useOrderBy, bool ascending, object comparer = null)
        {
            IOrderedQueryable<TSource> query;

            // LAMBDA: x => x.[PropertyName]
            var parameter = Expression.Parameter(typeof(TSource), "x");
            Expression property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            if (comparer == null)
            {
                // REFLECTION: source.[OrderMethod](x => x.[PropertyName])
                var orderMethod = useOrderBy ?
                    ascending ?
                        typeof(Queryable).GetMethods().First(x => x.Name == "OrderBy" && x.GetParameters().Length == 2) :
                        typeof(Queryable).GetMethods().First(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 2) :
                    ascending ?
                        typeof(Queryable).GetMethods().First(x => x.Name == "ThenBy" && x.GetParameters().Length == 2) :
                        typeof(Queryable).GetMethods().First(x => x.Name == "ThenByDescending" && x.GetParameters().Length == 2);

                var orderMethodGeneric = orderMethod.MakeGenericMethod(typeof(TSource), property.Type);

                query = (IOrderedQueryable<TSource>)orderMethodGeneric.Invoke(null, new object[] { source, lambda });
            }
            else
            {
                // REFLECTION: source.[OrderMethod](x => x.[PropertyName], comparer)
                var orderMethod = useOrderBy ?
                    ascending ?
                        typeof(Queryable).GetMethods().First(x => x.Name == "OrderBy" && x.GetParameters().Length == 3) :
                        typeof(Queryable).GetMethods().First(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 3) :
                    ascending ?
                        typeof(Queryable).GetMethods().First(x => x.Name == "ThenBy" && x.GetParameters().Length == 3) :
                        typeof(Queryable).GetMethods().First(x => x.Name == "ThenByDescending" && x.GetParameters().Length == 3);

                var orderMethodGeneric = orderMethod.MakeGenericMethod(typeof(TSource), property.Type);

                query = (IOrderedQueryable<TSource>)orderMethodGeneric.Invoke(null, new[] { source, lambda, comparer });
            }

            return query;
        }

        public static async Task<PagedResult<TModel>> PaginateAsync<TModel>(
            this IQueryable<TModel> query,
            int page,
            int limit,
            CancellationToken cancellationToken)
            where TModel : class
        {
            var totalItems = await query.CountAsync(cancellationToken);

            if (page <= 0 || limit <= 0)
            {
                var data = await query
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

                return new PagedResult<TModel>(1, totalItems, totalItems, 1, data);
            }

            page = (page < 0) ? 1 : page;
            var startRow = (page - 1) * limit;
            var items = await query
                       .Skip(startRow)
                       .Take(limit)
                       .AsNoTracking()
                       .ToListAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalItems / (double)limit);

            return new PagedResult<TModel>(page, limit, totalItems, totalPages, items);
        }
    }
}
