using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Model
{
    /// <summary>
    /// Class for storing offline content and is composed of
    /// content (JSON API result), date that was retrieved and the
    /// url from the source
    /// </summary>
    public class OfflineContent
    {
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public string Url { get; set; }
    }
}
