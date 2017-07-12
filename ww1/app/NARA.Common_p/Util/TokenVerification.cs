using NARA.Common_p.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Util
{
    /// <summary>
    /// Singleton for retrieving API tokens
    /// </summary>
    public class TokenVerification
    {
        public static ApiToken instance;
        private static OAuthClient oauth = new OAuthClient();
        public TokenVerification() { }
        private static string TokenUrl = PlatformTools.Domain;

        public static ApiToken Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ApiToken();
                }
                return instance;
            }
        }

        /// <summary>
        /// Retrieves token from the auth. API
        /// </summary>
        /// <param name="AuthClientId">Client id</param>
        /// <param name="AuthClientSecret">Client secret</param>
        /// <returns>ApiToken data</returns>
        public ApiToken GetToken(string AuthClientId, string AuthClientSecret)
        {
            if (instance == null || instance.expires_in < 300 /*|| token.token_type != "bearer"*/ || instance.access_token.Length == 0)
            {
                return oauth.GetAuthorizationToken(TokenUrl, AuthClientId, AuthClientSecret);
            }
            else
            {
                return instance;
            }
        }

    }
}
