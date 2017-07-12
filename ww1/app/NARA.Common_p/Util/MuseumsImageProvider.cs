using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Util
{
    public class MuseumsImageProvider
    {
        /// <summary>
        /// Gets a uri to the image which is created from logical uri of museums images
        /// </summary>
        /// <param name="pUrl">Logical museums image path</param>
        /// <param name="pWidth">Desired image width</param>
        /// <param name="pHeight">Desired image height</param>
        /// <param name="pShouldCrop">Should image be cropped</param>
        /// <returns>Returns image URI</returns>
        public static Uri GetImageUri(string pUrl, int pWidth = 100, int pHeight = 100, bool pShouldCrop = true)
        {
            if (string.IsNullOrWhiteSpace(pUrl))
            {
                return null;
            }

            pUrl = pUrl.Replace("\\", "/");
            string url = pUrl.StartsWith("http")
                             ? pUrl
                             : "https://museums.blob.core.windows.net/data" + pUrl.Replace(" ", "%20");

            var b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(url));
            var hmac = CreateHmac(
                Encoding.UTF8.GetBytes("laf#lg383ht()/;:O(/)(/)=g987ewt;2twfqw"), Encoding.UTF8.GetBytes(b64));

            string shortHash = CleanUpUrl(Convert.ToBase64String(hmac.Take(8).ToArray()));
            string mode = pShouldCrop ? "crop" : "max";

            string size = string.Empty;
            if (pWidth > 0 && pHeight > 0)
            {
                size = string.Format("&width={0}&height={1}", pWidth, pHeight);
            }

            string newUrl =
                string.Format(
                    "http://museu.ms/remote.jpg.ashx?mode={3}&format=png{1}&404=no_image.gif&urlb64={0}&hmac={2}",
                    b64,
                    size,
                    shortHash,
                    mode);

            return new Uri(newUrl);
        }

        /// <summary>
        /// Replaces unsupported symbols with supported ones.
        /// </summary>
        /// <param name="pUrl">The url.</param>
        /// <returns>Returns cleaned url.</returns>
        private static string CleanUpUrl(string pUrl)
        {
            return pUrl.Replace("=", string.Empty).Replace('+', '-').Replace('/', '_');
        }

        /// <summary>
        /// The create HMAC.
        /// </summary>
        /// <param name="secret">The secret.</param>
        /// <param name="message">The message.</param>
        /// <returns>Returns encrypted message.</returns>
        private static IEnumerable<byte> CreateHmac(byte[] secret, byte[] message)
        {
            HMac digest = new HMac(new Sha256Digest());

            digest.Init(new KeyParameter(secret));
            digest.BlockUpdate(message, 0, message.Length);

            byte[] output = new byte[digest.GetMacSize()];

            digest.DoFinal(output, 0);
            return output;
        }
    }
}
