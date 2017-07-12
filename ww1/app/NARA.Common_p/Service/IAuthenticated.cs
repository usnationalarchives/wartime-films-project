using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Service
{
    public interface IAuthenticated
    {
        string AuthenticationToken { get; set; }
    }
}
