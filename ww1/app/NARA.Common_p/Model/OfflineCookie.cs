using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Model
{
    /// <summary>
    /// Mapping class for storing retrieved cookies
    /// </summary>
    public class OfflineCookie
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Domain { get; set; }
        public DateTime Expires { get; set; }
        public bool HttpOnly { get; set; }
        public string Name { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Value { get; set; }
        public bool Discard { get; set; }
        public bool Expired { get; set; }
        public bool Secure { get; set; }
    }
}
