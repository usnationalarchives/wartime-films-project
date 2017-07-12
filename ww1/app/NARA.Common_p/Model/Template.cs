using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Model
{
    /// <summary>
    /// Mapping class for retrieving Template entity from
    /// museu.ms platform API
    /// </summary>
    public class Template
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int MasterTemplateId { get; set; }
        public int GroupId { get; set; }
    }
}
