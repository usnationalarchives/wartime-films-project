using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Service
{
    public interface IRestBackendProvider : IAuthenticated
    {
        /// <summary>
        /// Return an object of the specified type for the given relative parameter url
        /// </summary>
        /// <typeparam name="T">Type of the object to return</typeparam>
        /// <param name="p_Url">Relative url to get the object from</param>
        /// <returns>Object of the type T from the specified Url</returns>
        Task<T> LoadData<T>(string p_Url) where T : new();


        /// <summary>
        /// Return an object of the specified type for the given rest request
        /// </summary>
        /// <typeparam name="T">Type of the return object</typeparam>
        /// <param name="p_RestRequest">REST request to get the object</param>
        /// <returns>Object of type T based on the specified request</returns>
        Task<T> LoadData<T>(HttpRequestMessage p_RestRequest, bool saveCookies = false) where T : new();
    }
}
