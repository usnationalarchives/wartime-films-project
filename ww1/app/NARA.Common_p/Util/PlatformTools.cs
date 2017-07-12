using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Util
{
    public enum Environments
    {
        Production,
        Testing
    }
    public static class PlatformTools
    {
        public static Environments Environment { get { return Environments.Testing; } }
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

    }
}
