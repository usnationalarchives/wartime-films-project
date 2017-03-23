using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Util
{
    //Interface ISendMailService defines ComposeMail method, for implementation of mail composer
    public interface ISendMailService
    {
        void ComposeMail(string[] recipients, string subject, string messagebody = null, Action<bool> completed = null);
    }
}
