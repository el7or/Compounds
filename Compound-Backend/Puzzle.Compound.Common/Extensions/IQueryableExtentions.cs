using Puzzle.Compound.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Puzzle.Compound.Common.Extensions
{
    public static class IQueryableExtentions
    {
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, PagedInput queryObject, Dictionary<string, Expression<Func<T, object>>> columnMap)
        {
            if (string.IsNullOrEmpty(queryObject.SortBy) // there in not SortBy in the url query
                || !columnMap.ContainsKey(queryObject.SortBy)) // SortBy not avaliable in the columnMap!!!
                return query;

            if (queryObject.IsSortAscending)
                return query.OrderBy(columnMap[queryObject.SortBy]); //Get the expression from our dictionary
            else
                return query.OrderByDescending(columnMap[queryObject.SortBy]);
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, PagedInput queryObject)
        {
            if (queryObject.PageNumber <= 0)
                queryObject.PageNumber = 1;
            if (queryObject.PageSize <= 0)
                queryObject.PageSize = 20;

            return query.Skip((queryObject.PageNumber - 1) * queryObject.PageSize).Take(queryObject.PageSize);
        }
    }
}
