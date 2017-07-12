using NARA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Model
{
    /// <summary>
    /// Interface that defines the methods, that have
    /// to be implemented on platform specific level.
    /// </summary>
    public interface IPlatformSpecific
    {
        string ConnectionString();
        bool CheckConnection();
        Task<List<string>> getPageDependencies(List<string> links);
        void SaveText(string text, bool append);
        string LoadText();
        void SavePictureToDisk(string filename, byte[] imageData, WebViewCustom webView, bool isImage = true, string url = "");
        void SaveImage(string p_Filename, string p_Url, WebViewCustom wvc);
    }
}
