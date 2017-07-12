using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using NARA.Droid;
using NARA.Common_p.Model;
using Android.Net;
using System.Net;
using System.IO;
using System.Threading.Tasks;

[assembly: Dependency(typeof(PlatformSpecific))]
namespace NARA.Droid
{
    public class PlatformSpecific : IPlatformSpecific
    {
        public string ConnectionString()
        {
            return System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "naraDB.db3");
        }

        public bool CheckConnection()
        {
            var connectivityManager = (ConnectivityManager)Android.App.Application.Context.GetSystemService(Context.ConnectivityService);
            var activeNetworkInfo = connectivityManager.ActiveNetworkInfo;
            if (activeNetworkInfo != null && activeNetworkInfo.IsConnectedOrConnecting)
                return true;
            else
                return false;
        }

        int i = 0;
        public async Task<List<string>> getPageDependencies(List<string> links)
        {
            List<string> downloadedLinks = new List<string>();
            //List<string> js_s = new List<string>();
            ////downloadedLinks.Add(links.FirstOrDefault());
            ////links.Remove(links.FirstOrDefault());

            ////var myDestination = System.IO.Path.Combine(
            ////System.Environment.GetFolderPath(
            ////    System.Environment.GetFolderPath(System)),
            ////    System.IO.Path.GetFileName(link));

            //foreach (var link in links)
            //{
            //    string linkToDownload = "";
            //    if (link.StartsWith("/") && !link.StartsWith("//"))
            //        linkToDownload = NaraTools.EditView + link;
            //    else
            //        linkToDownload = link;

            //    if (link.Contains("css"))
            //        linkToDownload = "http:" + link;


            //    var destination = System.IO.Path.Combine(
            //    System.Environment.GetFolderPath(
            //        System.Environment.SpecialFolder.Personal),
            //        System.IO.Path.GetFileName(link));

            //    var path = Path.Combine(Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "Android", "data", "NARA.NARA"), System.IO.Path.GetFileName(link));

            //    if (destination.Length > 20 && link.Contains("jpg"))
            //    {
            //        destination = destination.Substring(0, destination.IndexOf("jpg") - 1) + i.ToString() + ".jpg";

            //    }
            //    else
            //    {
            //        if (destination.Contains("?"))
            //        {
            //            if(destination.Contains("css"))
            //                destination = destination.Substring(0, destination.LastIndexOf("?")) + i.ToString() + ".css";
            //            else
            //                destination = destination.Substring(0, destination.LastIndexOf("?")) + ".js";
            //        }
            //    }

            //    if (File.Exists(destination))
            //        File.Delete(destination);



            //    var webClient = new WebClient();
            //    webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;

            //    if (linkToDownload.StartsWith("//"))
            //    {
            //        linkToDownload = "http:" + link;
            //    }

            //    //if (!linkToDownload.StartsWith("//"))
            //    //{
            //        await webClient.DownloadFileTaskAsync(new System.Uri(linkToDownload),
            //                                                destination);


            //        downloadedLinks.Add(destination);
            //        //js_s.Add(System.IO.File.ReadAllText(path));
            //    //}
            //    i++;
            //}

            return downloadedLinks;
        }

        void webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

        }

        public void SaveText(string text, bool append)
        {
        }

        public string LoadText()
        {
            return "";
        }          public void SavePictureToDisk(string filename, byte[] imageData, WebViewCustom wvc, bool isImage = true, string url = "")
        {
            var dir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim);
            var pictures = dir.AbsolutePath;
            //adding a time stamp time file name to allow saving more than one image... otherwise it overwrites the previous saved image of the same name
            string name = filename + System.DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
            string filePath = System.IO.Path.Combine(pictures, name);
            try
            {
                System.IO.File.WriteAllBytes(filePath, imageData);
                //mediascan adds the saved image into the gallery
                var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(filePath)));
                Xamarin.Forms.Forms.Context.SendBroadcast(mediaScanIntent);
                wvc.InvokeShowMessage(true);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
                wvc.InvokeShowMessage(false);
            }

        }          public void SaveImage(string p_Filename, string p_Url, WebViewCustom wvc)
        {
            try
            {
                var webClient = new WebClient();

                webClient.DownloadDataAsync(new System.Uri(p_Url));
                webClient.DownloadDataCompleted += (s, e) =>
                {
                    var bytes = e.Result; // get the downloaded data
                SavePictureToDisk(p_Filename, bytes, wvc);
                };
            }
            catch(Exception e)
            {
                wvc.InvokeShowMessage(false);
            }
        } 
    }
}