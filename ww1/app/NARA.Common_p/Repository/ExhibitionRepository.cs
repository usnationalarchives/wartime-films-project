using NARA.Common_p.Model;
using NARA.Common_p.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Repository
{
    public class ExhibitionRepository:RestServiceBase
    {
        public ExhibitionRepository(string p_Token, IRestBackendProvider p_Provider, bool p_connected, string p_offlineCon)
            : base(p_Token, p_Provider, p_connected, p_offlineCon)
        {

        }

        /// <summary>
        /// Add's default collection
        /// </summary>
        /// <param name="user">User to be added to database</param>
        /// <returns>User data with status flag</returns>
        public async Task<bool> AddDefaultCollection()
        {
            string requestUrl = String.Format("/api/vex/my/add/newvexlink/0/My%20Collection?p_externalId=");

            var docs = await m_Provider.LoadData<bool>(requestUrl);
            return docs;
        }
    }
}
