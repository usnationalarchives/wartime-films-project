using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Model
{
    /// <summary>
    /// Mapping class for retrieving SuperTag entity from
    /// museu.ms platform API
    /// </summary>
    public class SuperTag
    {
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int ItemCount { get; set; }
        public string Name { get; set; }
    }
}
