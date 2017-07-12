using HtmlAgilityPack;
using NARA.Common_p.Model;
using NARA.Common_p.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using System.Net.Http.Headers;
using NARA.Common_p.Service;
using System.IO;

namespace NARA.Common_p.Util
{
    /// <summary>
    /// Helper class for storing html content, javascript/css dependencies
    /// </summary>
    public class WebUtil
    {

        OfflineRepository repo = new OfflineRepository(DependencyService.Get<IPlatformSpecific>().ConnectionString());

        public WebUtil()
        {
        }

        /// <summary>
        /// Retrieves html content from the url 
        /// </summary>
        /// <param name="url">Url of the source</param>
        /// <returns>Html content</returns>
        public async Task<List<string>> GetContent(string url)
        {
            string page = url;

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                return getDependenciesFromHtml(result);
            }
        }
        /// <summary>
        /// Clears cookies from the database
        /// </summary>
        public void ClearLogin()
        {
            repo.ClearCookies();
        }
        /// <summary>
        /// Checks if user is logged in or not and validation of stored cookies, if there
        /// is any
        /// </summary>
        /// <returns></returns>
        public bool CheckLogin()
        {
            var cookies = repo.GetCookies();

            //Checks if cookies exists
            if (cookies.Count != 0)
            {
                var cookie_ex = cookies.Where(i => i.Name == "nara").FirstOrDefault();
                if (cookie_ex != null)
                {
                    //Checks expiration date
                    if (cookie_ex.Expires > DateTime.Now)
                        return true;
                    else
                    {
                        //Clears cookies from database
                        ClearLogin();
                        return false;
                    }
                }
                //Clears cookies from database
                ClearLogin();
                return false;
            }
            else
            {
                //Clears cookies from database
                ClearLogin();
                return false;
            }
        }

        int retries = 0;
        /// <summary>
        /// Retrieves html content from the url, including dependencies
        /// </summary>
        /// <param name="url">Url of the source</param>
        /// <returns>Html content</returns>
        public async Task<string> GetContentHtml(string url)
        {
            try
            {
                //Retrieves cookies and set them for the request
                var cookiesR = repo.GetCookies();
                var cookieContainer = new CookieContainer();
                if (cookiesR != null)
                {
                    foreach (var cookie in cookiesR.Where(i => i.Name != "nara"))
                    {
                        Cookie cookieNet = new Cookie()
                        {
                            Value = cookie.Value,
                            Name = cookie.Name,
                            HttpOnly = false,
                            Expired = cookie.Expired,
                            Expires = DateTime.Now.AddDays(1),
                            Domain = cookie.Domain
                        };

                        var baseUri = new Uri(PlatformTools.Domain + "/");
                        cookieContainer.Add(baseUri, cookieNet);
                    }
                }

                //Retrieves html content from the source
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                using (HttpClient client = new HttpClient(handler))
                using (HttpResponseMessage response = await client.GetAsync(url))
                using (HttpContent content = response.Content)
                {
                    if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
                    {
                        string result = await content.ReadAsStringAsync();

                        return result;
                    }
                    DependencyService.Get<IPlatformSpecific>().SaveText("Error from WS inner : " + response.StatusCode + DateTime.Now.ToString("HH:mm:ss") + "| URL : " + url + " |" + Environment.NewLine, true);
                    return "";
                }
            }
            catch (Exception e)
            {
                DependencyService.Get<IPlatformSpecific>().SaveText("Error from WS : " + e.Message + DateTime.Now.ToString("HH:mm:ss") + "| URL : " + url + " |" + Environment.NewLine, true);
                return "";
            }
        }

        /// <summary>
        /// Retrieves javascript/css dependencies from html content
        /// </summary>
        /// <param name="content">Html content</param>
        /// <returns>Javascript/Css dependencies</returns>
        public List<string> getDependenciesFromHtml(string content)
        {
            List<string> links = new List<string>();

            try
            {
                //Parse html content using HtmlAgilityPack plugin
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(content);

                var all = doc.DocumentNode.Descendants("script").ToArray();
                var allLink = doc.DocumentNode.Descendants("link").ToArray();

                links.Add(content);
                links.Add("//fonts.googleapis.com/css?family=Raleway:400,700");

                //Checks for included scripts
                foreach (var script in doc.DocumentNode.Descendants("script").ToArray())
                {
                    if (script.Attributes.Count > 0)
                    {
                        HtmlAttribute text = (HtmlAttribute)script.Attributes.FirstOrDefault();

                        if (text.Value != null)
                            links.Add(text.Value);
                    }
                }

                //Checks for image tags and src properties
                var imgUrls = doc.DocumentNode.Descendants("img")
                                .Select(e => e.GetAttributeValue("src", null))
                                .Where(s => !String.IsNullOrEmpty(s)).ToList();

                links.AddRange(imgUrls);

                //Checks for images in background-image properties of a div tags
                var list = doc.DocumentNode.Descendants("div");
                var styleImgUrls1 = doc.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("style") && d.Attributes["style"].Value.Contains("background")).ToList();

                if (styleImgUrls1.Count > 0)
                {
                    int firstLenght = styleImgUrls1.FirstOrDefault().Attributes["style"].Value.LastIndexOf(")") - styleImgUrls1.FirstOrDefault().Attributes["style"].Value.IndexOf("url(");

                    var linkSS = styleImgUrls1.FirstOrDefault().Attributes["style"].Value.Substring(styleImgUrls1.FirstOrDefault().Attributes["style"].Value.IndexOf("url("));
                    var linkSSFinal = linkSS.Substring(5, firstLenght - 6);
                    links.Add(linkSSFinal);
                }

                var styleImgUrls = doc.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("style") && d.Attributes["style"].Value.Contains("background-image:url")).ToList();
                links.Add("/Areas/Eu/Content/Dvex/Images/scroll_down_indicator.png");

                //Retrieves links
                foreach (var url_s in styleImgUrls)
                {
                    int lenght = url_s.Attributes["style"].Value.IndexOf(')') - url_s.Attributes["style"].Value.IndexOf('(') - 3;
                    var link = (url_s.Attributes["style"].Value.Substring(url_s.Attributes["style"].Value.IndexOf('(') + 2));
                    links.Add(link.Substring(0, lenght));
                }

            }
            catch (Exception ex)
            { }

            return links;
        }

        /// <summary>
        /// Merges online content with offline so it can be used without network connection
        /// </summary>
        /// <param name="downloadedLinks">Downloaded dependencies</param>
        /// <param name="originalStrings">Original dependencies</param>
        /// <param name="content">Html content</param>
        /// <returns>Merged html content</returns>
        public string CorrectContent(List<string> downloadedLinks, List<string> originalStrings, string content)
        {
            for (int i = 0; i < downloadedLinks.Count; i++)
            {
                content = content.Replace(originalStrings[i], downloadedLinks[i]);
            }

            return content;
        }

        /// <summary>
        /// Appends script tags in the head section of html content
        /// </summary>
        /// <param name="content">Html content to which the scripts will be appended</param>
        /// <param name="js">Scripts that will be appended</param>
        /// <returns>Html content with appended scripts</returns>
        public string appendScript(string content, List<string> js)
        {
            foreach (var script in js)
            {
                string scriptAppend = "<script>" + script + "</script>";
                content = content.Insert(content.IndexOf("<head>") + 6, scriptAppend);
            }
            return content;
        }

        /// <summary>
        /// Saves html content to the offline repository
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        public async void SaveContent(string url, string content)
        {
            try
            {
                if (url != PlatformTools.EditView)
                {
                    await Task.Run(async () =>
                    {
                        //Retrieves html content from
                        List<string> result = getDependenciesFromHtml(content);
                        result.Remove(result.FirstOrDefault());
                        //Retrieves page dependencies
                        var downloaded = await DependencyService.Get<IPlatformSpecific>().getPageDependencies(result);

                        //Saves merged content
                        var offlineContent = CorrectContent(downloaded, result, content);
                        repo.SaveContent(new OfflineContent() { Content = offlineContent, Date = DateTime.Now, Url = url });
                    });
                }
            }
            catch (Exception e)
            { }
        }

        /// <summary>
        /// Retrieves cookies from the request and saves them to the database
        /// </summary>
        /// <param name="responseCookies"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> SaveCookies(List<Cookie> responseCookies, string username, string password)
        {
            try
            {
                //Sets content that will be sent to the auth. service
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Username", username),
                    new KeyValuePair<string, string>("Password", password)
                });

                //Sets up cookie handler for http client
                CookieContainer cookies = new CookieContainer();
                HttpClientHandler handler = new HttpClientHandler();
                handler.CookieContainer = cookies;

                using (HttpClient authClient = new HttpClient(handler))
                {

                    var uri = new Uri(PlatformTools.Domain);

                    authClient.BaseAddress = uri;
                    authClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sends request
                    HttpResponseMessage authenticationResponse = await authClient.PostAsync("/cms/account/login", formContent);

                    //Retrieves cookies
                    responseCookies = cookies.GetCookies(uri).Cast<Cookie>().ToList();

                    //Checks if cookies has been retrieved
                    if (responseCookies.Count > 1)
                    {
                        //Clears database of and existing cookies
                        repo.ClearCookies();

                        //Saves cookies to the database
                        foreach (var cookie in responseCookies)
                        {
                            repo.SaveCookie(username, DateTime.Now.AddDays(1), cookie.HttpOnly, cookie.Name, cookie.TimeStamp, cookie.Value, cookie.Domain, cookie.Secure, cookie.Discard, cookie.Expired);
                        }

                        repo.SaveCookie(username, DateTime.Now.AddDays(1), true, "nara", DateTime.Now, "", PlatformTools.Host, true, false, false);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
