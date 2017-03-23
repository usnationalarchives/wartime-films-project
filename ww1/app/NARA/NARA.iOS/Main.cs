using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using System.IO;

namespace NARA.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");


            AppDomain.CurrentDomain.UnhandledException += (sender, argss) =>
            {
                var nsex = argss.ExceptionObject as NSException;

                if (nsex != null)
                {
                    var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var filename = Path.Combine(documents, "nara_log.txt");

                    File.WriteAllText(filename, nsex.Description);
                }
            };
        }
    }
}
