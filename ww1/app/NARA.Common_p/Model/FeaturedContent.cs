using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Model
{
    /// <summary>
    /// Class for storing properties for featured content, which
    /// are displayed on the homescreen
    /// </summary>
    public class FeaturedContent
    {
        public int PageSize { get; set; }
        public int Page { get; set; }
        public int TotalCount { get; set; }
        public bool SupportsPagination { get; set; }
        public List<Result> Results { get; set; }
    }
}
