using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Models
{
    /// <summary>
    /// Generic paginated list model
    /// </summary>
    public class PageList<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<T> List { get; set; } = new List<T>();

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

        /// <summary>
        /// Project list items to a different type
        /// </summary>
        public PageList<TTarget> Project<TTarget>(Func<T, TTarget> project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            return new PageList<TTarget>
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                TotalCount = TotalCount,
                List = List?.Select(project).ToList() ?? new List<TTarget>(),
            };
        }
    }
}
