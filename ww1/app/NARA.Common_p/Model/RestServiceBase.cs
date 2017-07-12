using NARA.Common_p.Repository;
using NARA.Common_p.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Model
{
    /// <summary>
    /// Base class for rest service provider
    /// </summary>
    public abstract class RestServiceBase : IAuthenticated
    {
        public string AuthenticationToken
        { get; set; }


        protected IRestBackendProvider m_Provider;
        protected string personalDir = "";
        protected OfflineRepository offlineRepository;
        protected bool connected;
        /// <summary>
        /// Creates a new instance with the specified token and provider
        /// </summary>
        /// <param name="p_Token">Token to use for authentication</param>
        /// <param name="p_Provider">Provider used for data retrieval</param>
        public RestServiceBase(string p_Token, IRestBackendProvider p_Provider, bool p_connected, string p_offlineCon)
        {
            AuthenticationToken = p_Token;
            m_Provider = p_Provider;
            offlineRepository = new OfflineRepository(p_offlineCon);
            connected = p_connected;
        }
    }
}
