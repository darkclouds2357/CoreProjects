using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreProject.Common.Extensions
{
    public static class LambdaExpressionExtensions
    {
        public static IQueryable<TDto> Select<T, TDto>(this IQueryable<T> source)
        {
            return null;
        }
    }
}
