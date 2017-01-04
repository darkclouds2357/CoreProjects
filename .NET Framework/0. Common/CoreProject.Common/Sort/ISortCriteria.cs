using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreProject.Common.Sort
{
    public interface ISortCriteria<T>
    {
        SortDirection Direction { get; set; }

        //-----------------------------------------------------------------------
        IOrderedQueryable<T> ApplyOrdering(IQueryable<T> query, Boolean useThenBy);
    }

    public enum SortDirection
    {
        Ascending = 0,
        Descending = 1
    }
}
