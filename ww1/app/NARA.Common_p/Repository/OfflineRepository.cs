using NARA.Common_p.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Repository
{
    /// <summary>
    /// OfflineRepository is implemented for storing data for offline use, such as
    /// html content, js scripts, css stylesheets and for user profile info using SQL Lite DB
    /// </summary>
    public class OfflineRepository
    {
        string con;
        /// <summary>
        /// Constructor that creates SQLiteConnection and
        /// db tables if they don't already exists
        /// </summary>
        /// <param name="connection">Platform specific db path</param>
        public OfflineRepository(string connection)
        {
            con = connection;
            var db = new SQLiteConnection(connection);
            db.CreateTable<OfflineContent>();
            db.CreateTable<OfflineCookie>();
            db.CreateTable<Credentials>();
            db.CreateTable<User>();
            db.CreateTable<AdditionalData>();
            db.Close();
        }

        /// <summary>
        /// Save retrieved offline JSON content
        /// </summary>
        /// <param name="content">JSON content, that is saved to DB</param>
        public void SaveContent(OfflineContent content)
        {
            using (var db = new SQLiteConnection(con))
            {
                db.InsertOrReplace(content);
            }
        }

        /// <summary>
        /// Retrieves offline JSON content
        /// </summary>
        /// <param name="url">url of the JSON result, that should be retrieved</param>
        public OfflineContent GetContent(string url)
        {

            using (var db = new SQLiteConnection(con))
            {
                var all = db.Table<OfflineContent>().Where(i => i.Url == url);

                return db.Table<OfflineContent>().Where(i => i.Url == url).LastOrDefault();
            }
        }
        /// <summary>
        /// Save additional data
        /// </summary>
        /// <param name="data">Additional data, that is saved to DB</param>
        public void SaveAdditionalData(AdditionalData data)
        {
            using (var db = new SQLiteConnection(con))
            {
                db.Insert(data);
            }
        }

        /// <summary>
        /// Retrieves additional data
        /// </summary>
        public List<AdditionalData> GetAdditionalData()
        {
            using (var db = new SQLiteConnection(con))
            {
                return db.Table<AdditionalData>().ToList();
            }
        }

        /// <summary>
        /// Retrieves stored sign-in credentials
        /// </summary>
        public Credentials GetCredentials()
        {
            using (var db = new SQLiteConnection(con))
            {

                return db.Table<Credentials>().LastOrDefault();
            }
        }

        /// <summary>
        /// Stores sign-in credentials
        /// </summary>
        public void SaveCredentials(Credentials credentials)
        {
            using (var db = new SQLiteConnection(con))
            {
                db.InsertOrReplace(credentials);
            }
        }

        /// <summary>
        /// Stores currently signed in user
        /// </summary>
        public void SaveUser(User user)
        {
            ClearUser();
            using (var db = new SQLiteConnection(con))
            {
                db.InsertOrReplace(user);
            }
        }

        /// <summary>
        /// Retrieves currently signed-in user
        /// </summary>
        public User GetUser()
        {
            using (var db = new SQLiteConnection(con))
            {
                return db.Table<User>().LastOrDefault();
            }
        }

        /// <summary>
        /// Clears stored users from database
        /// </summary>
        public void ClearUser()
        {
            using (var db = new SQLiteConnection(con))
            {
                db.Execute("delete from " + db.Table<User>().Table.TableName);
            }
        }

        /// <summary>
        /// Clears stored credentials from database
        /// </summary>
        public void ClearCredentials()
        {
            using (var db = new SQLiteConnection(con))
            {
                db.Execute("delete from " + db.Table<Credentials>().Table.TableName);
            }
        }

        /// <summary>
        /// Retrieves stored cookies from db table
        /// </summary>
        public List<OfflineCookie> GetCookies()
        {
            using (var db = new SQLiteConnection(con))
            {
                return db.Table<OfflineCookie>().ToList();
            }
        }

        /// <summary>
        /// Retrieves token _museums_access_token from currently signed-in user
        /// </summary>
        public string GetToken()
        {
            using (var db = new SQLiteConnection(con))
            {
                return db.Table<OfflineCookie>().Where(i => i.Name == "_museums_access_token").FirstOrDefault().Value;
            }
        }

        /// <summary>
        /// Removes stored cookies from db table
        /// </summary>
        public void ClearCookies()
        {
            using (var db = new SQLiteConnection(con))
            {
                var cookies = db.Table<OfflineCookie>().ToList();
                foreach (var cookie in cookies)
                {
                    db.Delete(cookie);
                }
            }
        }

        /// <summary>
        /// Inserts cookies in the database
        /// </summary>
        public void SaveCookie(string username, DateTime expires, bool httpOnly, string name, DateTime timeStamp, string value, string domain, bool secure, bool discard, bool expired)
        {
            using (var db = new SQLiteConnection(con))
            {
                OfflineCookie content = new OfflineCookie()
                {
                    Username = username,
                    Expires = expires,
                    HttpOnly = httpOnly,
                    Name = name,
                    TimeStamp = timeStamp,
                    Value = value,
                    Domain = domain
                };

                db.Insert(content);
            }
        }
    }
}
