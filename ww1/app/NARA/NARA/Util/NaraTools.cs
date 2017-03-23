using NARA.Common_p.Model;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NARA.Util
{
    //NaraTools is static class with properties for font family, and auth. data for pure basic access to the API
    public enum Environments
    {
        Production,
        Testing
    }
    public static class NaraTools
    {
        public static Environments Environment { get { return Environments.Production; } }
        public static byte[] LoaderInByteArray { get; set; }
        public static string SelectedFont
        {
            get
            {
                if (Device.Idiom == TargetIdiom.Tablet)
                    return "TrebuchetMS";
                else
                    return "TrebuchetMS";
            }
            set { }
        }
        public static string ApiUsername
        {
            get
            {
                return "nara2016";
            }
            set { }
        }
        public static string ApiPassword
        {
            get
            {
                if (Environment == Environments.Production)
                {
                    return "20nara16";
                }
                else
                {
                    return "20semantika16";
                }
            }
            set { }
        }
        public static string ExplorePage
        {
            get
            {
                if (Environment == Environments.Testing)
                {
                    return "https://nara-test.semantika.eu/dvex/list";
                }
                else
                {
                    return "https://museu.ms/dvex/list";
                }
            }
        }
        public static string ProfileView
        {
            get
            {
                if (Environment == Environments.Testing)
                {
                    return "https://nara-test.semantika.eu/dvex/profileView";
                }
                else
                {
                    return "https://museu.ms/dvex/profileView";
                }
            }
        }
        public static string EditView
        {
            get
            {

                if (Environment == Environments.Testing)
                {
                    return "https://nara-test.semantika.eu/dvex/edit";
                }
                else
                {
                    return "https://museu.ms/dvex/edit";
                }

            }
        }
        public static string Host
        {
            get
            {

                if (Environment == Environments.Testing)
                {
                    return "nara-test.semantika.eu";
                }
                else
                {
                    return "museu.ms";
                }

            }
        }
        public static string Domain
        {
            get
            {

                if (Environment == Environments.Testing)
                {
                    return "https://nara-test.semantika.eu";
                }
                else
                {
                    return "https://museu.ms";
                }

            }
        }
        public static string Dvex
        {
            get
            {

                if (Environment == Environments.Testing)
                {
                    return "https://nara-test.semantika.eu/dvex";
                }
                else
                {
                    return "https://museu.ms/dvex";
                }

            }
        }
        public static string DvexList
        {
            get
            {

                if (Environment == Environments.Testing)
                {
                    return "https://nara-test.semantika.eu/dvex/list";
                }
                else
                {
                    return "https://museu.ms/dvex/list";
                }

            }
        }
        public static string TokenUrl
        {
            get
            {

                if (Environment == Environments.Testing)
                {
                    return "https://nara-test.semantika.eu/api/token";
                }
                else
                {
                    return "https://museu.ms/api/token";
                }

            }
        }
        /// <summary>
        /// //Reads stream and return a byte array of the selected image
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
        public static string Token { get; set; }
        public static int ContentIntervalCheck { get { return 120; } }
        public static bool LogingEnabled { get { return false; } }
    }
}
