using NARA.Common_p.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Util
{

    public class OAuthClient
    {
        private static JsonSerializer m_Serializer = new JsonSerializer();

        /// <summary>
        /// The post body template to use to send the username and password to 
        /// </summary>
        private static readonly string m_BodyTemplate = "username={0}&password={1}&grant_type=password";

        /// <summary>
        /// Get the raw Json response from the authentication server
        /// </summary>
        /// <param name="url">The url of the authentication server</param>
        /// <param name="username">Username to use for authentication</param>
        /// <param name="password">Password to use for authentication</param>
        /// <returns>Body of the server response in string format</returns>
        private static string GetPostContent(string url, string username, string password)
        {

            //The data template to use
            string postBody = String.Format(m_BodyTemplate, username, password);



            //var httpClient = new HttpClient();

            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            //request.
            //var response = httpClient.SendAsync(request).Result;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/token");
            request.Content = new StringContent(postBody, Encoding.UTF8, "application/x-www-form-urlencoded");


            var result = client.SendAsync(request).Result;
            string resultContent = result.Content.ReadAsStringAsync().Result;



            return resultContent;
        }

        /// <summary>
        /// Return the authentication token by utilizing the provided username and password
        /// </summary>
        /// <param name="url">The url of the authentication server</param>
        /// <param name="username">Username to use for authentication</param>
        /// <param name="password">Password to use for authentication</param>
        /// <returns>THe ApiToken object returned by the server for the provided credentials</returns>
        public ApiToken GetAuthorizationToken(string url, string username, string password)
        {
            try
            {
                //Read and desiralize the esponse from server based on the result
                ApiToken token = JsonConvert.DeserializeObject<ApiToken>(GetPostContent(url, username, password));

                if (token == null || token.expires_in < 300 /*|| token.token_type != "bearer"*/ || token.access_token.Length == 0)
                {
                    //If the token is null, is not returned at all or expires in less than 5 minutes / has already expired or is not a bearer token, throw an exception.
                    throw new SecurityException("Token received but is not valid", null);
                }
                else
                {
                    return token;
                }
            }
            catch (Exception ex)
            {
                throw new SecurityException("Failed to get token due to an internal error", ex);
            }
        }

    }

}
