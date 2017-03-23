using AVFoundation;
using Foundation;
using NARA.Common_p.Model;
using NARA.iOS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using NARA.iOS.Util;
using NARA.Util;

[assembly: Dependency(typeof(PlatformSpecific))]
namespace NARA.iOS
{
    /// <summary>
    /// Logic that is implemented on platform specific level, as defined in IPlatformSpecific interface
    /// </summary>
    public class PlatformSpecific : IPlatformSpecific
    {

        public PlatformSpecific() { }

        //Db file path
        public string ConnectionString()
        {
            return System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "naraDB.db3");
        }

        //Checks for internet connection
        public bool CheckConnection()
        {
            return NARA.iOS.Util.Reachability.IsHostReachable(NaraTools.Host);
        }

        //Retrieves content from dependencies links (Javascript, CSS)
        public async Task<List<string>> getPageDependencies(List<string> links)
        {
            List<string> downloadedLinks = new List<string>();
            List<string> js_s = new List<string>();
            int i = 0;

            foreach (var link in links)
            {
                //Corrects urls
                string linkToDownload = "";
                if (link.StartsWith("/") && !link.StartsWith("//"))
                    linkToDownload = NaraTools.Domain + link;
                else
                    linkToDownload = link;

                if (link.Contains("css"))
                    linkToDownload = "http:" + link;

                //Sets destination of a file containing content of the dependencies
                var destination = System.IO.Path.Combine(
                System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal),
                    System.IO.Path.GetFileName(link));

                //Corrects destination file path
                if (destination.Length > 20 && link.Contains("jpg"))
                {
                    destination = destination.Substring(0, destination.IndexOf("jpg") - 1) + i.ToString() + ".jpg";
                }
                else
                {
                    if (destination.Contains("?"))
                    {
                        if (destination.Contains("css"))
                            destination = destination.Substring(0, destination.LastIndexOf("?")) + i.ToString() + ".css";
                        else
                            destination = destination.Substring(0, destination.LastIndexOf("?")) + ".js";
                    }
                }

                //Removes file if it already exists
                if (File.Exists(destination))
                    File.Delete(destination);

                var webClient = new WebClient();

                //Binds event of file download completion
                webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;

                if (linkToDownload.StartsWith("//"))
                {
                    linkToDownload = "http:" + link;
                }

                //Downloads file and add destination to list of downloaded links
                await webClient.DownloadFileTaskAsync(new System.Uri(linkToDownload), destination);
                downloadedLinks.Add(destination);

                i++;
            }

            return downloadedLinks;
        }

        /// <summary>
        /// Event triggers when file download is completed
        /// </summary>
        void webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

        }

        /// <summary>
        /// Saving text to the file, used for logging purposes
        /// </summary>
        /// <param name="text">Text to be saved to the file</param>
        /// <param name="append">Bool flag if text should be appended</param>
        public void SaveText(string text, bool append)
        {
            try
            {
                string filename = "naraLog.txt";
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(documentsPath, filename);
                if (append == true)
                {
                    System.IO.File.AppendAllText(filePath, text);
                }
                else
                {
                    System.IO.File.WriteAllText(filePath, text);
                }

            }
            catch { }
            //Console.WriteLine("Text Saved To: " + filePath);
        }

        /// <summary>
        /// Loads text from text file, used for logging
        /// </summary>
        /// <returns>Log text</returns>
        public string LoadText()
        {
            try
            {
                string filename = "naraLog.txt";
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(documentsPath, filename);
                Console.WriteLine("Text Loaded From: " + filePath);
                return System.IO.File.ReadAllText(filePath);
            }
            catch { return ""; }
        }
    }
}
