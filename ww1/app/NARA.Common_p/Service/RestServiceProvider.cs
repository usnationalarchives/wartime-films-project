using NARA.Common_p.Model;
using NARA.Common_p.Repository;
using NARA.Common_p.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NARA.Common_p.Service
{
    public class RestServiceProvider : IAuthenticated, IRestBackendProvider
    {
        public string OfflineString { get; set; }
        /// <summary>
        /// Stores the root url of the REST endpoint to be used
        /// </summary>
        private static string m_EndPointUrl;
        CookieCollection responseCookiesString;
        WebUtil webUtil = new WebUtil();
        List<Cookie> responseCookies = new List<Cookie>();

        /// <summary>
        /// Stores the authentication token used for accessing the backend
        /// </summary>
        public string AuthenticationToken
        {
            get;
            set;
        }

        /// <summary>
        /// Stores the number of retries for the server to try to access the backend. The number can be set between 1 and 10 and will default to 3 if specified outside of this range.
        /// </summary>
        private int m_Retries = 3;

        /// <summary>
        /// Property defining the number of auto retires to access the data from the API. If the 
        /// </summary>
        public int AutoRetries
        {
            get
            {
                return m_Retries;
            }
            set
            {
                if (value < 0 || value > 10)
                {
                    m_Retries = 3;
                }
                else
                {
                    m_Retries = value;
                }
            }
        }

        /// <summary>
        /// Constructs a new instance of the REST provider and loads the endpoint url from the confirguration, if not already loaded
        /// </summary>
        /// 

        //TODO - URL !
        public RestServiceProvider(string p_Token)
        {
            if (String.IsNullOrEmpty(m_EndPointUrl))
            {
                m_EndPointUrl = PlatformTools.Domain;
            }
            AuthenticationToken = p_Token;
        }

        /// <summary>
        /// Returns the object from the specified relative url
        /// </summary>
        /// <typeparam name="T">Type of the object to retrieve</typeparam>
        /// <param name="p_Url"></param>
        /// <returns>Object of the specified type for the given relative url</returns>
        public async Task<T> LoadData<T>(string p_Url) where T : new()
        {
            //Consturct a new request based on the provided url
            HttpRequestMessage request = new HttpRequestMessage();
            request = new HttpRequestMessage(HttpMethod.Get, p_Url);
            //Execute the request
            return await LoadData<T>(request);
        }

        /// <summary>
        /// Return an object of the specified type for the given rest request
        /// </summary>
        /// <typeparam name="T">Type of the return object</typeparam>
        /// <param name="p_RestRequest">REST request to get the object</param>
        /// <returns>Object of type T based on the specified request</returns>
        public async Task<T> LoadData<T>(HttpRequestMessage p_RestRequest, bool saveCookies = false) where T : new()
        {
            Exception lastException = null;
            OfflineRepository offlineRepo = new OfflineRepository(DependencyService.Get<IPlatformSpecific>().ConnectionString());
            HttpResponseMessage result = new HttpResponseMessage();

            for (int i = 0; i < AutoRetries; i++)
            {
                try
                {

                    string resultContent = "";
                    using (HttpClient client = new HttpClient())
                    {

                        client.BaseAddress = new Uri(m_EndPointUrl);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticationToken);
                        client.DefaultRequestHeaders
                              .Accept
                              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        result = await client.SendAsync(p_RestRequest);
                        resultContent = await result.Content.ReadAsStringAsync();
                    }


                    if (result.IsSuccessStatusCode && result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        offlineRepo.SaveContent(new OfflineContent() { Content = resultContent, Date = DateTime.Now, Url = p_RestRequest.RequestUri.LocalPath });

                        var response = JsonConvert.DeserializeObject<T>(resultContent);

                        return response;
                    }
                    else if (offlineRepo.GetContent(p_RestRequest.RequestUri.LocalPath) != null)
                    {

                        return JsonConvert.DeserializeObject<T>(offlineRepo.GetContent(p_RestRequest.RequestUri.LocalPath).Content);
                    }
                    else
                    {

                        StringBuilder errorMessage = new StringBuilder("Error message: ").Append(result.ReasonPhrase).Append("; ");
                        errorMessage.Append("Return code: ").Append(result.StatusCode).Append("; ");
                        errorMessage.Append("Response status: ").Append(result.ReasonPhrase).Append("; ");

                        throw new Exception(errorMessage.ToString(), new Exception(result.ReasonPhrase));
                    }

                }
                catch (Exception ex)
                {
                    //In case of exception, set the last exception and retry
                    lastException = ex;
                }
                finally
                {
                    result.Dispose();
                }
            }

            if (offlineRepo.GetContent(p_RestRequest.RequestUri.LocalPath) != null)
            {
                return JsonConvert.DeserializeObject<T>(offlineRepo.GetContent(p_RestRequest.RequestUri.LocalPath).Content);
            }
            else
            {
                //If we are here, the request failed for the maximum number of retries
                throw new Exception("Failed to retrieve data from the backend", lastException);
            }
        }

        /// <summary>
        /// Clones the HttpRequestMessage
        /// </summary>
        /// <param name="req">HttpRequestMessage to be cloned</param>
        /// <returns>cloned HttpRequestMessage</returns>
        public static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage req)
        {
            HttpRequestMessage clone = new HttpRequestMessage(req.Method, req.RequestUri);

            // Copy the request's content (via a MemoryStream) into the cloned object
            var ms = new MemoryStream();
            if (req.Content != null)
            {
                await req.Content.CopyToAsync(ms).ConfigureAwait(false);
                ms.Position = 0;
                clone.Content = new StreamContent(ms);

                // Copy the content headers
                if (req.Content.Headers != null)
                    foreach (var h in req.Content.Headers)
                        clone.Content.Headers.Add(h.Key, h.Value);
            }


            clone.Version = req.Version;

            foreach (KeyValuePair<string, object> prop in req.Properties)
                clone.Properties.Add(prop);

            foreach (KeyValuePair<string, IEnumerable<string>> header in req.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return clone;
        }
    }
}
