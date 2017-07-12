using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Model
{
    public class AdditionalData
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTime Date { get; set; }
    }
}
