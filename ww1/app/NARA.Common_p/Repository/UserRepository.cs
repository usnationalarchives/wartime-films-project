using NARA.Common_p.Model;
using NARA.Common_p.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Repository
{
    public class UserRepository : RestServiceBase
    {
        /// <summary>
        /// Creates a new instance of user repository with the specified token and provider
        /// </summary>
        /// <param name="p_Token">Token to use for authentication</param>
        /// <param name="p_Provider">Provider used for data retrieval</param>
        /// <param name="p_connected">Is connected to the internet</param>
        public UserRepository(string p_Token, IRestBackendProvider p_Provider, bool p_connected, string p_offlineCon)
            : base(p_Token, p_Provider, p_connected, p_offlineCon)
        {

        }

        /// <summary>
        /// Add's new user in the database
        /// </summary>
        /// <param name="user">User to be added to database</param>
        /// <returns>User data with status flag</returns>
        public async Task<User> AddUser(User user)
        {
            string requestUrl = String.Format("/api/user/register");

            HttpRequestMessage request = new HttpRequestMessage();
            request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            string n = JsonConvert.SerializeObject(user);
            request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.Unicode, "application/json");

            var docs = await m_Provider.LoadData<User>(request);
            return docs;
        }

        /// <summary>
        /// Updates user profile data
        /// </summary>
        /// <param name="user">User data, that will be updated</param>
        /// <returns>User data with status flag</returns>
        public async Task<User> UpdateUser(User user)
        {
            string requestUrl = String.Format("/api/user/updateprofile");

            HttpRequestMessage request = new HttpRequestMessage();
            request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            string n = JsonConvert.SerializeObject(user);
            request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.Unicode, "application/json");

            var docs = await m_Provider.LoadData<User>(request);
            return docs;
        }

        /// <summary>
        /// Changes user's password
        /// </summary>
        /// <param name="user">User data, with passwords to change</param>
        /// <returns>Status flag</returns>
        public async Task<User> UserChangePassword(User user)
        {
            string requestUrl = String.Format("/api/user/changepassword");

            HttpRequestMessage request = new HttpRequestMessage();
            request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.Unicode, "application/json");

            var docs = await m_Provider.LoadData<User>(request);
            return docs;
        }

        /// <summary>
        /// Sends email of password reset to the user
        /// </summary>
        /// <param name="user">User data, with email address</param>
        /// <returns>Status flag</returns>
        public async Task<User> UserForgotPassword(User user)
        {
            string requestUrl = String.Format("/api/user/forgotpassword");

            HttpRequestMessage request = new HttpRequestMessage();
            request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.Unicode, "application/json");

            var docs = await m_Provider.LoadData<User>(request);
            return docs;
        }

        /// <summary>
        /// Add's user to database using Facebook authentication token
        /// </summary>
        /// <param name="user">User data structure that holds Facebook authentication token</param>
        /// <returns></returns>
        public async Task<User> AddUserViaFacebook(User user)
        {
            string requestUrl = String.Format("/api/user/registerviafacebook");

            HttpRequestMessage request = new HttpRequestMessage();
            request = new HttpRequestMessage(HttpMethod.Post, requestUrl);

            request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.Unicode, "application/json");

            var docs = await m_Provider.LoadData<User>(request, true);
            return docs;
        }

        /// <summary>
        /// Retrieves user data by it's username
        /// </summary>
        /// <param name="username">Username of user which data will be retrieved</param>
        /// <returns></returns>
        public async Task<User> GetUserByUsername(string username)
        {
            string requestUrl = String.Format("/api/user/{0}", username);

            var docs = await m_Provider.LoadData<User>(requestUrl);
            return docs;
        }

        /// <summary>
        /// Saves or updates user profile picture
        /// </summary>
        /// <param name="user">User data, to which the profile picture belongs</param>
        /// <param name="image">Image that will be uploaded</param>
        /// <returns></returns>
        public async Task<User> UpdateUserProfilePicture(User user, byte[] image)
        {
            string requestUrl = String.Format("api/media/uploaduserprofilepicture");

            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(image);
            var n = new ContentDispositionHeaderValue("attachment");

            var encoded = WebUtility.UrlEncode(user.RegistrationUserName);

            n.FileName = user.ImgUrl;
            n.Parameters.Add(new NameValueHeaderValue("username", encoded));
            n.Parameters.Add(new NameValueHeaderValue("id", user.Id.ToString()));
            fileContent.Headers.ContentDisposition = n;

            content.Add(fileContent);
            request.Content = content;

            return await m_Provider.LoadData<User>(request);
        }
    }
}
